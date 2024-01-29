using AceJobAgency.Model;
using AceJobAgency.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages
{
    public class LoginModel : PageModel
    {
        public bool IsLockedOut = false;

        [BindProperty]
        public Login LModel { get; set; }

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IHttpContextAccessor contxt;
        private ILogger<LoginModel> _logger;
        private readonly IEmailSender _emailSender;
        public LoginModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, ILogger<LoginModel> logger, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.contxt = httpContextAccessor;
            _logger = logger;
            this._emailSender = emailSender;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, false, true);

                if (identityResult.Succeeded)
                {
                    _logger.LogInformation(LModel.Email);
                    var user = await userManager.FindByNameAsync(LModel.Email);

                    if (user != null)
                    {
                        // Add SessionId
                        var sessionId = Guid.NewGuid().ToString();
                        user.SessionId = sessionId;
                        await userManager.UpdateAsync(user);
                        contxt.HttpContext.Session.SetString("SessionId", sessionId);

                        if (user.LastPasswordChange.AddDays(120) < DateTime.Now)
                        {
                            // Password change to recently, please wait another day
                            return RedirectToPage("ForgetPassword");
                        }
                        else if (!user.TwoFactorEnabled)
                        {
                            await signInManager.SignOutAsync();
							var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");
							var message = $"Your authentication code is: {token}";

							_emailSender.SendEmailAsync(user.Email, "2FA Token", message);

							return RedirectToPage($"/TwoFactorAuthentication", new { email = user.Email });
						}
                        else
                        {
                            return RedirectToPage("Index");
                        }
                        //var user = await userManager.GetUserAsync(User);
                        //_logger.LogInformation(user.Id);
                        //if (user != null)
                        //{
                        //    user.SessionId = contxt.HttpContext.Session.Id;
                        //    await userManager.UpdateAsync(user);
                        //} 
                    }
                    _logger.LogInformation("No user");

                }
                else if (identityResult.IsLockedOut)
                {
                    ModelState.AddModelError("", "Account Lockout for 10 mins, try again later.");
                    IsLockedOut = true;
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password incorrect");
                }
            }
            if (!ModelState.IsValid)
            {
                // Inspect ModelState errors
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                    }
                }
            }


            return Page();
        }
    }
}

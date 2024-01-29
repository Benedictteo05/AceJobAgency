using AceJobAgency.Model;
using AceJobAgency.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages
{
    public class ResetPasswordModel : PageModel
    {
        [BindProperty]
        public ResetPassword RPModel { get; set; }

        private readonly UserManager<ApplicationUser> userManager;
        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            RPModel = new ResetPassword();
        }
        public void OnGet(string email, string token)
        {
            RPModel.Email = email;
            RPModel.Token = token;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(RPModel.Email);

                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<ApplicationUser>();

                    if (CheckPasswordValidation(user, passwordHasher, RPModel.Password))
                    {
                        ModelState.AddModelError(nameof(RPModel.Password), "Password cannot be one of the last two passwords.");
                        return Page();
                    }


                    var result = await userManager.ResetPasswordAsync(user, RPModel.Token, RPModel.Password);

                    if (result.Succeeded)
                    {
                        var tempOldPassowrd = user.PasswordHash;
                        user.LastPasswordChange = DateTime.Now;
                        user.SecondOldPassword = user.OldPassword;
                        user.OldPassword = tempOldPassowrd;

                        await userManager.UpdateAsync(user);
                        return RedirectToPage("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User Not found");
                }
            }

            return Page();
        }

        private bool CheckPasswordValidation(ApplicationUser user, PasswordHasher<ApplicationUser> passwordHasher, string newPassword)
        {
            return passwordHasher.VerifyHashedPassword(user, user.PasswordHash, newPassword) == PasswordVerificationResult.Success ||
               passwordHasher.VerifyHashedPassword(user, user.OldPassword, newPassword) == PasswordVerificationResult.Success ||
               passwordHasher.VerifyHashedPassword(user, user.SecondOldPassword, newPassword) == PasswordVerificationResult.Success;
        }
    }
}


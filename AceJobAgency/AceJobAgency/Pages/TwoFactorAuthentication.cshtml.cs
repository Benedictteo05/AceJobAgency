using AceJobAgency.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages
{
	public class TwoFactorAuthenticationModel : PageModel
	{
		[BindProperty]
		public string TwoFactorCode { get; set; }
		[BindProperty(SupportsGet = true)]
		public string Email { get; set; }


		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<TwoFactorAuthenticationModel> logger;
		private readonly AuthDbContext authDbContext;
		public TwoFactorAuthenticationModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<TwoFactorAuthenticationModel> logger, AuthDbContext authDbContext)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.logger = logger;
			this.authDbContext = authDbContext;
		}

		public void OnGet(string email)
		{
			Email = email;
		}


		public async Task<IActionResult> OnPostAsync()
		{
			logger.LogInformation(Email);
			var user = await userManager.FindByNameAsync(Email);
			logger.LogInformation("Ran start");
			if (user != null)
			{
				logger.LogInformation(user.Gender);
				var isTokenValid = await userManager.VerifyTwoFactorTokenAsync(user, "Email", TwoFactorCode);
				if (isTokenValid)
				{
					logger.LogInformation("Valid Token");
					// Add Session Id;
					await signInManager.SignInAsync(user, false);
					var SessionId = Guid.NewGuid().ToString();
					user.SessionId = SessionId;
					HttpContext.Session.SetString("SessionId", SessionId);

					user.TwoFactorEnabled = true;
					await userManager.UpdateAsync(user);

					if (user?.LastPasswordChange.AddDays(90) < DateTime.Now)
					{
						// Password has expired
						// Redirect user to change password page
						return Redirect("ChangePassword");
					}
					else
					{
						var auditLog = new AuditLogs()
						{
							Logs = "User ID: " + user.Email + " Login",
                            CreatedAt = DateTime.UtcNow,
                        };
						authDbContext.AuditLogs.Add(auditLog);
						authDbContext.SaveChanges();
						logger.LogInformation("Redirect");
						return RedirectToPage("Index");
					}
				}
				else
				{
					ModelState.AddModelError("", "2FA Code is incorrect");
				}
			}
			return Page();
		}
	}
}

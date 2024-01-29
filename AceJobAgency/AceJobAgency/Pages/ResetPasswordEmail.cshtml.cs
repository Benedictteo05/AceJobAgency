using AceJobAgency.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace AceJobAgency.Pages
{
    public class ResetPasswordEmailModel : PageModel
    {
        [BindProperty]
        public string UserEmail { get; set; }
		private readonly IEmailSender emailSender;
		private readonly ILogger<ResetPasswordEmailModel> _logger;
		private readonly UserManager<ApplicationUser> userManager;
		public void OnGet()
        {

        }

		public ResetPasswordEmailModel(IEmailSender emailSender, ILogger<ResetPasswordEmailModel> logger, UserManager<ApplicationUser> userManager)
		{
			this._logger = logger;
			this.emailSender = emailSender;
			this.userManager = userManager;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByNameAsync(UserEmail);
				if (user != null)
				{
					var token = await userManager.GeneratePasswordResetTokenAsync(user);
					var userEmail = user.Email;
					var resetLink = $"{Request.Scheme}://{Request.Host}/ResetPassword?Email={userEmail}&token={HttpUtility.UrlEncode(token)}";

					await emailSender.SendEmailAsync(user.Email, "Password Reset", $"Click here to reset your password: {resetLink}");
				}
				ModelState.AddModelError(nameof(UserEmail), "Reset Password Link was sent to that Email");
			}
			return Page();
		}

	}
}

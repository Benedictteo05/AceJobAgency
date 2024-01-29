using AceJobAgency.Model;
using AceJobAgency.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages
{
    [Authorize]
    public class ForgetPasswordModel : PageModel
    {
        [BindProperty]
        public ForgetPassword FPModel { get; set; }

        private UserManager<ApplicationUser> userManager { get; }
        public ForgetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existingUser = await userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if (existingUser == null)
                {
                    ModelState.AddModelError("", "User cannot be found");
                    return Page();
                }
                else
                {
                    var validateOldPW = await userManager.CheckPasswordAsync(existingUser, FPModel.OldPassword);
                    if (validateOldPW)
                    {

                        var passwordHasher = new PasswordHasher<ApplicationUser>();

                        if (existingUser?.LastPasswordChange.AddHours(1) > DateTime.Now)
                        {
                            // Password change to recently, please wait another day
                            ModelState.AddModelError("", "Password change is too recent");
                            return Page();
                        }

                        if (CheckPasswordValidation(existingUser, passwordHasher, FPModel.Password))
                        {
                            ModelState.AddModelError(nameof(FPModel.Password), "Password cannot be one of the last two passwords.");
                            return Page();
                        }
                        var isCurrentPasswordValid = await userManager.CheckPasswordAsync(existingUser, FPModel.Password);
                        if (isCurrentPasswordValid)
                        {
                            ModelState.AddModelError("", "User cannot reuse the past two passwords.");
                            return Page();
                        }
                        existingUser.OldPassword = userManager.PasswordHasher.HashPassword(existingUser, FPModel.OldPassword);
                        var token = await userManager.GeneratePasswordResetTokenAsync(existingUser);
                        // Password Check

                        var tempPassword = existingUser.OldPassword;
                        existingUser.OldPassword = existingUser.PasswordHash;
                        existingUser.SecondOldPassword = tempPassword;

                        await userManager.UpdateAsync(existingUser);

                        var result = await userManager.ResetPasswordAsync(existingUser, token, FPModel.Password);
                        return RedirectToPage("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Old Password is incorrect");
                        return Page();
                    }
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

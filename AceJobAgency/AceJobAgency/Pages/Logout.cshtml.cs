using AceJobAgency.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IHttpContextAccessor contxt;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor contxt, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.contxt = contxt;
            this.userManager = userManager;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            var user = await userManager.GetUserAsync(User);
            await signInManager.SignOutAsync();
            contxt.HttpContext.Session.Clear();
            user.SessionId = Guid.NewGuid().ToString();
            await userManager.UpdateAsync(user);
            return RedirectToPage("Login");
        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("Index");
        }
    }
}

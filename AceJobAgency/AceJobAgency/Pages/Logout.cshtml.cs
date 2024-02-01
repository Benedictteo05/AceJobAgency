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
        private readonly AuthDbContext authDbContext;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor contxt, UserManager<ApplicationUser> userManager, AuthDbContext authDbContext)
        {
            this.signInManager = signInManager;
            this.contxt = contxt;
            this.userManager = userManager;
            this.authDbContext = authDbContext;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            var user = await userManager.GetUserAsync(User);

            var auditLog = new AuditLogs()
            {
                Logs = "User ID: " + user.Email + " Logout",
                CreatedAt = DateTime.UtcNow,
            };
            authDbContext.AuditLogs.Add(auditLog);
            authDbContext.SaveChanges();
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

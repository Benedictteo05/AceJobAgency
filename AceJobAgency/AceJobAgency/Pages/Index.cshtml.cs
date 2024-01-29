using AceJobAgency.Model;
using AceJobAgency.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Web;

namespace AceJobAgency.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string NRIC { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ResumeFile { get; set; }
        public string WhoAmI { get; set; }

        private UserManager<ApplicationUser> userManager { get; }
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment environment;
        private readonly IHttpContextAccessor contxt;

        public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            _logger = logger;
            this.environment = environment;
            this.contxt = httpContextAccessor;
            this.signInManager = signInManager;
        }

        //public void OnGet()
        //{

        //}

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                if (contxt.HttpContext.Session.GetString("SessionId") != user.SessionId)
                {
                    await signInManager.SignOutAsync();
                    contxt.HttpContext.Session.Clear();
                    return RedirectToPage("Login");
                }
                _logger.LogInformation(contxt.HttpContext.Session.Id);
                var dataProtectedProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectedProvider.CreateProtector("MySecretKey");
                var filePath = Path.Combine(environment.ContentRootPath, @"wwwroot/uploads", user.ResumeFile);
                if (System.IO.File.Exists(filePath))
                {
                    using var fileStream = new FileStream(filePath, FileMode.Open);
                    _logger.LogInformation(filePath);
                    FirstName = user.UserName;
                    LastName = user.LastName;
                    Gender = user.Gender;
                    NRIC = protector.Unprotect(user.NRIC);
                    Email = user.Email;
                    DateOfBirth = user.DateOfBirth;
                    ResumeFile = user.ResumeFile;
                    WhoAmI = HttpUtility.HtmlDecode(user.WhoAmI);
                }

                return Page();
            }

        }


    }
}
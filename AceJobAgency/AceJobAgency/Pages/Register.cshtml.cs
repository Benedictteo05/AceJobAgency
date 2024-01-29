using AceJobAgency.Model;
using AceJobAgency.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NanoidDotNet;
using System.Net;
using System.Web;

namespace AceJobAgency.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private ILogger _logger;
        private readonly IWebHostEnvironment environment;
        [BindProperty]
        public Register RModel { get; set; }
        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<RegisterModel> logger, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _logger = logger;
            this.environment = environment;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dataProtectedProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectedProvider.CreateProtector("MySecretKey");
            if (ModelState.IsValid)
            {
                var existingUser = await userManager.FindByEmailAsync(RModel.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email already exists. Please use another Email Address");
                    return Page();
                }
                if (Path.GetExtension(RModel.ResumeFile.FileName) != ".pdf" && Path.GetExtension(RModel.ResumeFile.FileName) != ".docx")
                {
                    ModelState.AddModelError("", "Resume File only accepts .pdf and .docx files");
                    return Page();
                }
                var id = Nanoid.Generate(size: 10);
                var filename = id + Path.GetExtension(RModel.ResumeFile.FileName);
                // store file in project
                var filePath = Path.Combine(environment.ContentRootPath, @"wwwroot/uploads", filename);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                RModel.ResumeFile.CopyTo(fileStream);
                _logger.LogInformation(filename);
                var user = new ApplicationUser()
                {
                    UserName = RModel.Email,
                    FirstName = RModel.FirstName,
                    LastName = RModel.LastName,
                    Gender = RModel.Gender,
                    NRIC = protector.Protect(RModel.NRIC),
                    Email = RModel.Email,
                    DateOfBirth = RModel.DateOfBirth,
                    ResumeFile = filename,
                    WhoAmI = HttpUtility.HtmlEncode(RModel.WhoAmI),
                };
                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }

        public class ReCaptchaResponse
        {
            public bool Success { get; set; }
            public decimal Score { get; set; }
            public string[] ErrorCodes { get; set; }
        }

        private bool ValidateRecaptcha(string response)
        {
            HttpClient httpClient = new HttpClient();

            var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret=6LeVOWEpAAAAAM1mHgIzVq6_7Ns5sWyB8JUTuQ_F&response={response}").Result;

            if (res.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            var googleRes = res.Content.ReadFromJsonAsync<ReCaptchaResponse>().Result;
            Console.WriteLine($"ReCaptcha Score: {googleRes.Score}");

            if (!googleRes.Success || googleRes.Score < 0.5m)
            {
                return false;
            }

            return true;
        }
    }
}

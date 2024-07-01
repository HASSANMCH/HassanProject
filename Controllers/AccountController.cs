using HassanProject.Data;
using HassanProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace HassanProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Generate verification code
                var verificationCode = new Random().Next(100000, 999999).ToString();
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    City = model.City,
                    VerificationCode = verificationCode

                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Send verification code via email (implement the method)
                    await SendVerificationCodeEmailAsync(user.Email, verificationCode);

                    // Sign out the user after registration
                    await _signInManager.SignOutAsync();

                    return RedirectToAction("VerifyEmail", "Account", new { userId = user.Id });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
        private async Task SendVerificationCodeEmailAsync(string email, string verificationCode)
        {
            var fromAddress = new MailAddress("chmhassan48@gmail.com", "TEst");
            var toAddress = new MailAddress(email);
            const string fromPassword = "pzchgexidqojybjl";
            const string subject = "Email Verification Code";
            string body = $"Your verification code is {verificationCode}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
        [HttpGet]
        public IActionResult VerifyEmail(string userId)
        {
            var model = new VerifyEmailViewModel { UserId = userId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null && user.VerificationCode == model.VerificationCode)
                {
                    user.EmailConfirmed = true; // Set EmailConfirmed to true
                    user.VerificationCode = null; // Clear the verification code
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        // Assign roles based on the email address 
                        if (user.Email == "admin@admin.com")
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");
                        }
                        else if (user.Email == "manager@admin.com")
                        {
                            await _userManager.AddToRoleAsync(user, "Manager");
                        }
                        else if (user.Email == "superadmin@admin.com")
                        {
                            await _userManager.AddToRoleAsync(user, "SuperAdmin");
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, "User");
                        }
                        return RedirectToAction("Index", "Home", new { userId = user.Id });
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid verification code.");
            return View(model);
        } 
        // Login Method
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                  
                    
                        return RedirectToAction("Index", "Home", new { id = user.Id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");                                                                                                                                                                                                                                                                                                                                                                                                                                           
                }
            }
            return View(model);
        }
        // Logout Method
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

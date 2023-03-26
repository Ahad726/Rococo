using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using Rococo.Models.ViewModels;
using Rococo.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Rococo.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(
             UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Register(string returnUrl = null)
        {
            var registerModel = new RegisterModel
            {
                ReturnUrl = returnUrl,
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                RoleList = _roleManager.Roles.Where(x => x.Name != SD.Role_User_Indi).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            return View(registerModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    CompanyId = model.CompanyId,
                    StreetAddress = model.StreetAddress,
                    City = model.City,
                    State = model.State,
                    PostalCode = model.PostalCode,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    Role = model.Role
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_Employee))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_User_Comp))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_User_Indi))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi));
                    }

                    if (user.Role == null)
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_User_Indi);
                    }
                    else
                    {
                        if (user.CompanyId > 0)
                        {
                            await _userManager.AddToRoleAsync(user, SD.Role_User_Comp);
                        }
                        var role = _roleManager.Roles.Where(x => x.Id == user.Role).FirstOrDefault();

                        await _userManager.AddToRoleAsync(user, role.Name);
                    }


                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        nameof(ConfirmEmail),
                        "Account",
                        values: new { area = "Customer", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(new List<Message>
                    {
                        new Message
                        {
                            Receiver = model.Email,
                            Subject = "Confirm your email",
                            Body =  $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>."
                        }
                    });

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction("RegisterConfirmation", new { email = model.Email });
                    }
                    else
                    {
                        if (user.Role == null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(model.ReturnUrl);
                        }
                        else
                        {
                            //admin is registering new user
                            return RedirectToAction(nameof(Index), "User", new { Area = "Admin" });
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            // regenerate select List in view
            model.CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            model.RoleList = _roleManager.Roles.Where(x => x.Name != SD.Role_User_Indi).Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            var model = new LoginModel
            {
                ReturnUrl = returnUrl ?? Url.Content("~/")

            };

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    //Get user id
                    var user = _unitOfWork.ApplicationUser.GetAll(u => u.Email == model.Email).FirstOrDefault();

                    // Get user's all products count and 
                    var count = _unitOfWork.ShoppingCart
                        .GetAll(c => c.ApplicationUserId == user.Id)
                        .ToList().Count();

                    //save the count to session
                    HttpContext.Session.SetInt32(SD.ssShoppingCartKey, count);

                    return LocalRedirect(model.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            HttpContext.Session.Remove(SD.ssShoppingCartKey);

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(Index), "Home");
            }
        }
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Register");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            ViewBag.StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            return View();
        }
    }
}

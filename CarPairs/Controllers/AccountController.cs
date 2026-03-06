using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using CarPairs.Web.Services.Interfaces;

namespace CarPairs.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountApiService _accountService;

        public AccountController(IAccountApiService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var loginRequest = new LoginRequest 
                { 
                    Email = model.Email, 
                    Password = model.Password 
                };
                
                var authResponse = await _accountService.LoginAsync(loginRequest);

                if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                {
                    // Store JWT token in session for API calls
                    HttpContext.Session.SetString("JwtToken", authResponse.Token);

                    // Create claims for MVC authentication
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, authResponse.Email ?? model.Email),
                        new Claim(ClaimTypes.Email, authResponse.Email ?? model.Email),
                        new Claim("Token", authResponse.Token)
                    };

                    // Add role claims - use single Role string from API
                    if (!string.IsNullOrEmpty(authResponse.Role))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, authResponse.Role));
                    }

                    // Add organization ID claim if user belongs to an organization
                    if (authResponse.OrganizationId.HasValue)
                    {
                        claims.Add(new Claim("OrganizationId", authResponse.OrganizationId.Value.ToString()));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    return Redirect(returnUrl ?? "/");
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during login.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                    return View(model);
                }

                var registerRequest = new RegisterRequest
                {
                    Email = model.Email,
                    Password = model.Password,
                    OrganizationId = model.OrganizationId
                };

                var success = await _accountService.RegisterAsync(registerRequest);

                if (success)
                {
                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Registration failed. Please try again.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during registration.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public int? OrganizationId { get; set; }
    }
}
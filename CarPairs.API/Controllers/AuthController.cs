using CarPairs.Core;
using CarPairs.Core.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarPairs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IOrganizationService _organizationService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IOrganizationService organizationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _organizationService = organizationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify organization exists
            if (model.OrganizationId.HasValue)
            {
                var org = await _organizationService.GetByIdAsync(model.OrganizationId.Value);
                if (org == null)
                    return BadRequest("Organization not found");
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                OrganizationId = model.OrganizationId,
                Role = model.Role ?? UserRole.User
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign role
                var roleStr = user.Role.ToString();
                await _userManager.AddToRoleAsync(user, roleStr);
                
                return Ok(new
                {
                    Message = "User registered successfully",
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    OrganizationId = user.OrganizationId
                });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            
            return Ok(new
            {
                Token = token,
                Email = user.Email,
                Role = user.Role.ToString(),
                OrganizationId = user.OrganizationId
            });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("UserRole", user.Role.ToString())
            };

            // Add organization ID claim if user belongs to an organization
            if (user.OrganizationId.HasValue)
            {
                claims.Add(new Claim("OrganizationId", user.OrganizationId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int? OrganizationId { get; set; }
        public UserRole? Role { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
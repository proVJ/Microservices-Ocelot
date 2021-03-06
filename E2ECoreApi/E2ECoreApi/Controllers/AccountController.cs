using E2E.Core.Data.DataContext;
using E2E.Core.Model;
using E2E.Core.Model.Request;
using E2E.Core.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E2E.Core.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {

    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IConfiguration _configuration;
    private IOptions<Audience> _settings;

    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="roleManager"></param>
    /// <param name="configuration"></param>
    public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IOptions<Audience> settings)
    {
      this.userManager = userManager;
      this.roleManager = roleManager;
      _configuration = configuration;
      this._settings = settings;

    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
      var user = await userManager.FindByNameAsync(model.Username);
      if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
      {
        var userRoles = await userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

        foreach (var userRole in userRoles)
        {
          authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.Secret));

        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
          issuer: _settings.Value.Iss,
          audience: _settings.Value.Aud,
          claims: authClaims,
          notBefore: now,
          expires: now.AddHours(3),

          signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
          );

        return Ok(new
        {
          token = new JwtSecurityTokenHandler().WriteToken(token),
          expiration = token.ValidTo
        });
      }
      return Unauthorized();
    }
    #region PreviousCode
    //public async Task<IActionResult> Login([FromBody] LoginModel model)
    //{
    //  var user = await userManager.FindByNameAsync(model.Username);
    //  if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
    //  {
    //    var userRoles = await userManager.GetRolesAsync(user);

    //    var authClaims = new List<Claim>
    //            {
    //                new Claim(ClaimTypes.Name, user.UserName),
    //                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //            };

    //    foreach (var userRole in userRoles)
    //    {
    //      authClaims.Add(new Claim(ClaimTypes.Role, userRole));
    //    }

    //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

    //    var token = new JwtSecurityToken(
    //        issuer: _configuration["JWT:ValidIssuer"],
    //        audience: _configuration["JWT:ValidAudience"],
    //        expires: DateTime.Now.AddHours(3),
    //        claims: authClaims,
    //        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
    //        );

    //    return Ok(new
    //    {
    //      token = new JwtSecurityTokenHandler().WriteToken(token),
    //      expiration = token.ValidTo
    //    });
    //  }
    //  return Unauthorized();
    //}
    #endregion


    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
      var userExists = await userManager.FindByNameAsync(model.Username);
      if (userExists != null)
        return StatusCode(StatusCodes.Status500InternalServerError, new Response<object> { Status = "Error", Message = "User already exists!" });

      ApplicationUser user = new ApplicationUser()
      {
        Email = model.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = model.Username
      };
      var result = await userManager.CreateAsync(user, model.Password);
      if (!result.Succeeded)
        return StatusCode(StatusCodes.Status500InternalServerError, new Response<object> { Status = "Error", Message = "User creation failed! Please check user details and try again." }); ;

      return Ok(new Response<object> { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
    {
      var userExists = await userManager.FindByNameAsync(model.Username);
      if (userExists != null)
        return StatusCode(StatusCodes.Status500InternalServerError, new Response<object> { Status = "Error", Message = "User already exists!" });

      ApplicationUser user = new ApplicationUser()
      {
        Email = model.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = model.Username
      };
      var result = await userManager.CreateAsync(user, model.Password);
      if (!result.Succeeded)
        return StatusCode(StatusCodes.Status500InternalServerError, new Response<object> { Status = "Error", Message = "User creation failed! Please check user details and try again." });

      if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
        await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
      if (!await roleManager.RoleExistsAsync(UserRoles.User))
        await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

      if (await roleManager.RoleExistsAsync(UserRoles.Admin))
      {
        await userManager.AddToRoleAsync(user, UserRoles.Admin);
      }

      return Ok(new Response<object> { Status = "Success", Message = "User created successfully!" });
    }

  }
}

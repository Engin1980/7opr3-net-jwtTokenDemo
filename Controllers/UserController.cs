using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTCoreDemo.Controllers
{
  [Route("api/user")]
  public class UserController : Controller
  {
    private IConfiguration configuration;

    public UserController([FromServices] IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    public IActionResult Index()
    {
      return RedirectToAction("unsecured");
    }

    [HttpGet]
    [Route("unsecured")]
    public IActionResult Unsecured()
    {
      return Ok("Unsecured ok");
    }

    [HttpGet]
    [Route("secured")]
    [Authorize]
    public IActionResult Secured()
    {
      return Ok("Secured ok");
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    public IActionResult Login(string email, string password)
    {
      IActionResult ret;

      if (password != "a")
        ret = BadRequest("Invalid email or password");
      else
      {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
          Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Aud, "any"),
                new Claim(JwtRegisteredClaimNames.Iss, "Marek Vajgl test server"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())            }),
          Expires = DateTime.UtcNow.AddSeconds(10),
          SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);
        ret = Ok(stringToken);
      }
      return ret;
    }
  }
}

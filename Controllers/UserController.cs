using JWTCoreDemo.Model;
using JWTCoreDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTCoreDemo.Controllers
{
  [Route("api/user")]
  [Authorize]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly AppUserService appUserService;
    private readonly SecurityService securityService;

    public UserController(
      [FromServices] AppUserService appUserService,
      [FromServices] SecurityService securityService)
    {
      this.appUserService = appUserService;
      this.securityService = securityService;
    }

    [HttpPut]
    [AllowAnonymous]
    public IActionResult Create(string email, string password, bool isAdmin)
    {
      try
      {
        this.appUserService.Create(email, password, isAdmin);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
      return Ok();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetUserNames()
    {
      List<string> ret = appUserService.GetUsers()
        .Select(q => q.Email[..q.Email.IndexOf("@")])
        .ToList();
      return Ok(ret);
    }

    [HttpGet("emails")]
    public IActionResult GetUserEmails()
    {
      List<string> ret = appUserService.GetUsers()
        .Select(q => q.Email)
        .ToList();
      return Ok(ret);
    }

    [HttpGet("all")]
    [Authorize(Roles = AppUser.ADMIN_ROLE_NAME)]
    public IActionResult GetAll()
    {
      List<AppUser> ret = appUserService.GetUsers();
      return Ok(ret);
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Login(string email, string password)
    {
      AppUser appUser;
      try
      {
        appUser = this.appUserService.GetUserByCredentials(email, password);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      string token = securityService.BuildJwtToken(appUser);
      return Ok(token);
    }
  }
}

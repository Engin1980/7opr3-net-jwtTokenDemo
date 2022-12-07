using JWTCoreDemo.Model;
using Microsoft.AspNetCore.Mvc;

namespace JWTCoreDemo.Services
{
  public class AppUserService
  {
    private readonly List<AppUser> inner = new();
    private readonly SecurityService securityService;

    public AppUserService([FromServices] SecurityService securityService)
    {
      this.securityService = securityService;
    }

    public AppUser Create(string email, string password, bool isAdmin)
    {
      EnsureNotNull(email, nameof(email));
      EnsureNotNull(password, nameof(password));

      email = email.ToLower();

      if (inner.Any(q => q.Email == email))
        throw CreateException($"Email {email} already exists.", null);

      string hash = this.securityService.HashPassword(password);

      AppUser ret = new(email, hash);
      if (isAdmin) ret.Roles.Add(AppUser.ADMIN_ROLE_NAME);
      this.inner.Add(ret);

      return ret;
    }

    public List<AppUser> GetUsers()
    {
      return this.inner.ToList();
    }

    public AppUser GetUserByCredentials(string email, string password)
    {
      EnsureNotNull(email, nameof(email));
      EnsureNotNull(password, nameof(password));

      AppUser appUser = inner.FirstOrDefault(q => q.Email == email.ToLower())
        ?? throw CreateException($"Email {email} does not exist.");

      if (!this.securityService.VerifyPassword(password, appUser.PasswordHash))
        throw CreateException($"Credentials are not valid.");

      return appUser;
    }

    private static void EnsureNotNull(string value, string parameterName)
    {
      if (value == null)
        throw CreateException($"Parameter {parameterName} cannot be null.");
    }

    private static ServiceException CreateException(string message, Exception? innerException = null) =>
      new(typeof(AppUserService), message, innerException);
  }
}

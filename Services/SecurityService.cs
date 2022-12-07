using JWTCoreDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.IO.IsolatedStorage;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTCoreDemo.Services
{
  public class SecurityService
  {
    private const int TOKEN_EXPIRATION_IN_SECONDS = 600;
    private readonly IConfiguration configuration;
    public byte[] Key { get; } = RandomNumberGenerator.GetBytes(128);

    public SecurityService([FromServices] IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    public string BuildJwtToken(AppUser appUser)
    {
      // key from configuration:
      //var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
      // ... or unique key per app start
      var key = this.Key;

      Dictionary<string, object> roleClaims = appUser.Roles
        .ToDictionary(
          q => ClaimTypes.Role,
          q => (object)q.ToUpper());

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[]
          {
        new Claim(JwtRegisteredClaimNames.Sub, appUser.Email),
        new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
        new Claim(JwtRegisteredClaimNames.Aud, configuration["Jwt:Audience"]),
        new Claim(JwtRegisteredClaimNames.Iss, configuration["Jwt:Issuer"]),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
      }),
        Expires = DateTime.UtcNow.AddSeconds(TOKEN_EXPIRATION_IN_SECONDS),
        SigningCredentials = new SigningCredentials
          (new SymmetricSecurityKey(key),
          SecurityAlgorithms.HmacSha512Signature),
        Claims = roleClaims
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);
      var ret = tokenHandler.WriteToken(token);
      return ret;
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
      bool ret;
      ret = BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
      return ret;
    }

    public string HashPassword(string password)
    {
      string ret = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
      return ret;
    }

    // not required here, but an example how to generate salt using
    // safe random number generator
    //internal string GenerateSalt()
    //{
    //  var bytes = RandomNumberGenerator.GetBytes(SALT_LENGTH);
    //  string ret = System.Convert.ToBase64String(bytes);
    //  return ret;
    //}
  }
}

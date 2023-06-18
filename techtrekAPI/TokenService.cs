using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using techtrekAPI.Helpers;

namespace techtrekAPI
{
    public class TokenService
    {


        private const int ExpirationMinutes = 30;
        private readonly IConfiguration _configuration;

        // DOCU: To access configuration field. which is from your appsettings.json, you need to pass it in the constructor (more like angular where you need to put the imports there to access its methods).
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateToken(ApplicationUser user)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var token = CreateJwtToken(
                CreateClaims(user),
                CreateSigningCredentials(),
                expiration
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        // DOCU: private "JwtSecurityToken" refers to the return type of what the method will return, arrow function => implicit return{}. 
        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
            DateTime expiration) =>
            new(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        private List<Claim> CreateClaims(ApplicationUser user)
        {
            
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "TokenForTheApiWithAuth"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    // DOCU: We added Role for Role-based authorization. This allows Authorize(Roles ="admin").
                    // take note that Roles is not from the ApplicationUser but from the Microsoft.AspNetCore.Authorization and it looks at the Role property in the JWT Token.
                    new Claim(ClaimTypes.Role, user.Role)

                };
                return claims;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // DOCU: SymmetricSecurityKey means whatever keyword used to encrypt is the same keyword for decryption.
        private SigningCredentials CreateSigningCredentials()
        {
            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JwtKey"])
                ),
                SecurityAlgorithms.HmacSha256
            );
        }

    }
}

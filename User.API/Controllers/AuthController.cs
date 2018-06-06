using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using User.API.Models;

namespace User.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private IOptions<JwtIssuerOptions> _settings;

        public AuthController(IOptions<JwtIssuerOptions> settings)
        {
            _settings = settings;
        }
        [HttpPost]
        public IActionResult Post([FromBody]UserViewModel user)
        {
            if (user.UserName == "admin" && user.Password == "admin123")
            {
                var responseJson = new
                {
                    token = GenerateEncodedToken(user),
                  
                };
                return Ok(responseJson);
            }
            else
            {
                return NotFound();
            }
        }
            #region Private Methods

            private string GenerateEncodedToken(UserViewModel user)
        {
            var now = DateTime.UtcNow;

            var claims = new Claim[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64),

            };
         
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Value.Secret));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = _settings.Value.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Value.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,

            };

            var jwt = new JwtSecurityToken(
                issuer: _settings.Value.Issuer,
                audience: _settings.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromDays(30)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)

            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }
        #endregion
    }
}
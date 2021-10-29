using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using DevExpress.ExpressApp.Blazor.Utils;
using DevExpress.ExpressApp.Security;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using ComplexPortfolio.Module.BusinessObjects;

namespace ComplexPortfolio.WebApi.JWT {
    public class JwtTokenService {
        readonly IObjectConverter objectConverter;
        readonly IConfiguration configuration;
        public JwtTokenService(IObjectConverter objectConverter, IConfiguration configuration) {
            this.objectConverter = objectConverter;
            this.configuration = configuration;
        }
        public string CreateToken(ApplicationUser user, AuthenticationStandardLogonParameters logonParameters) {
            IList<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Oid.ToString()));
            // You can save logonParameters for further use.
            claims.Add(new Claim("LogonParams", objectConverter.Pack(logonParameters)));

            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:Jwt:IssuerSigningKey"]));
            var token = new JwtSecurityToken(
                issuer: configuration["Authentication:Jwt:Issuer"],
                audience: configuration["Authentication:Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

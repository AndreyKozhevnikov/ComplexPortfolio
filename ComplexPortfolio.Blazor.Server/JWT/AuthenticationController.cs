using System;
using ComplexPortfolio.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ComplexPortfolio.WebApi.JWT {
    [ApiController]
    [Route("api/[controller]")]
    // This is a JWT authentication service sample.
    public class AuthenticationController : ControllerBase {
        private readonly ISecurityAuthenticationService securityAuthenticationService;
        private readonly JwtTokenService jwtTokenService;
        public AuthenticationController(ISecurityAuthenticationService securityAuthenticationService, JwtTokenService jwtTokenService) {
            this.securityAuthenticationService = securityAuthenticationService;
            this.jwtTokenService = jwtTokenService;
        }
        [HttpPost("Authenticate")]
        public IActionResult Authenticate(
            [FromBody]
            [SwaggerRequestBody(@"For example: <br /> { ""userName"": ""Admin"", ""password"": """" }")]
            AuthenticationStandardLogonParameters logonParameters
        ) {
            ApplicationUser user;
            try {
                user = (ApplicationUser)securityAuthenticationService.Authenticate(logonParameters);
            }
            catch(Exception ex) {
                if(ex is IUserFriendlyException) {
                    return Unauthorized(ex.Message);
                }
                else {
                    return Unauthorized();
                }
            }
            return Ok(jwtTokenService.CreateToken(user, logonParameters));
        }
    }
}

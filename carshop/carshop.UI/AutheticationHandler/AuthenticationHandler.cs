using carshop.Infrastructure.DataAccess;
using casrshop.Core.IServices;
using casrshop.Core.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace carshop.carshop.UI.AutheticationHandler
{
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAdminRepository _adminRepository;
        public AuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAdminRepository adminRepository) : base(options, logger, encoder, clock)
        {
            _adminRepository=adminRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                string[] credentials = Encoding.UTF8.GetString(bytes).Split(":");
                string login = credentials[0];
                string password = credentials[1];
                var admin = await _adminRepository.CheckAdminAsync(login, password);
                if (!admin)
                {
                    return AuthenticateResult.Fail("Invalid login or/and password");
                }

                
                var claims = new[] { new Claim(ClaimTypes.Name, login) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var pass = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(pass);
                
            }
            catch(Exception ex)
            {
                return AuthenticateResult.Fail($"Authorization failed {ex.Message}");
            }
        }
    }
}

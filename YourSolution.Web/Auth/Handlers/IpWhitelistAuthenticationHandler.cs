using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using YourSolution.Domain.Enums;
using YourSolution.Domain.Repositories;

namespace YourSolution.Web.Auth.Handlers
{
    /// <summary>
    /// IP白名單驗證方案邏輯
    /// </summary>
    public class IpWhitelistAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IpWhitelistAuthenticationHandler> _logger;

        public IpWhitelistAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IServiceProvider serviceProvider) : base(options, logger, encoder, clock)
        {
            _serviceProvider = serviceProvider;
            _logger = logger.CreateLogger<IpWhitelistAuthenticationHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var remoteIp = Context.Request.Headers.TryGetValue("X-Forwarded-For", out var ips)
                    ? ips.FirstOrDefault()?.ToString()
                    : Context.Connection.RemoteIpAddress?.ToString();

                remoteIp = remoteIp switch
                {
                    "::1" => "127.0.0.1",
                    _ => remoteIp
                };

                if (string.IsNullOrEmpty(remoteIp))
                {
                    // 不認證，讓其他認證方案繼續處理
                    return AuthenticateResult.NoResult();
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IpWhitelistRepository>();
                    var whiteLists = await repository.GetAllWhitelistsAsync();

                    if (whiteLists.Any(entry => entry.IsIpInSubnet(remoteIp)))
                    {
                        // 建立一個已認證的 ClaimsPrincipal，並賦予角色
                        var claims = new[]
                        {
                    new Claim(ClaimTypes.Name, "IpWhitelistUser"),
                    new Claim(ClaimTypes.Role, nameof(UserRole.IP白名單使用者)) // 依你的角色名稱調整
                };
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);

                        return AuthenticateResult.Success(ticket);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            // IP 不在白名單，不認證，讓其他認證方案繼續處理
            return AuthenticateResult.NoResult();
        }
    }
}

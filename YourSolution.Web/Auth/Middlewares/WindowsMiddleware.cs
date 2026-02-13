using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authentication;
using YourSolution.Domain.Services;
using YourSolution.Web.Auth.Services;
using YourSolution.Web.Auth.Schemes;

namespace YourSolution.Web.Auth.Middlewares
{
    /// <summary>
    /// Windows認證通過後，需透過cookie認證進行登入，這樣才能使用 Authorize(Role) 角色授權功能
    /// </summary>
    public class WindowsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<WindowsMiddleware> _logger;

        public WindowsMiddleware(RequestDelegate next, ILogger<WindowsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, UserAccountService accountService, ClaimsService claimeService, CookieAndWindowsAuthService cookieAndWindowsAuthService, IAuthenticationSchemeProvider schemes)
        {
            try
            {
                var allSchemes = await schemes.GetAllSchemesAsync();
                foreach (var scheme in allSchemes)
                {
                    _logger.LogInformation($"註冊的認證方案: {scheme.Name}");
                }

                // 使用多重認證方案
                var authResult = await httpContext.AuthenticateAsync(WindowsOrCookieScheme.CookiesOrNegotiate);

                if (authResult.Succeeded && authResult.Principal != null)
                {
                    var usedScheme = authResult.Ticket.AuthenticationScheme;

                    // 檢查是否採用 Windows 認證方式，是的話需設定 Cookie 認證
                    if (usedScheme == NegotiateDefaults.AuthenticationScheme || usedScheme == WindowsOrCookieScheme.IIS_WindowsSchemeName)
                    {
                        // 檢查是否已經有 Cookie 認證，避免重複簽入
                        var cookieAuth = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        if (!cookieAuth.Succeeded)
                        {
                            #region (自訂邏輯)windows認證通過後，需透過windows帳戶名稱取得Claims
                            var domainAccount = cookieAndWindowsAuthService.WindowsLoginGetDomainAccount(httpContext);
                            var domainUserName = cookieAndWindowsAuthService.WindowsLoginGetDomainUserName(httpContext);
                            var userAccount = await accountService.GetUserAccountByADAsync(domainAccount, domainUserName);
                            var claims = claimeService.CreateLoginClaims(userAccount);
                            await accountService.UpdateLastLoginTimeAsync(userAccount);
                            #endregion

                            //用 Cookie 認證方式登入，並設定Claims
                            await cookieAndWindowsAuthService.WindowsLoginSetClaims(httpContext, claims);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(WindowsMiddleware)} encountered an error.");
            }

            await _next(httpContext);
        }
    }

    public static class WindowsMiddlewareExtensions
    {
        /// <summary>
        /// Windows認證通過後，需透過cookie認證進行登入，這樣才能使用 Authorize(Role) 角色授權功能
        /// 這段要放在 app.UseAuthentication() 和 app.UseAuthorization() 之間
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWindowsMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WindowsMiddleware>();
        }
    }
}

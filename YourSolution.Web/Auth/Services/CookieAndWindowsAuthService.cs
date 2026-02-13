using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Negotiate;
using System.DirectoryServices.AccountManagement;

namespace YourSolution.Web.Auth.Services
{
    /// <summary>
    /// Cookie & windows 驗證的登入登出
    /// </summary>
    public class CookieAndWindowsAuthService
    {
        /// <summary>
        /// cookie驗證登入
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task CookieLoginAsync(HttpContext HttpContext, List<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //取得或設定驗證票證的簽發時間。
                //IssuedUtc = DateTime.Now.ToLocalTime(),

                // 是否允許 Cookie 被刷新（延長有效期），如果設為true，則CookieAuthenticationOptions.SlidingExpiration也要設為true
                AllowRefresh = true,

                // 是否持久化 Cookie，設 true 讓使用者關閉瀏覽器後仍保持登入
                IsPersistent = true,

                //Cookie 的過期時間，IsPersistent = true的狀況下才需要設定，可覆蓋CookieAuthenticationOptions.ExpireTimeSpan的設定
                //ExpiresUtc = DateTime.Now.ToLocalTime().AddSeconds(5),

                // 登入後要導向的頁面，可視需求設定
                //RedirectUri = <string>
            };

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

            // 立即更新當下請求的使用者物件，讓後續程式碼能使用新的 Claims
            HttpContext.User = claimsPrincipal;
        }

        /// <summary>
        /// cookie驗證登出
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <returns></returns>
        public async Task CookieLogoutAsync(HttpContext HttpContext)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //HttpContext.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(string.Empty), null);
        }

        /// <summary>
        /// windows驗證登入，回傳OK(200)代表通過Windows認證
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <param name="GetClaimsAsync"></param>
        /// <returns>Http狀態碼</returns>
        public async Task<HttpStatusCode> WindowsLoginAsync(HttpContext HttpContext)
        {
            // 1. 登出 Cookie 認證方案
            await CookieLogoutAsync(HttpContext);

            var user = HttpContext.User;

            // 2. 尚未認證，觸發 Windows 認證挑戰並回傳 401
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                //加上WWW-Authenticate: Negotiate 標頭
                await HttpContext.ChallengeAsync(NegotiateDefaults.AuthenticationScheme);
                return HttpStatusCode.Unauthorized;
            }

            // 3. 帳號格式不符，回傳 403 Forbidden
            if (!(user.Identity.Name?.Contains('\\') ?? true))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return HttpStatusCode.Forbidden;
            }

            return HttpStatusCode.OK;
        }

        /// <summary>
        /// 取得Windows驗證通過後的帳戶名稱
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <returns></returns>
        public string WindowsLoginGetDomainAccount(HttpContext HttpContext)
        {
            var user = HttpContext.User;

            if (user?.Identity != null && user.Identity.IsAuthenticated && (user.Identity.Name?.Contains('\\') ?? true))
                return user.Identity.Name.Split('\\').Last();

            return string.Empty;
        }

        /// <summary>
        /// Windows 驗證後的使用者帳號名稱
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public string WindowsLoginGetDomainUserName(HttpContext httpContext)
        {
            var user = httpContext.User;

            if (user?.Identity != null && user.Identity.IsAuthenticated && (user.Identity.Name?.Contains('\\') ?? false))
            {
                var parts = user.Identity.Name.Split('\\');
                if (parts.Length == 2)
                {
                    string domain = parts[0];
                    string username = parts[1];

                    // 取得使用者顯示名稱
                    string displayName = GetUserDisplayName(domain, username);
                    return displayName;
                }
            }

            return string.Empty;
        }


        /// <summary>
        /// Windows 驗證後的使用者帳號名稱（通常格式是 DOMAIN\username）
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        private string GetUserDisplayName(string domain, string username)
        {
            // 使用 PrincipalContext 連接到指定網域
            using (var context = new PrincipalContext(ContextType.Domain, domain))
            {
                // 找出指定使用者
                using (var user = UserPrincipal.FindByIdentity(context, username))
                {
                    if (user != null)
                    {
                        return user.DisplayName ?? string.Empty;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 將通過Windows驗證的使用者資訊，以Cookie驗證方案登入並設定Claims
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task WindowsLoginSetClaims(HttpContext HttpContext, List<Claim> claims)
        {
            await CookieLogoutAsync(HttpContext);
            await CookieLoginAsync(HttpContext, claims);
        }
    }
}

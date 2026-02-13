using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using YourSolution.Domain.Enums;
using YourSolution.Domain.Services;
using YourSolution.Web.Auth.Services;

namespace YourSolution.Web.Auth.Events
{
    /// <summary>
    /// ValidatePrincipal 事件本身只負責驗證 Cookie 是否有效，失效後會拒絕該 Cookie，但不會自動發送重定向（302）給客戶端。
    /// 判斷方式：當userAccount.IsLock=true，則視為無效並登出
    /// 權限被變更或使用者資料被更新，會即時更新Claims
    /// </summary>
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly UserAccountService _userAccountService;
        private readonly CookieAndWindowsAuthService _cookieAndWindowsAuthService;

        public CustomCookieAuthenticationEvents(UserAccountService userAccountService, CookieAndWindowsAuthService cookieAndWindowsAuthService)
        {
            _userAccountService = userAccountService;
            _cookieAndWindowsAuthService = cookieAndWindowsAuthService;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            try
            {
                var claims = userPrincipal?.Claims;

                var flowIdStr = (from c in claims
                                 where c.Type == ClaimTypes.NameIdentifier
                                 select c.Value).FirstOrDefault();

                if (Guid.TryParse(flowIdStr, out Guid flowid) == false)
                {
                    context.RejectPrincipal();

                    await _cookieAndWindowsAuthService.CookieLogoutAsync(context.HttpContext);
                }

                var userAccount = await _userAccountService.GetUserAccountByFlowIdAsync(flowid);

                //當userAccount.IsLock=true，則視為無效並登出
                if (userAccount.IsLock)
                {
                    context.RejectPrincipal();

                    await _cookieAndWindowsAuthService.CookieLogoutAsync(context.HttpContext);
                }

                var roles = (from c in claims
                             where c.Type == ClaimTypes.Role
                             select c).FirstOrDefault();

                var givenName = (from c in claims
                                 where c.Type == ClaimTypes.GivenName
                                 select c).FirstOrDefault();

                //當使用者權限變更或使用者名稱被更新時，會將現有已登入的使用者資料(Claim)更新權限
                if (Enum.GetName(typeof(UserRole), userAccount.Role) != roles?.Value || userAccount.Name != givenName?.Value)
                {
                    List<Claim> newClaims = new List<Claim>();

                    foreach (var claim in claims)
                    {
                        if (claim.Type == ClaimTypes.Role)
                        {
                            var roleName = Enum.GetName(typeof(UserRole), userAccount.Role) ?? string.Empty;
                            newClaims.Add(new Claim(ClaimTypes.Role, roleName));
                        }
                        else if (claim.Type == ClaimTypes.GivenName)
                        {
                            newClaims.Add(new Claim(ClaimTypes.GivenName, userAccount.Name));
                        }
                        else
                            newClaims.Add(claim);
                    }

                    var newClaimsIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var newPrincipal = new ClaimsPrincipal(newClaimsIdentity);
                    context.ReplacePrincipal(newPrincipal);
                    await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, newPrincipal);
                }
            }
            catch (Exception ex)
            {
                context.RejectPrincipal();

                await _cookieAndWindowsAuthService.CookieLogoutAsync(context.HttpContext);
            }
        }
    }
}

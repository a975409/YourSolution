using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authentication;
using YourSolution.Domain.Enums;
using YourSolution.Web.Auth.Events;
using YourSolution.Web.Auth.Handlers;
using YourSolution.Web.Auth.Services;

namespace YourSolution.Web.Auth.Schemes
{
    /// <summary>
    /// cookie、Windows和IP白名單混合驗證方案
    /// </summary>
    public static class WindowsOrCookieScheme
    {
        /// <summary>
        /// cookie認證的cookie key，顯示為.AspNetCore.Cookies
        /// </summary>
        public static string CookieAuthKey = CookieAuthenticationDefaults.CookiePrefix + CookieAuthenticationDefaults.AuthenticationScheme;

        /// <summary>
        /// IP白名單驗證方案
        /// </summary>
        private static string IpWhitelistSchemeName = "IpWhitelistScheme";

        /// <summary>
        /// IIS 的 Windows驗證
        /// </summary>
        public static string IIS_WindowsSchemeName = "Windows";

        /// <summary>
        /// 混合認證方案名稱
        /// </summary>
        public static string CookiesOrNegotiate = "WindowsOrCookie";


        /// <summary>
        /// 新增Cookie、Windows、ip白名單多種驗證方案
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWindowsOrCookieScheme(this IServiceCollection services)
        {
            services.AddScoped<UserIdentityService>();
            services.AddScoped<CookieAndWindowsAuthService>();
            services.AddScoped<CustomCookieAuthenticationEvents>();
            services.AddScoped<ClaimsService>();
            services.AddHttpContextAccessor();

            //設定預設要採用的驗證方案
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookiesOrNegotiate;
                options.DefaultChallengeScheme = CookiesOrNegotiate;
            })

            //ip白名單驗證方案
            .AddScheme<AuthenticationSchemeOptions, IpWhitelistAuthenticationHandler>(IpWhitelistSchemeName, options => { })
            
            //cookie驗證方案
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Cookie 驗證登入頁面
                options.LogoutPath = "/Account/Login"; // Cookie 驗證登入頁面
                options.AccessDeniedPath = "/Account/Login";
                options.EventsType = typeof(CustomCookieAuthenticationEvents);

                //是否啟用滑動過期自動延長期限
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                //是否啟用滑動過期（使用者活動時自動延長過期時間），如果設為true，則AuthenticationProperties.AllowRefresh也要設為true
                options.SlidingExpiration = true;
            })
            
            //Windows驗證方案
            .AddNegotiate()
            .AddPolicyScheme(CookiesOrNegotiate, CookiesOrNegotiate, options =>
            {
                //根據 Http Request 決定採用哪種驗證方案
                options.ForwardDefaultSelector = context =>
                {
                    if (context.Request.Headers.TryGetValue("IpWhitelistIsActive", out var IpWhitelistIsActive))
                    {
                        return IpWhitelistSchemeName;//IP白名單驗證方案
                    }

                    if (context.Request.Cookies.ContainsKey(CookieAuthKey))
                    {
                        return CookieAuthenticationDefaults.AuthenticationScheme; //cookie驗證方案
                    }

                    return NegotiateDefaults.AuthenticationScheme;//windows驗證方案
                };
            });

            services.AddAuthorization(options =>
            {
                //自訂預設的授權策略
                options.AddPolicy("RolePolicy", policy =>
                {
                    policy.RequireRole(nameof(UserRole.一般使用者), nameof(UserRole.系統管理者));
                });

                //ip白名單授權策略
                options.AddPolicy("IpWhitelistPolicy", policy =>
                {
                    policy.RequireRole(nameof(UserRole.IP白名單使用者)); // 依角色調整
                });

                //預設的授權策略
                options.FallbackPolicy = options.GetPolicy("RolePolicy");
                //options.FallbackPolicy = options.DefaultPolicy;
            });

            return services;
        }
    }
}

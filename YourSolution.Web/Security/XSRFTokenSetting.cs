namespace YourSolution.Web.Security
{
    public static class XSRFTokenSetting
    {
        /// <summary>
        /// 針對API設定 XSRF or CSRF 風險防範
        /// 需在頁面(*.cshtml)上加上以下2段程式碼
        /// (C#) var requestToken = Antiforgery.GetAndStoreTokens(Context).RequestToken;
        /// (HTML) <input id = "RequestVerificationToken" type="hidden" value="@requestToken" />
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection APISettingXSRF(this IServiceCollection services)
        {
            //設定request header存放的token名稱
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            return services;
        }
    }
}

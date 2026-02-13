namespace YourSolution.Web.Auth.Middlewares
{
    /// <summary>
    /// 當 CustomCookieAuthenticationEvents 判斷cookie無效，則需要跳轉至登入畫面
    /// 放在app.UseAuthorization()下方
    /// </summary>
    public class RedirectToLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectToLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 不攔截登入頁，避免無限重導
            var path = context.Request.Path.Value ?? string.Empty;

            bool pathIgnore = false;
            string[] ignorePath = { "/css", "/lib", "/js", "/img", "/modules", "/vuejs" };

            foreach (var ignorePathItem in ignorePath)
            {
                if (path.StartsWith(ignorePathItem, StringComparison.OrdinalIgnoreCase))
                {
                    pathIgnore = true;
                    break;
                }
            }

            var controller = context.Request.RouteValues["controller"]?.ToString();
            var action = context.Request.RouteValues["action"]?.ToString();

            if (pathIgnore || controller == "Account" || controller == "AccountApi")
                pathIgnore = true;

            if (pathIgnore == false)
            {
                if (context.User?.Identity == null || context.User.Identity.IsAuthenticated == false)
                {
                    context.Response.Redirect("/Account/Login?IsLock=true");
                    return;
                }
            }

            await _next(context);
        }
    }
}

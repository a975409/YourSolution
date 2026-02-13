using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourSolution.Web.Auth.Services;

namespace YourSolution.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly CookieAndWindowsAuthService _cookieLogInAndOut;

        public AccountController(CookieAndWindowsAuthService cookieLogInAndOut)
        {
            _cookieLogInAndOut = cookieLogInAndOut;
        }

        /// <summary>
        /// 使用者登入頁
        /// </summary>
        /// <param name="IsLock"></param>
        /// <returns></returns>
        public IActionResult Login(bool IsLock = false)
        {
            ViewBag.IsLock = IsLock;
            return View();
        }

        /// <summary>
        /// 使用者登出後，跳轉至登入頁
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await _cookieLogInAndOut.CookieLogoutAsync(HttpContext);
            return RedirectToAction("Login");
        }
    }
}

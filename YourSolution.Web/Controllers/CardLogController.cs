using Microsoft.AspNetCore.Mvc;

namespace YourSolution.Web.Controllers
{
    public class CardLogController : Controller
    {
        /// <summary>
        /// 查詢頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}

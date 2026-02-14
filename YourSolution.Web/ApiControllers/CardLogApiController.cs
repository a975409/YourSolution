using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourSolution.Infrastructure.DTOs.Request;
using YourSolution.Infrastructure.DTOs.Response;

namespace YourSolution.Web.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardLogApiController : ControllerBase
    {

        /// <summary>
        /// 分頁查詢
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("GetCardLog")]
        public IActionResult GetCardLogAsync(SearchPageDto dto)
        {
            try
            {
                var result = new SearchPageResultDto<object>();

                return Ok(new ResultDto
                {
                    IsOK = true,
                    Message = "查詢成功",
                    Data = result,

                });
            }
            catch (Exception ex)
            {
                return Ok(new ResultDto
                {
                    IsOK = false,
                    Message = "查詢失敗：" + ex.Message,
                });
            }
        }
    }
}

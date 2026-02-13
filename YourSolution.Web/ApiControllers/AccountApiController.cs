using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using YourSolution.Domain.DTOs.Request;
using YourSolution.Domain.Services;
using YourSolution.Infrastructure.DTOs.Response;
using YourSolution.Web.Auth.Services;

namespace YourSolution.Web.ApiControllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly UserAccountService _accountService;
        private readonly CookieAndWindowsAuthService _cookieAndWindowsAuthService;
        private readonly ClaimsService _claimsService;
        private readonly ILogger _logger;

        public AccountApiController(UserAccountService accountService, CookieAndWindowsAuthService cookieAndWindowsAuthService, ClaimsService claimsService, ILogger<AccountApiController> logger)
        {
            _accountService = accountService;
            _cookieAndWindowsAuthService = cookieAndWindowsAuthService;
            _claimsService = claimsService;
            _logger = logger;
        }

        /// <summary>
        /// 使用者Cookie登入
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        //[ValidateAntiForgeryToken]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginRequestDto dto)
        {
            try
            {
                //檢查是否有符合的使用者資料
                var userAccount = await _accountService.GetLoginSuccessUserAccountAsync(dto.Account, dto.Pwd);

                var claims = _claimsService.CreateLoginClaims(userAccount);
                await _accountService.UpdateLastLoginTimeAsync(userAccount);
                await _cookieAndWindowsAuthService.CookieLoginAsync(HttpContext, claims);

                return Ok(new ResultDto
                {
                    IsOK = true,
                    Message = "登入成功",
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResultDto
                {
                    IsOK = false,
                    Message = $"登入失敗，{ex.Message}",
                });
            }
        }

        /// <summary>
        /// 使用者網域登入
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
        [HttpGet("WindowsLogin")]
        public async Task<IActionResult> WindowsLoginAsync()
        {
            try
            {
                var httpcode = await _cookieAndWindowsAuthService.WindowsLoginAsync(HttpContext);

                if (httpcode == HttpStatusCode.Unauthorized)
                {
                    return Unauthorized(new { message = "Windows authentication required." });
                }
                else if (httpcode == HttpStatusCode.Forbidden)
                {
                    return Forbid();
                }
                else
                {
                    return Ok(new ResultDto
                    {
                        IsOK = true,
                        Message = "登入成功",
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResultDto
                {
                    IsOK = false,
                    Message = $"登入失敗，{ex.Message}",
                });
            }
        }
    }
}

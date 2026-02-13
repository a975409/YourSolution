using System.Security.Claims;
using YourSolution.Domain.Enums;
using YourSolution.Web.Auth.DTOs;

namespace YourSolution.Web.Auth.Services
{
    /// <summary>
    /// 從 Claims 取得使用者資料
    /// </summary>
    public class UserIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 取得目前登入中的使用者資料
        /// </summary>
        /// <returns></returns>
        public UserInfo? GetCurrentLoginUserInfo()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            // 安全判斷 Identity 是否為 null，並檢查是否已驗證
            if (user?.Identity != null && user.Identity.IsAuthenticated)
            {
                //取得使用者資訊
                var claims = user.Claims;

                var userInfo = new UserInfo()
                {
                    Account = user.Identity.Name ?? string.Empty,
                    UserName = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value ?? string.Empty,
                };

                if (Enum.TryParse(claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value, out UserRole role))
                {
                    userInfo.Role = role;
                }

                if (Guid.TryParse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out Guid flowid))
                {
                    userInfo.FlowId = flowid;
                }

                return userInfo;
            }
            else
            {
                return null;
            }
        }
    }
}

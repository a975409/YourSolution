using System.Security.Claims;
using YourSolution.Domain.Enums;
using YourSolution.Domain.Models;

namespace YourSolution.Web.Auth.Services
{
    public class ClaimsService
    {
        /// <summary>
        /// 產生登入用的claims
        /// </summary>
        /// <param name="userAccount">使用者資料</param>
        /// <returns></returns>
        public List<Claim> CreateLoginClaims(UserAccount userAccount)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userAccount.Account),
                new Claim(ClaimTypes.GivenName, userAccount.Name),
                new Claim(ClaimTypes.NameIdentifier,userAccount.FlowId.ToString())
            };

            if (string.IsNullOrEmpty(Enum.GetName(typeof(UserRole), userAccount.Role)) == false)
            {
                var roleName = Enum.GetName(typeof(UserRole), userAccount.Role) ?? string.Empty;
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }
            else
                claims.Add(new Claim(ClaimTypes.Role, nameof(UserRole.一般使用者)));

            return claims;
        }
    }
}

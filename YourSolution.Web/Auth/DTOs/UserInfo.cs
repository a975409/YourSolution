using YourSolution.Domain.Enums;

namespace YourSolution.Web.Auth.DTOs
{
    public class UserInfo
    {
        /// <summary>
        /// 帳號(AD帳號)
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 所屬權限
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// 唯一碼(PK)
        /// </summary>
        public Guid FlowId { get; set; }
    }
}

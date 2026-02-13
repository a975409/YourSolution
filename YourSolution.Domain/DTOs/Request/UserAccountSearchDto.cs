using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Infrastructure.DTOs.Request;

namespace YourSolution.Domain.DTOs.Request
{
    /// <summary>
    /// 管理者查詢使用者資料
    /// </summary>
    public class UserAccountSearchDto : SearchPageDto
    {
        /// <summary>
        /// 帳號(AD帳號)
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// (帳號狀態)該帳戶是否被鎖定
        /// </summary>
        public int AccountStatus { get; set; }

        /// <summary>
        /// 使用者姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色權限
        /// </summary>
        public int Role { get; set; }
    }
}

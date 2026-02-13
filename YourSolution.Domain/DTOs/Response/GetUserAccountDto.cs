using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.Enums;
using YourSolution.Infrastructure.Enums;

namespace YourSolution.Domain.DTOs.Response
{
    public class GetUserAccountDto
    {
        public Guid FlowId { get; set; }

        /// <summary>
        /// 帳號(AD帳號)
        /// </summary>
        [DisplayName("帳號(AD帳號)")]
        public string Account { get; set; }

        /// <summary>
        /// 帳號狀態
        /// </summary>
        [DisplayName("帳號狀態")]
        public string AccountStatus { get; set; }

        /// <summary>
        /// 帳戶鎖定原因
        /// </summary>
        public string AccountIsLockMsg { get; set; }

        /// <summary>
        /// 使用者姓名
        /// </summary>
        [DisplayName("使用者姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 角色權限
        /// </summary>
        public UserRole Role { get; set; }

        [DisplayName("角色權限")]
        public string RoleText { get; set; }

        /// <summary>
        /// 該帳戶是否被鎖定
        /// </summary>
        public bool IsLock { get; set; }

        /// <summary>
        /// 新增資料的時間
        /// </summary>
        [DisplayName("新增資料的時間")]
        public string CreateTime { get; set; }

        /// <summary>
        /// 更新資料的時間
        /// </summary>
        [DisplayName("更新資料的時間")]
        public string UpdateTime { get; set; }

        /// <summary>
        /// 上次登入的時間
        /// </summary>
        [DisplayName("上次登入的時間")]
        public string LastLoginTime { get; set; }

        public byte[] RowVersion { get; set; }
    }
}

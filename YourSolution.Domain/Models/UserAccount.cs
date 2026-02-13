using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.Enums;
using YourSolution.Infrastructure.Enums;

namespace YourSolution.Domain.Models
{
    /// <summary>
    /// 使用者帳戶
    /// </summary>
    [Index(nameof(FlowId))]
    [Index(nameof(Account))]
    public class UserAccount
    {
        public Guid FlowId { get; set; }

        public decimal Id { get; set; }

        /// <summary>
        /// 帳號(AD帳號)
        /// </summary>
        [Required, MaxLength(30)]
        public string Account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required, MaxLength(200)]
        public string Pwd { get; set; }

        /// <summary>
        /// 使用者姓名
        /// </summary>
        [Required, MaxLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 登入失敗的錯誤次數，達到指定次數會鎖定，解除鎖定後會歸0重新計算
        /// </summary>
        public byte LoginErrorCount { get; set; }

        /// <summary>
        /// 該帳戶是否被鎖定
        /// </summary>
        public bool IsLock { get; set; }

        /// <summary>
        /// 變更鎖定狀態的原因
        /// </summary>
        public string ChangeLockStatusMsg { get; set; }

        /// <summary>
        /// 上次變更鎖定狀態的使用者ID
        /// </summary>
        public Guid? ChangeLockStatusUserFlowId { get; set; }

        /// <summary>
        /// 上次變更鎖定狀態的時間
        /// </summary>
        public DateTime? ChangeLockStatusTime { get; set; }

        /// <summary>
        /// 角色權限
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// 新增資料的時間
        /// </summary>
        [Required]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新資料的時間
        /// </summary>
        [Required]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 上次登入的時間
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        [Timestamp] // 標示為 rowversion 欄位，用於併發控制 
        public byte[] RowVersion { get; set; }
    }
}

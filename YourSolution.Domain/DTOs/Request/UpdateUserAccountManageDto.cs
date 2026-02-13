using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.Enums;

namespace YourSolution.Domain.DTOs.Request
{
    /// <summary>
    /// 管理者更新其他使用者資料
    /// </summary>
    public class UpdateUserAccountManageDto
    {
        public Guid FlowId { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [MaxLength(200, ErrorMessage = "帳號不能超過200個字")]
        public string? Pwd { get; set; }

        /// <summary>
        /// 使用者姓名
        /// </summary>
        [Required(ErrorMessage = "姓名必填"), MaxLength(20, ErrorMessage = "姓名不能超過20個字")]
        public string Name { get; set; }

        /// <summary>
        /// 角色權限
        /// </summary>
        [Required(ErrorMessage = "必須設定角色權限"), Range((byte)UserRole.一般使用者, (byte)UserRole.系統管理者, ErrorMessage = "權限設定錯誤")]
        public byte Role { get; set; }

        public byte[] RowVersion { get; set; }
    }
}

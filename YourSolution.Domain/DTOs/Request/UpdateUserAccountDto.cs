using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Domain.DTOs.Request
{
    /// <summary>
    /// 使用者更新自己的資料
    /// </summary>
    public class UpdateUserAccountDto
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

        public byte[] RowVersion { get; set; }
    }
}

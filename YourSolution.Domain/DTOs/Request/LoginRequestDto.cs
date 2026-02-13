using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Domain.DTOs.Request
{
    public class LoginRequestDto
    {
        /// <summary>
        /// 帳號(AD帳號)
        /// </summary>
        [Required(ErrorMessage = "帳號必填"), MaxLength(30, ErrorMessage = "帳號最多30個字")]
        public string Account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required(ErrorMessage = "密碼必填"), MaxLength(20, ErrorMessage = "密碼最多20個字")]
        public string Pwd { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Domain.DTOs.Request
{
    public class SystemSettingDto
    {
        [Required(ErrorMessage = "需設定最大可登入失敗的次數"), Range(1, int.MaxValue, ErrorMessage = "最大可登入失敗的次數需設定大於0")]
        public int MaxLoginFailCount { get; set; }
    }
}

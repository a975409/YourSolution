using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Infrastructure.DTOs.Request
{
    /// <summary>
    /// 指定頁碼查詢設定
    /// </summary>
    public class SearchPageDto
    {
        /// <summary>
        /// 指定查詢頁碼
        /// </summary>
        public int Page { get; set; }
    }
}

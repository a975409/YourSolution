using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Infrastructure.DTOs.Response
{
    /// <summary>
    /// 分頁查詢結果
    /// </summary>
    /// <typeparam name="T">查詢結果的資料類型</typeparam>
    public class SearchPageResultDto<T>
        where T : class
    {
        /// <summary>
        /// 指定查詢頁碼
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 總資料筆數(查詢結果)
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 總頁數(查詢結果)
        /// </summary>
        public int TotalPage { get; set; }

        /// <summary>
        /// 顯示頁碼範圍
        /// </summary>
        public int[] pageRangeList { get; set; }

        /// <summary>
        /// 查詢結果
        /// </summary>
        public List<T> SearchResult { get; set; }
    }
}

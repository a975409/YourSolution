using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Infrastructure.Enums;

namespace YourSolution.Infrastructure.DTOs
{
    /// <summary>
    /// SQL分頁查詢設定
    /// </summary>
    public class PaginationQueryDto
    {
        /// <summary>
        /// 連線字串
        /// </summary>
        public SqlConnection SqlConnection { get; set; }

        /// <summary>
        /// SQL語法(select...where ....)
        /// </summary>
        public string BaseQuery { get; set; }

        /// <summary>
        /// 指定頁數
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 單頁最大顯示的資料筆數
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 如何排序
        /// </summary>
        public OrderOption OrderOption { get; set; } = OrderOption.ASC;

        /// <summary>
        /// 指定哪個欄位做排序
        /// </summary>
        public string OrderField { get; set; } = "id";

        /// <summary>
        /// 查詢條件
        /// </summary>
        public DynamicParameters? Parameters { get; set; } = null;
    }
}

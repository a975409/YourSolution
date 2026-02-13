using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Infrastructure.Extensions
{
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// 將DbConnection轉換成SqlConnection
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="connectionString">連線字串</param>
        /// <returns></returns>
        public static SqlConnection ConvertToSqlConnection(this DbConnection conn, string connectionString)
        {
            if (conn is SqlConnection sqlConn)
                return sqlConn;

            return new SqlConnection(connectionString);
        }
    }
}

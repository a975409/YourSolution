using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Infrastructure.DTOs;
using YourSolution.Infrastructure.Models;

namespace YourSolution.Infrastructure.Repositories
{
    public class RequestLogRepository
    {
        private readonly IMapper _iMapper;
        private string _connectionString;
        private readonly IConfiguration _configuration;

        public RequestLogRepository(IMapper iMapper, IConfiguration Configuration)
        {
            _configuration = Configuration;
            _connectionString = Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            _iMapper = iMapper;
        }

        /// <summary>
        /// 設定連線字串名稱
        /// </summary>
        /// <param name="connectionSection">連線字串名稱</param>
        public void SetConnectionSection(string connectionSection)
        {
            _connectionString = _configuration.GetConnectionString(connectionSection) ?? string.Empty;
        }

        /// <summary>
        /// 新增一筆Http Request紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> InsertRequestLogAsync(RequestLogDto dto)
        {
            var insertData = _iMapper.Map<RequestLog>(dto);

            if (insertData == null)
                return 0;

            insertData.Timestamp = DateTime.Now;

            string insertSql = @"INSERT INTO [dbo].[RequestLogs]
                                   ([Method]
                                   ,[Path]
                                   ,[QueryString]
                                   ,[Body]
                                   ,[Headers]
                                   ,[Timestamp]
                                   ,[ResponseStatusCode]
                                   ,[ResponseHeaders]
                                   ,[ResponseBody]
                                   ,[UserFlowId])
                             VALUES
                                   (@Method
                                   ,@Path
                                   ,@QueryString
                                   ,@Body
                                   ,@Headers
                                   ,@Timestamp
                                   ,@ResponseStatusCode
                                   ,@ResponseHeaders
                                   ,@ResponseBody
                                   ,@UserFlowId);
                                  SELECT @@IDENTITY;";

            var conn = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters(insertData);
            return await conn.QueryFirstOrDefaultAsync<int>(insertSql, parameters);
        }

        public async Task<int> RemoveExpiredLogByDayAsync(int days)
        {
            if (days <= 0)
                return 0;

            var expiredDate = DateTime.Now.AddDays(days * -1);

            var conn = new SqlConnection(_connectionString);
            const int batchSize = 1000;
            int totalDeleted = 0;
            int rowsAffected;

            do
            {
                // 分批刪除逾期資料，避免一次刪除大量資料造成鎖表
                //rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                //    "DELETE TOP (@p0) FROM RequestLogs WHERE Timestamp < @p1", batchSize, expiredDate);
                var parameters = new DynamicParameters();
                parameters.Add("@p0", batchSize);
                parameters.Add("@p1", expiredDate);

                rowsAffected = await conn.ExecuteAsync("DELETE TOP (@p0) FROM RequestLogs WHERE Timestamp < @p1", parameters);

                totalDeleted += rowsAffected;
            }
            while (rowsAffected > 0);

            return totalDeleted;
        }

        /// <summary>
        /// 刪除所有RequestLogs紀錄
        /// </summary>
        /// <returns></returns>
        public async Task RemoveAllLogAsync()
        {
            var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("TRUNCATE TABLE RequestLogs");
        }
    }
}

using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Infrastructure.DTOs.Response;
using YourSolution.Infrastructure.DTOs;
using YourSolution.Infrastructure.Enums;
using YourSolution.Infrastructure.Factories;

namespace YourSolution.Infrastructure.Repositories
{
    public class PaginationQueryRepository
    {
        private readonly SearchPageResultDtoFactory _searchPageResultDtoFactory;

        public PaginationQueryRepository(SearchPageResultDtoFactory searchPageResultDtoFactory)
        {
            _searchPageResultDtoFactory = searchPageResultDtoFactory;
        }

        /// <summary>
        /// 分頁查詢
        /// </summary>
        /// <typeparam name="T">分頁查詢結果的資料型態</typeparam>
        /// <param name="dto">查詢條件</param>
        /// <returns>分頁查詢結果的物件</returns>
        public async Task<SearchPageResultDto<T>> PaginationQueryAsync<T>(PaginationQueryDto dto)
            where T : class
        {
            if (dto.Parameters == null)
                dto.Parameters = new DynamicParameters();

            var conn = dto.SqlConnection;

            //取得資料筆數
            //var getTotalCountQuery = $"select count(1) from ({dto.BaseQuery}) as itemdata";

            var getTotalCountQuery = new StringBuilder();
            getTotalCountQuery.Append("select count(1) from (");
            getTotalCountQuery.Append(dto.BaseQuery);
            getTotalCountQuery.Append(") as itemdata");

            var totalCount = await conn.QueryFirstAsync<int>(getTotalCountQuery.ToString(), dto.Parameters);

            var result = _searchPageResultDtoFactory.GetSearchPageResultDto<T>(totalCount, dto.Page, dto.Limit);

            #region 依照指定頁數範圍查詢資料
            var query = @$"select * from ({dto.BaseQuery}) as itemdata 
                               order by {dto.OrderField} {Enum.GetName<OrderOption>(dto.OrderOption)} 
                               OFFSET @PageIndex ROWS 
                               FETCH NEXT @PageSize ROWS ONLY";

            dto.Parameters.Add("@PageIndex", (result.Page - 1) * dto.Limit);
            dto.Parameters.Add("@PageSize", dto.Limit);

            var source = await conn.QueryAsync<T>(query.ToString(), dto.Parameters);

            result.SearchResult = source.ToList();

            #endregion

            return result;
        }
    }
}

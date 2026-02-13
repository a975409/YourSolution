using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Infrastructure.DTOs.Response;

namespace YourSolution.Infrastructure.Factories
{
    public class SearchPageResultDtoFactory
    {
        private readonly IMapper _iMapper;

        public SearchPageResultDtoFactory(IMapper iMapper)
        {
            _iMapper = iMapper;
        }

        /// <summary>
        /// 透過來源資料複製一個新的SearchPageResultDto
        /// </summary>
        /// <typeparam name="S">來源的資料型態</typeparam>
        /// <typeparam name="T">產出結果的資料型態</typeparam>
        /// <param name="source">來源資料</param>
        /// <returns>產出的SearchPageResultDto</returns>
        public SearchPageResultDto<T> CopyToNewSearchPageResultDto<S, T>(SearchPageResultDto<S> source)
            where S : class
            where T : class
        {
            var result = new SearchPageResultDto<T>();
            result.Page = source.Page;
            result.TotalCount = source.TotalCount;
            result.TotalPage = source.TotalPage;
            result.pageRangeList = source.pageRangeList;
            result.SearchResult = _iMapper.Map<List<T>>(source.SearchResult);

            return result;
        }

        /// <summary>
        /// 取得查詢結果的頁碼設定
        /// </summary>
        /// <typeparam name="T">查詢結果的資料類型</typeparam>
        /// <param name="totalCount">資料總筆數</param>
        /// <param name="page">指定頁數</param>
        /// <param name="limit">單頁顯示幾筆資料</param>
        /// <returns></returns>
        internal SearchPageResultDto<T> GetSearchPageResultDto<T>(int totalCount, int page, int limit)
            where T : class
        {
            var result = new SearchPageResultDto<T>();

            result.TotalCount = totalCount;
            result.TotalPage = result.TotalCount / limit;

            if (result.TotalCount % limit > 0)
                result.TotalPage++;

            if (page > result.TotalPage)
                result.Page = result.TotalPage;
            else
                result.Page = page;

            if (result.Page <= 0)
                result.Page = 1;

            int pageRangeSize = 10;
            int startPage = (result.Page - 1) / pageRangeSize * pageRangeSize + 1;
            int endPage = startPage + pageRangeSize - 1;

            if (result.TotalPage < endPage)
                endPage = result.TotalPage;

            List<int> pageRange = new List<int>();

            for (int i = startPage; i <= endPage; i++)
                pageRange.Add(i);

            result.pageRangeList = pageRange.ToArray();

            return result;
        }
    }
}

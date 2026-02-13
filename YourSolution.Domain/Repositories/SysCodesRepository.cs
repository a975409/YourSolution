using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.Data;
using YourSolution.Domain.Models;

namespace YourSolution.Domain.Repositories
{
    public class SysCodesRepository
    {
        private readonly AppDBContext _context;

        public SysCodesRepository(AppDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取得單筆SysCode資料
        /// </summary>
        /// <param name="codeKind"></param>
        /// <returns></returns>
        public Task<SysCode?> GetFirstSysCodeByCodeKindAsync(string codeKind)
        {
            return _context.SysCodes.AsNoTracking().FirstOrDefaultAsync(x => x.CodeKind == codeKind);
        }

        /// <summary>
        /// 新增一筆SysCode資料
        /// </summary>
        /// <param name="sysCode"></param>
        /// <returns></returns>
        public async Task<int> AddSysCodeAsync(SysCode sysCode)
        {
            _context.SysCodes.Add(sysCode);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 取得多筆SysCode資料
        /// </summary>
        /// <param name="codeKind"></param>
        /// <returns></returns>
        public Task<List<SysCode>> GetSysCodeListByCodeKindAsync(string codeKind)
        {
            return _context.SysCodes.AsNoTracking()
                                    .Where(x => x.CodeKind == codeKind)
                                    .ToListAsync();
        }

        /// <summary>
        /// 新增多筆SysCode資料
        /// </summary>
        /// <param name="sysCodeList"></param>
        /// <returns></returns>
        public async Task<int> AddSysCodeAsync(List<SysCode> sysCodeList)
        {
            _context.SysCodes.AddRange(sysCodeList);
            return await _context.SaveChangesAsync();
        }
    }
}

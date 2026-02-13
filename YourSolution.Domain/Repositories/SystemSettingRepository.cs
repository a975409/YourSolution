using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.ConstDefines;
using YourSolution.Domain.Data;
using YourSolution.Domain.Models;

namespace YourSolution.Domain.Repositories
{
    public class SystemSettingRepository
    {
        /// <summary>
        /// 最大可登入失敗的次數，超過就會鎖定帳戶(預設值)
        /// </summary>
        public readonly int DefaultMaxLoginFailCount;
        private readonly AppDBContext _context;
        private readonly SysCodesRepository _sysCodesRepository;
        private readonly ILogger _logger;

        public SystemSettingRepository(AppDBContext context, IMapper iMapper, SysCodesRepository sysCodesRepository, ILogger<SystemSettingRepository> logger)
        {
            DefaultMaxLoginFailCount = 10;
            _context = context;
            _sysCodesRepository = sysCodesRepository;
            _logger = logger;
        }

        /// <summary>
        /// 取得最大可登入失敗的次數(SysCode資料表未設定時，會回傳系統預設值)
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetMaxLoginFailCountForScheduleAsync()
        {
            try
            {
                var result = await _sysCodesRepository.GetFirstSysCodeByCodeKindAsync(CodeKindConstDefine.MaxLoginFailCount);

                if (result == null)
                    return DefaultMaxLoginFailCount;

                if (int.TryParse(result.CodeValue, out int FrequencyMinutes))
                    return FrequencyMinutes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return DefaultMaxLoginFailCount;
        }

        /// <summary>
        /// 取得最大可登入失敗的次數
        /// </summary>
        /// <returns></returns>
        public async Task<int?> GetMaxLoginFailCountAsync()
        {
            var result = await _sysCodesRepository.GetFirstSysCodeByCodeKindAsync(CodeKindConstDefine.MaxLoginFailCount);

            if (result == null)
                return null;

            if (int.TryParse(result.CodeValue, out int FrequencyMinutes))
                return FrequencyMinutes;

            return 0;
        }

        /// <summary>
        /// 設定最大可登入失敗的次數，超過就會鎖定帳戶
        /// </summary>
        /// <param name="LoginFailCount"></param>
        /// <returns></returns>
        public async Task<int> SetMaxLoginFailCountAsync(int LoginFailCount)
        {
            if (LoginFailCount > byte.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(LoginFailCount), "最大可登入失敗的次數不可超過255");

            var result = await _sysCodesRepository.GetFirstSysCodeByCodeKindAsync(CodeKindConstDefine.MaxLoginFailCount);

            if (result == null)
            {
                return await _sysCodesRepository.AddSysCodeAsync(new SysCode
                {
                    CodeKind = CodeKindConstDefine.MaxLoginFailCount,
                    CodeName = "最大可登入失敗的次數，超過就會鎖定帳戶",
                    CodeValue = LoginFailCount.ToString(),
                    Order = 0
                });
            }

            result.CodeValue = LoginFailCount.ToString();
            _context.Entry(result).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }
    }
}

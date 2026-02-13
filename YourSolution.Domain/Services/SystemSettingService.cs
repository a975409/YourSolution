using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.DTOs.Request;
using YourSolution.Domain.Repositories;

namespace YourSolution.Domain.Services
{
    public class SystemSettingService
    {
        public readonly SystemSettingRepository _systemSettingRepository;

        public SystemSettingService(SystemSettingRepository systemSettingRepository)
        {
            _systemSettingRepository = systemSettingRepository;
        }

        /// <summary>
        /// 取得系統參數的設定值
        /// </summary>
        /// <returns></returns>
        public async Task<SystemSettingDto> GetSystemSettingDtoAsync()
        {
            var dto = new SystemSettingDto
            {
                MaxLoginFailCount = await _systemSettingRepository.GetMaxLoginFailCountAsync() ?? 0,
            };

            return dto;
        }

        /// <summary>
        /// 更新系統參數
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task UpdateSystemSettingDtoAsync(SystemSettingDto dto)
        {
            await _systemSettingRepository.SetMaxLoginFailCountAsync(dto.MaxLoginFailCount);
        }
    }
}

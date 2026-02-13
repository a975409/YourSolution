using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Infrastructure.IOptions;

namespace YourSolution.Infrastructure.Services
{
    /// <summary>
    /// appsettings.json的設定檔
    /// </summary>
    public class AppSettingService
    {
        private readonly IOptionsMonitor<AppSettings> _settingsMonitor;

        public AppSettingService(IOptionsMonitor<AppSettings> settingsMonitor)
        {
            _settingsMonitor = settingsMonitor;
        }

        /// <summary>
        /// 取得appsettings.json的設定檔
        /// </summary>
        /// <returns></returns>
        public AppSettings GetCurrentAppSetting()
        {
            var AppSetting = _settingsMonitor.CurrentValue;

            if (string.IsNullOrEmpty(AppSetting.ignoreRequestPath))
                AppSetting.ignoreRequestPath = string.Empty;

            AppSetting.requestLogDayRange = AppSetting.requestLogDayRange ?? 7;

            if (AppSetting.requestLogDayRange < 1)
                AppSetting.requestLogDayRange = 1;

            AppSetting.allRequestLog = AppSetting.allRequestLog ?? false;

            return AppSetting;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Infrastructure.IOptions
{
    /// <summary>
    /// appsettings.json的設定檔
    /// </summary>
    public class AppSettings
    {
        public bool? allRequestLog { get; set; }
        public string? ignoreRequestPath { get; set; }
        public int? requestLogDayRange { get; set; }
    }
}

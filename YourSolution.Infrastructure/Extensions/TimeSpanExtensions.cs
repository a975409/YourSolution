using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Infrastructure.Extensions
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// 取得時分(24小時制)
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string TohhmmFormate(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm");
        }

        /// <summary>
        /// 取得時分(24小時制)(不含(:))
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string TohhmmFormateNotslash(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hhmm");
        }

        /// <summary>
        /// 取得時分秒(24小時制)
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string TohhmmssFormate(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}

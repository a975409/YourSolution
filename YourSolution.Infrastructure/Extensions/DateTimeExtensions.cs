using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 取得完整民國年格式字串
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string ToFullTaiwanDate(this DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

            return string.Format("民國 {0} 年 {1} 月 {2} 日",
                taiwanCalendar.GetYear(datetime),
                datetime.Month,
                datetime.Day);
        }

        /// <summary>
        /// 取得民國年格式字串(含(/))
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string ToSimpleTaiwanDate(this DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

            return string.Format("{0}/{1}/{2}",
                taiwanCalendar.GetYear(datetime),
                datetime.Month,
                datetime.Day);
        }
        /// <summary>
        /// 取得民國年格式字串(不含(/))
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string ToSimpleTaiwanDateNotslash(this DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

            return string.Format("{0}{1}{2}",
                taiwanCalendar.GetYear(datetime),
                datetime.Month,
                datetime.Day);
        }

        /// <summary>
        /// 取得民國年格式字串(不含(/)、含時間)
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string ToSimpleTaiwanDateTimeNotslash(this DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

            return $"{taiwanCalendar.GetYear(datetime):000}{datetime.Month:00}{datetime.Day:00}{datetime.Hour:00}{datetime.Minute:00}";
        }

        /// <summary>
        /// 取得西元年格式字串(yyyy/MM/dd)
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSimpleDate(this DateTime? datetime)
        {
            if (datetime == null)
                return string.Empty;

            return ((DateTime)datetime).ToString("yyyy/MM/dd");
        }

        /// <summary>
        /// 取得西元年格式字串(yyyy/MM/dd)
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSimpleDate(this DateTime datetime)
        {
            return ((DateTime)datetime).ToString("yyyy/MM/dd");
        }

        /// <summary>
        /// 取得西元年格式字串(yyyy-MM-dd)，前端Html input[type=date]用
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSimpleDateForHTMLInputDate(this DateTime? datetime)
        {
            if (datetime == null)
                return string.Empty;

            return ((DateTime)datetime).ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 取得西元年格式字串(yyyy-MM-dd)，前端Html input[type=date]用
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSimpleDateForHTMLInputDate(this DateTime datetime)
        {
            return ((DateTime)datetime).ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 取得日期時間格式字串(yyyy-MM-dd)，前端Html input[type=date]用
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSimpleDateForHTMLInputDateTime(this DateTime? datetime)
        {
            if (datetime == null)
                return string.Empty;

            return ((DateTime)datetime).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 取得日期時間格式字串(yyyy-MM-dd)，前端Html input[type=date]用
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSimpleDateForHTMLInputDateTime(this DateTime datetime)
        {
            return ((DateTime)datetime).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 取得西元年格式字串(yyyy/MM/dd)，含時分
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSimpleDateTime(this DateTime? datetime)
        {
            if (datetime == null)
                return string.Empty;

            return ((DateTime)datetime).ToString("yyyy/MM/dd HH:mm");
        }

        /// <summary>
        /// 取得西元年格式字串(yyyy/MM/dd)，含時分
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToSimpleDateTime(this DateTime datetime)
        {
            return ((DateTime)datetime).ToString("yyyy/MM/dd HH:mm");
        }

        /// <summary>
        /// 取得西元年格式字串(yyyyMMdd_HHmmss)，含時分秒
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToAllDateTime(this DateTime? datetime)
        {
            if (datetime == null)
                return string.Empty;

            return ((DateTime)datetime).ToString("yyyyMMdd_HHmmss");
        }

        /// <summary>
        /// 取得西元年格式字串(yyyyMMdd_HHmmss)，含時分
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToAllDateTime(this DateTime datetime)
        {
            return ((DateTime)datetime).ToString("yyyyMMdd_HHmmss");
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 檢查該字串是否包含其中一個字串陣列元素
        /// </summary>
        /// <param name="str"></param>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static bool AnyArrayElementIsContains(this string str, string[] arr)
        {
            bool IsContains = false;

            foreach (var item in arr)
            {
                if (str.Contains(item))
                {
                    IsContains = true;
                    break;
                }
            }

            return IsContains;
        }

        /// <summary>
        /// 密碼加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string ToEncryptPassword(this string password)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// 設定字串只會顯示前面幾個位數，後面的字元會被 * 取代。
        /// </summary>
        /// <param name="input">要處理的字串。</param>
        /// <param name="visibleCount">前面要顯示的字元數。</param>
        /// <returns></returns>
        public static string MaskString(this string input, int visibleCount)
        {
            if (string.IsNullOrEmpty(input) || visibleCount <= 0)
            {
                // 如果輸入是空字串或visibleCount不合理，全部用*表示
                return new string('*', input?.Length ?? 0);
            }

            if (visibleCount >= input.Length)
            {
                // 如果visibleCount超過字串長度，直接回傳原字串
                return input;
            }

            // 取出前面 visibleCount 個字元
            string visiblePart = input.Substring(0, visibleCount);

            // 後面補 * 號
            string maskedPart = new string('*', input.Length - visibleCount);

            return visiblePart + maskedPart;
        }

        /// <summary>
        /// 將Json字串轉成指定的class DTO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T? ConvertToDto<T>(this string json)
            where T : class
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

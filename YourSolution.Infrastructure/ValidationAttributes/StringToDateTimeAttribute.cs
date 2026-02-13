using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourSolution.Infrastructure.ValidationAttributes
{
    /// <summary>
    /// 字串轉DateTime的屬性驗證
    /// </summary>
    public class StringToDateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string strDate = (string)value;

            if (DateTime.TryParse(strDate, out DateTime dateTime) == false)
                return new ValidationResult("請輸入正確的日期格式");

            return ValidationResult.Success;
        }
    }
}

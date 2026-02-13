using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;
using Ganss.Xss;
using System.Collections;

namespace YourSolution.Web.Filters
{
    public class XSSFilter : ActionFilterAttribute
    {
        private readonly HtmlSanitizer htmlSanitizer = new HtmlSanitizer();

        /// <summary>
        /// 處理 Stored XSS attacks 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var parameters = context.ActionArguments;

            foreach (var key in parameters.Keys)
                parameters[key] = SanitizeActionParameters(parameters[key], false);

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// 處理 Reflected XSS attacks
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // 處理 ViewResult (MVC)
            if (context.Result is ViewResult viewResult)
            {
                //針對ViewBag、ViewData的值做處理
                foreach (var key in viewResult.ViewData.Keys.ToList())
                {
                    if (viewResult.ViewData[key] != null)
                    {
                        viewResult.ViewData[key] = SanitizeActionParameters(viewResult.ViewData[key], true);
                    }
                }

                if (viewResult.ViewData.Model != null)
                    viewResult.ViewData.Model = SanitizeActionParameters(viewResult.ViewData.Model, true);
            }
            // 處理 ObjectResult (API)
            else if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.Value != null)
                {
                    objectResult.Value = SanitizeActionParameters(objectResult.Value, true);
                }
            }
            // 處理 JsonResult
            else if (context.Result is JsonResult jsonResult)
            {
                if (jsonResult.Value != null)
                {
                    jsonResult.Value = SanitizeActionParameters(jsonResult.Value, true);
                }
            }

            base.OnResultExecuting(context);
        }

        /// <summary>
        /// 依據value的型態決定處理編碼的方式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decrypt">是否要解碼(反序列化)</param>
        /// <returns></returns>
        private object SanitizeActionParameters(object value, bool decrypt)
        {
            if (value == null)
                return value;

            if (value is string stringValue)
            {
                value = HtmlXSSFilter(stringValue, decrypt);
            }
            else if (value.GetType().IsArray)
            {
                if (value is string[] stringArray)
                {
                    for (int i = 0; i < stringArray.Length; i++)
                        stringArray[i] = SanitizeActionParameters(stringArray[i], decrypt) as string;
                }
            }
            else if (value is IEnumerable)
            {
                //List<object> list = new List<object>();
                //foreach (var item in (IEnumerable)value)
                //    list.Add(item);

                //for (int i = 0; i < list.Count; i++)
                //{
                //    if (list[i] == null)
                //        continue;

                //    list[i] = SanitizeActionParameters(list[i]);
                //}

                //value = list;
            }
            else if (value is ICollection)
            {
                //List<object> list = new List<object>();
                //foreach (var item in (ICollection)value)
                //    list.Add(item);

                //for (int i = 0; i < list.Count; i++)
                //{
                //    if (list[i] == null)
                //        continue;

                //    list[i] = SanitizeActionParameters(list[i]);
                //}

                //value = list;
            }
            else if (value is IList)
            {
                var list = value as IList;

                for (int i = 0; i < list.Count; i++)
                    list[i] = SanitizeActionParameters(list[i], decrypt);
            }
            else if (value.GetType().IsClass && value.GetType().IsPrimitive == false)
                ProcessObjectProperties(value, decrypt);

            return value;
        }

        /// <summary>
        /// 針對物件內所有屬性的值做處理
        /// </summary>
        /// <param name="obj"></param>
        private void ProcessObjectProperties(object obj, bool decrypt)
        {
            if (obj.GetType().IsClass == false || obj.GetType().IsPrimitive || obj == null)
                return;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                if (property.CanWrite)
                    property.SetValue(obj, SanitizeActionParameters(value, decrypt));
            }
        }

        /// <summary>
        /// 單純針對字串做處理
        /// </summary>
        /// <param name="html"></param>
        /// <param name="decrypt">是否要解碼(反序列化)</param>
        /// <returns></returns>
        private string HtmlXSSFilter(string html, bool decrypt)
        {
            //回傳至客戶端時解碼
            if (decrypt)
                return WebUtility.HtmlDecode(html);

            if (!string.IsNullOrWhiteSpace(html) && htmlSanitizer != null)
                html = htmlSanitizer.Sanitize(html);

            return html;
        }
    }
}

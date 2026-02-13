using YourSolution.Domain.Data;
using YourSolution.Infrastructure.Models;
using YourSolution.Infrastructure.Services;

namespace YourSolution.Web.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettingService _appSettingService;

        public RequestLoggingMiddleware(RequestDelegate next, AppSettingService appSettingService)
        {
            _next = next;
            _appSettingService = appSettingService;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            string requestBody = string.Empty;

            // 讀取並序列化 Header
            var headersDict = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            string requestHeadersJson = System.Text.Json.JsonSerializer.Serialize(headersDict);

            // 啟用 Buffering 以便多次讀取 Body
            context.Request.EnableBuffering();

            if (context.Request.HasFormContentType)
            {
                var form = await context.Request.ReadFormAsync();
                var dict = new Dictionary<string, object>();

                // 讀取一般欄位
                foreach (var field in form)
                {
                    dict[field.Key] = field.Value.ToString();
                }

                // 讀取檔案元資料
                foreach (var file in form.Files)
                {
                    dict[file.Name] = new
                    {
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        Length = file.Length
                    };
                }

                requestBody = System.Text.Json.JsonSerializer.Serialize(dict);
            }
            else if ((context.Request.ContentLength > 0 || context.Request.ContentLength == null) && context.Request.Body.CanRead && context.Request.Body.CanSeek)
            {
                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);

                try
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                catch { }

                context.Request.Body.Position = 0;
            }

            // 攔截 Response Body
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // 執行後續中介軟體
            await _next(context);

            using var scope = serviceProvider.CreateScope();
            var _dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();

            try
            {
                var AppSetting = _appSettingService.GetCurrentAppSetting();

                string[] ignoreRequestPath = Array.Empty<string>();

                if (string.IsNullOrEmpty(AppSetting.ignoreRequestPath) == false)
                    ignoreRequestPath = AppSetting.ignoreRequestPath.Split(',');

                if (ignoreRequestPath?.Contains(context.Request.Path.Value) == false)
                {
                    //依據Content-Type來判斷請求端呼叫的是否為API，如果是API才新增一筆記錄至RequestLog資料表
                    if (context.Response.Headers.TryGetValue("Content-Type", out var contentType) || (AppSetting.allRequestLog ?? false))
                    {
                        if (contentType.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase) || (AppSetting.allRequestLog ?? false))
                        {
                            // 讀取 Response Body
                            context.Response.Body.Seek(0, SeekOrigin.Begin);
                            string responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();

                            // 讀取 Response Headers
                            var responseHeaders = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
                            string responseHeadersJson = System.Text.Json.JsonSerializer.Serialize(responseHeaders);

                            // 紀錄資料
                            var log = new RequestLog
                            {
                                Method = context.Request.Method,
                                Path = context.Request.Path,
                                QueryString = context.Request.QueryString.ToString(),
                                Headers = requestHeadersJson,
                                Body = requestBody,
                                Timestamp = DateTime.Now,
                                ResponseStatusCode = context.Response.StatusCode,
                                ResponseHeaders = responseHeadersJson,
                                ResponseBody = responseBodyText
                            };

                            _dbContext.RequestLogs.Add(log);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                // 將攔截到的 Response Body 寫回原始 Response 流
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }
}

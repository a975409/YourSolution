using YourSolution.Infrastructure.Repositories;
using YourSolution.Infrastructure.Services;

namespace YourSolution.WebApi.BackgroundServices
{
    public class RequestLogRemoveAll_BackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;
        private readonly AppSettingService _appSettingService;

        public RequestLogRemoveAll_BackgroundService(IServiceScopeFactory scopeFactory, ILogger<RequestLogRemoveAll_BackgroundService> logger, AppSettingService appSettingService)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _appSettingService = appSettingService;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    // 每天執行一次清理動作
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var _repository = scope.ServiceProvider.GetRequiredService<RequestLogRepository>();
                        var requestLogDayRange = _appSettingService.GetCurrentAppSetting().requestLogDayRange ?? 30;

                        try
                        {
                            // 刪除超過指定天數的資料
                            await _repository.RemoveExpiredLogByDayAsync(requestLogDayRange);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error occurred while removing expired request logs.");
                        }
                    }

                    // 延遲 24 小時後再次執行
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // 服務被取消，正常結束
                _logger.LogInformation("RequestLogRemoveAll_BackgroundService is stopping.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background service encountered an error.");
            }
        }
    }
}

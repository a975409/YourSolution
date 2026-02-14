using System.Reflection;
using YourSolution.Domain;
using YourSolution.Infrastructure;
using YourSolution.Infrastructure.IOptions;
using YourSolution.WebApi.BackgroundServices;
using YourSolution.WebApi.Middleware;
using YourSolution.WebApi.Security;

var builder = WebApplication.CreateBuilder(args);

//熱載入appsettings.json的設定檔
builder.Configuration
 .SetBasePath(AppContext.BaseDirectory)
 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
 .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddDomainServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHostedService<RequestLogRemoveAll_BackgroundService>();
builder.Services.APISettingXSRF();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 1. 放在最前面，盡早攔截請求與回應
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

using System.Reflection;
using YourSolution.Domain;
using YourSolution.Infrastructure;
using YourSolution.Infrastructure.IOptions;
using YourSolution.Web.Auth.Middlewares;
using YourSolution.Web.Auth.Schemes;
using YourSolution.Web.Auth.Services;
using YourSolution.Web.BackgroundServices;
using YourSolution.Web.Filters;
using YourSolution.Web.Middleware;
using YourSolution.Web.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(option => option.Filters.Add<XSSFilter>())
                .AddRazorRuntimeCompilation();
builder.Services.AddWindowsOrCookieScheme();

//熱載入appsettings.json的設定檔
builder.Configuration
 .SetBasePath(AppContext.BaseDirectory)
 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
 .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddRazorPages();
builder.Services.AddDomainServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHostedService<RequestLogRemoveAll_BackgroundService>();
builder.Services.APISettingXSRF();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 1. 放在最前面，盡早攔截請求與回應
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseWindowsMiddleware();
app.UseAuthorization();
app.UseMiddleware<RedirectToLoginMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

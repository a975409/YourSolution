using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.Data;
using YourSolution.Domain.Repositories;
using YourSolution.Domain.Services;

namespace YourSolution.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<SysCodesRepository>();
            services.AddScoped<SystemSettingRepository>();
            services.AddScoped<UserAccountRepository>();
            services.AddScoped<SystemSettingService>();
            services.AddScoped<UserAccountService>();
            services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}
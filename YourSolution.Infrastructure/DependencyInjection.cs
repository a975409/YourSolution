using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Infrastructure.Factories;
using YourSolution.Infrastructure.Repositories;
using YourSolution.Infrastructure.Services;

namespace YourSolution.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<RequestLogRepository>();
            services.AddScoped<PaginationQueryRepository>();
            services.AddScoped<SearchPageResultDtoFactory>();
            services.AddSingleton<AppSettingService>();
            services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}

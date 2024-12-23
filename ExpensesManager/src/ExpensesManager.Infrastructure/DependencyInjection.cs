using ExpensesManager.Infrastructure.Data;
using ExpensesManager.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpensesManager.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("ConnectionString");

            builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddScoped<ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            builder.Services.AddScoped<ApplicationDbContextInitialiser>();

            builder.Services.AddSingleton(TimeProvider.System);
        }
    }
}

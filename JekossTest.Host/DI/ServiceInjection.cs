using Jekoss.Implementations.Services;
using Jekoss.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JekossTest.DI
{
    public static class ServiceInjection
    {
        public static void AddService(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped<IAccountService,AccountService>();
        }
    }
}
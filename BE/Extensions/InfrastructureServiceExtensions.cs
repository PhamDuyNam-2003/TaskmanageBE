using BE.Services.Implements;
using BE.Services.Interfaces;
using MongoDB.Driver;

namespace BE.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoSettings = configuration.GetSection("DatabaseSettings");
            var client = new MongoClient(mongoSettings["ConnectionString"]);
            var database = client.GetDatabase(mongoSettings["DatabaseName"]);

            services.AddSingleton(database);

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITaskService, TaskService>(); 

            return services;
        }
    }
}
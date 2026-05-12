using Microsoft.EntityFrameworkCore;

using BE.Data;

namespace BE.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("PostgreConnection");


            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));


            return services;
        }
    }
}
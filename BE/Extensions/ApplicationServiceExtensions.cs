using BE.Services.Implements;
using BE.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BE.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
        {
            {
                var jwtSettings = config.GetSection("JwtSettings");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

                services.AddAuthentication(options =>
                {

                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwtSettings["Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero 
                    };
                });
                services.AddControllers();
                services.AddOpenApi();

                services.AddScoped<IProjectService, ProjectService>();
                services.AddScoped<ITaskService, TaskService>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IAuthService, AuthService>();

                services.AddAutoMapper(typeof(Program).Assembly);

                services.AddValidatorsFromAssemblyContaining<Program>();
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", builder =>
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                });

                return services;
            }

        }
    }
}

using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using BE.Services.Interfaces;
using BE.Services.Implements;

namespace BE.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services,
            IConfiguration config)
        {

            var jwtSettings = config.GetSection("JwtSettings");

            var secret = jwtSettings["Secret"]
                ?? throw new Exception("JWT Secret missing");

            var key = Encoding.UTF8.GetBytes(secret);



            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler =
                        ReferenceHandler.IgnoreCycles;
                });


            services.AddEndpointsApiExplorer();




            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(key),

                        ValidateIssuer = true,

                        ValidIssuer = jwtSettings["Issuer"],

                        ValidateAudience = true,

                        ValidAudience = jwtSettings["Audience"],

                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.Zero
                    };
            });



            services.AddAutoMapper(cfg => { },typeof(Program).Assembly);



            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddFluentValidationAutoValidation();



            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });


            services.AddScoped<IProjectService, ProjectService>();

            services.AddScoped<ITaskService, TaskService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IAuthService, AuthService>();


            return services;
        }
    }
}
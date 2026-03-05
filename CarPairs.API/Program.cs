using CarPairs.Core;
using CarPairs.Core.Services;
using CarPairs.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace CarPairs.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IPartService, PartService>();
            builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CarPairs API",
                    Version = "v1",
                    Description = "Auto Parts Management System API"
                });

                // 🔐 Prepare Swagger for JWT (we'll enable JWT soon)
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token like: Bearer {your token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // ============================
            // AUTHENTICATION
            // - In production use real JWT or other schemes
            // - In Development enable a simple dev auth handler that injects a test user (Admin role)
            // ============================
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "DevAuth";
                    options.DefaultChallengeScheme = "DevAuth";
                })
                .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, CarPairs.API.Authentication.DevelopmentAuthenticationHandler>(
                    "DevAuth", options => { });

                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
                });
            }
            else
            {
                builder.Services.AddAuthentication();
                builder.Services.AddAuthorization();
            }

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
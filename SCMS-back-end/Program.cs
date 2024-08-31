using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Services;
using System.Text.Json.Serialization;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Service configuration
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                });

            // Connection string + DbContext
            string ConnectionStringVar = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<StudyCenterDbContext>(optionsX => optionsX.UseSqlServer(ConnectionStringVar));

            // Identity configuration
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<StudyCenterDbContext>();

            // JWT authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = JwtTokenService.ValidateToken(builder.Configuration);
                });

            // Register custom services
            builder.Services.AddScoped<ISubject, IdentitySubjectServices>();

            // Swagger configuration
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "StudyCenter API",
                    Version = "v1",
                    Description = "API for managing study center operations."
                });

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Please enter your JWT token below."
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            // Middleware configuration
            var app = builder.Build();

            // Swagger middleware
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/api/v1/swagger.json", "StudyCenter API v1");
                options.RoutePrefix = "";
            });

            // Authentication & Authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Map controllers
            app.MapControllers();

            app.MapGet("/", () => "Hello World!");
            app.Run();
        }
    }
}

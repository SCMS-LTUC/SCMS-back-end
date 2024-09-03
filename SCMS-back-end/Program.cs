using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Services;
using System.Text.Json.Serialization;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Models;

namespace SCMS_back_end
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //service configuration
            //builder.Services.AddControllers();

            // Configure JSON options to handle object cycles
            // Service configuration
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                });

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "SCMS API",
                    Version = "v1",
                    Description = "API for managing students, courses and teachers in the study center"
                });
            });

            //connection string + DbContext
            // Connection string + DbContext
            string ConnectionStringVar = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<StudyCenterDbContext>(optionsX => optionsX.UseSqlServer(ConnectionStringVar));

            //Identity 
            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<StudyCenterDbContext>();
            builder.Services.AddScoped<IAccount, IdentityAccountService>();

            // Register repositories
            //builder.Services.AddScoped<IPlaylist, PlaylistService>();
            
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
                options.SwaggerEndpoint("/api/v1/swagger.json", "SCMS API v1");
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

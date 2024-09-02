using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Identity.Client;
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
            string ConnectionStringVar = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<StudyCenterDbContext>(optionsX => optionsX.UseSqlServer(ConnectionStringVar));

            //Identity 
            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<StudyCenterDbContext>();
            builder.Services.AddScoped<IAccount, IdentityAccountService>();
            builder.Services.AddScoped<IAssignment , AsignmentService>();
            builder.Services.AddScoped<ITeacher, TeacherService>();

            // Register repositories
            //builder.Services.AddScoped<IPlaylist, PlaylistService>();


            //JWT authentication
            builder.Services.AddAuthentication(
                options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = JwtTokenService.ValidateToken(builder.Configuration);
                }
                );


            //swagger configuration
            builder.Services.AddSwaggerGen
                (

                option =>
                {
                    option.SwaggerDoc("employeesApi", new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "Employees Api Doc",
                        Version = "v1",
                        Description = "Api for managing all emolyees"
                    });

                    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Please enter user token below."
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


            //middleware configuration
            var app = builder.Build();

            //swagger
            app.UseSwagger(
             options =>
             {
                 options.RouteTemplate = "api/{documentName}/swagger.json";
             }
             );


            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/api/v1/swagger.json", "SCMS API v1");
                options.RoutePrefix = "";
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();


            app.MapGet("/", () => "Hello World!");
            app.Run();
        }
    }
    
}

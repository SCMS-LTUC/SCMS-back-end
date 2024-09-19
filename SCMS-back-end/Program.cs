using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Services;
using System.Text.Json.Serialization;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Models;
using SCMS_back_end.Services;
using Microsoft.Extensions.Options;

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
                    Title = "EduSphere API",
                    Version = "v1",
                    Description = "API for managing students, courses and teachers in the study center"
                });
            });

            //connection string + DbContext
            string ConnectionStringVar = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<StudyCenterDbContext>(optionsX => optionsX.UseSqlServer(ConnectionStringVar));

            //Identity 
            builder.Services.AddScoped<IEmail, EmailService>();
            builder.Services.AddIdentity<User, IdentityRole>(
                    options =>
                    {
                        options.User.RequireUniqueEmail = true; 
                    })  
                .AddEntityFrameworkStores<StudyCenterDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddScoped<IAccount, IdentityAccountService>();
            builder.Services.AddScoped<IDepartment, DepartmentService>();
            // Register custom services
            builder.Services.AddScoped<ISubject, SubjectService>();

            builder.Services.AddScoped<ILecture, LectureService>();

            builder.Services.AddHostedService<WeeklyTaskService>();
            builder.Services.AddScoped<ICourse, CourseService>();
            builder.Services.AddScoped<IAssignment , AssignmentService>();
            builder.Services.AddScoped<ITeacher, TeacherService>();


            // Register repositories
            //builder.Services.AddScoped<IPlaylist, PlaylistService>();

            builder.Services.AddScoped<IStudent, StudentService>();
            builder.Services.AddScoped<IStudentAssignments, StudentAssignmentsService>();

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

            builder.Services.AddAuthorization(options =>
            {
                // You can define policies if needed, or use the default policy
                options.AddPolicy("DefaultPolicy", policy =>
                    policy.RequireAuthenticatedUser());
            });

            // Register custom services
            builder.Services.AddScoped<ISubject, SubjectService>();

            //swagger configuration
            builder.Services.AddSwaggerGen
                (

                option =>
                {
                    option.SwaggerDoc("EduSphereApi", new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "EduSphere Api Doc",
                        Version = "v1",
                        Description = "Api for managing all academic operations"
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
                options.SwaggerEndpoint("/api/v1/swagger.json", "EduSphere API v1");
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

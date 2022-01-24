using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WorkforceManagementAPI.BLL.Managers;
using WorkforceManagementAPI.BLL.Services;
using WorkforceManagementAPI.BLL.Services.Background;
using WorkforceManagementAPI.DAL;
using WorkforceManagementAPI.DAL.Entities;
using WorkforceManagementAPI.DAL.Repositories;
using WorkforceManagementAPI.Web.Auth;
using WorkforceManagementAPI.Web.Policies;

namespace WorkforceManagementAPI.Web
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkforceManagementAPI", Version = "v1" });

                // Adds the authorize button in swagger UI 
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                // Uses the token from the authorize input and sends it as a header
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                            {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            //EFCore
            services.AddDbContext<WorkforceManagementAPIDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Default"]));

            //EFIdentity
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<WorkforceManagementAPIDbContext>();

            //DAL
            services.AddTransient<ITeamRepository, TeamRepository>();
            services.AddTransient<ITimeOffRequestRepository, TimeOffRequestRepository>();
            services.AddTransient<IApprovalRespository, ApprovalRespository>();
            services.AddTransient<IHolidaysRepository, HolidaysRepository>();

            //BLL
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<IUserManager, WorkforceManagementAPIUserManager>();
            services.AddTransient<ITimeOffService, TimeOffService>();
            services.AddTransient<IEmailService, EmailService>();

            //Scheduler
            var scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            services.AddSingleton(scheduler);
            services.AddSingleton<IJobFactory, RequestsDeletionJobFactory>();
            services.AddHostedService<QuartzHostedService>();

            services.AddQuartz(
                q =>
                {
                    q.UseMicrosoftDependencyInjectionScopedJobFactory();
                    var jobKey = new JobKey("SetWorkingStatuses");
                    q.AddJob<SetWorkingStatusJob>(options => options.WithIdentity(jobKey));
                    q.AddTrigger(options => options
                    .ForJob(jobKey)
                    .WithIdentity("TriggerForWorkStatuses")
                    .WithCronSchedule("0 0 0 ? * * *"));

                    var jobKeyDaysOff = new JobKey("SetDaysOffForNewYear");
                    q.AddJob<SetDaysOffForNewYear>(options => options.WithIdentity(jobKeyDaysOff));
                    q.AddTrigger(options => options
                    .ForJob(jobKeyDaysOff)
                    .WithIdentity("TriggerForNewYear")
                    .WithCronSchedule("0 0 0 1 JAN ? *"));
                });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            // IdentityServer
            var builder = services.AddIdentityServer((options) =>
            {
                options.EmitStaticAudienceClaim = true;
            })
                                   .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
                                   .AddInMemoryClients(IdentityConfig.Clients);

            builder.AddDeveloperSigningCredential();
            builder.AddResourceOwnerValidator<PasswordValidator>();

            // Authentication
            // Adds the asp.net auth services
            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy("IsAdminOrTimeOffRequestCreator", policy =>
                        policy.Requirements.Add(new IsAdminOrTimeOffRequestCreator()));
                })
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })

                // Adds the JWT bearer token services that will authenticate each request based on the token in the Authorize header
                // and configures them to validate the token with the options
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.Audience = "https://localhost:5001/resources";
                    options.RequireHttpsMetadata = false;
                });

            services.AddTransient<IAuthorizationHandler, IsAdminOrTimeOffRequestHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            DbInitializer.Seed(app.ApplicationServices);

            app.UseIdentityServer();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkforceManagementAPI v1"));
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

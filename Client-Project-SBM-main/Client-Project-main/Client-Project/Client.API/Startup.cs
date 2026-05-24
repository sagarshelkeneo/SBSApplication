using System.Data;
using System.Text;
using Client.API.Authorization.Handlers;
using Client.API.Authorization.Requirements;
using Client.API.Middlewares;
using Client.Application;
using Client.Application.Interfaces;
using Client.Application.Profiles;
using Client.Domain.Models;
using Client.Persistence;
using Client.Persistence.Context;
using Client.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;

namespace Client.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connString = Configuration.GetConnectionString("DefaultConnection");

            //DbContext for Mysql
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connString));

            services.AddScoped<IDbConnection>(sp =>
                new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.
            services.AddApplicationServices();
            services.AddPersistenceServices(Configuration);
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            services.AddAuthorization(options =>
            {
                // Define screen codes and permissions
                var screenCodes = new[]
                {
                    "INVOICE", "PRODUCT", "SUBCONTRACTOR", "PAYMENT", "ADDITIONALENTITY",
                    "PAIDREPORT", "UNPAIDREPORT", "PRODUCTWISEREPORT", "SUBCONTRACTORWISEREPORT",
                    "COMBINEDREPORT", "COMPANY", "USER", "ROLE", "ROLEACCESS", "BANK"
                };
                var permissions = new[] { "View", "Create", "Edit", "Delete" };

                foreach (var screenCode in screenCodes)
                {
                    foreach (var permission in permissions)
                    {
                        options.AddPolicy($"{screenCode}_{permission}",
                            policy => policy.Requirements.Add(new ScreenAccessRequirement(screenCode, permission)));
                    }
                }
            });

            services.AddControllers();
            services.AddSwaggerGen();
            services.AddScoped<IAuthorizationHandler, ScreenAccessHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Client.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

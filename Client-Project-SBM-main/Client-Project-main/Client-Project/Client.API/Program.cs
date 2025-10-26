
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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System.Data;
using System.Text;

namespace Client.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            // DbContext
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlServer(connString),
            //    ServiceLifetime.Scoped);
            //builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connString));

            //DbContext for Mysql
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connString,
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
            ));
            builder.Services.AddScoped<IDbConnection>(sp =>
            new MySqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));


            // Add services to the container.
            builder.Services.AddApplicationServices();
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailService, EmailService>();


            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
            builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            builder.Services.AddAuthorization(options =>
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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAllOrigins",
            //        builder => builder.WithOrigins("http://localhost:4200")
            //                          .AllowAnyMethod()
            //                          .AllowAnyHeader());
            //});
            builder.Services.AddScoped<IAuthorizationHandler, ScreenAccessHandler>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(x => x
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.MapControllers();

            app.Run();
        }
    }
}

using System;
using System.Data;
using Client.Application.Interfaces;
using Client.Domain.Models;
using Client.Persistence;
using Client.Persistence.Repositories;
using Client_WebApp.Middleware;
using Client_WebApp.Models;
using Client_WebApp.Services;
using Client_WebApp.Services.Config;
using Client_WebApp.Services.Master;
using Client_WebApp.Services.Report;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client_WebApp
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

            // Add cookie authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/Login";      // redirect if not logged in
                    options.AccessDeniedPath = "/Home/AccessDenied";  // redirect if no permission
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);   // optional
                    options.SlidingExpiration = true;
                });

            services.AddAuthorization();

            services.AddControllersWithViews();

            // Add session
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddHttpContextAccessor();

            // Register repository and services
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<ISubContractorRepository, SubContractorRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IBankMasterRepository, BankMasterRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAdditionalEntityRepository, AdditionalEntityRepository>();
            services.AddScoped<ISalaryDetailsRepository, SalaryDetailsRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IRoleAccessRepository, RoleAccessRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();

            services.AddScoped<LoginService>();
            services.AddScoped<InvoiceService>();
            services.AddScoped<PaymentService>();
            services.AddScoped<BankService>();
            services.AddScoped<SubContractorService>();
            services.AddScoped<ProductService>();
            services.AddScoped<UserService>();
            services.AddScoped<AdditionalEntityService>();
            services.AddScoped<SalaryDetailsService>();
            services.AddScoped<CompanyService>();
            services.AddScoped<RoleService>();
            services.AddScoped<RoleAccessService>();
            services.AddScoped<ReportService>();

            services.AddScoped<IDbConnection>(sp => new SqlConnection(connString));

            // IEmailService 
            services.AddPersistenceServices(Configuration);
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();
            services.Configure<GoogleReCaptchaConfig>(Configuration.GetSection("GoogleReCaptcha"));

            // IJwtService
            services.AddScoped<IJwtService, JwtService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseMiddleware<AuthMiddleware>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

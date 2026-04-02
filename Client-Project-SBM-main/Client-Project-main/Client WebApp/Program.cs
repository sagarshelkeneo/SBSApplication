using Client.Application.Interfaces;
using Client.Domain.Models;
using Client.Persistence;
using Client.Persistence.Context;
using Client.Persistence.Repositories;
using Client_WebApp.Middleware;
using Client_WebApp.Models;
using Client_WebApp.Services;
using Client_WebApp.Services.Config;
using Client_WebApp.Services.Master;
using Client_WebApp.Services.Report;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";      // redirect if not logged in
        options.AccessDeniedPath = "/Home/AccessDenied";  // redirect if no permission
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);   // optional
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();

// Add session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// Register repository and services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<ISubContractorRepository, SubContractorRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IBankMasterRepository, BankMasterRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAdditionalEntityRepository, AdditionalEntityRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleAccessRepository, RoleAccessRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<BankService>();
builder.Services.AddScoped<SubContractorService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AdditionalEntityService>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<RoleAccessService>();
builder.Services.AddScoped<ReportService>();

// IDbConnection
//builder.Services.AddScoped<IDbConnection>(sp =>
//    new MySqlConnection(connString)
//);

builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connString));

// IEmailService 
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<GoogleReCaptchaConfig>(builder.Configuration.GetSection("GoogleReCaptcha"));

// IJwtService
builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseMiddleware<AuthMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Product.Dtos;
using Client.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Client.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet properties for your entities
        public DbSet<CompanyDto> Companies { get; set; } 
        public DbSet<ProductDto> GetAllProducts { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CompanyDto>().HasNoKey();
            modelBuilder.Entity<ProductDto>().HasNoKey();
            modelBuilder.Entity<CompanyMaster>().ToTable("sbs_companyMaster");
            modelBuilder.Entity<Product>().ToTable("sbs_productMaster");

        }
    }
}

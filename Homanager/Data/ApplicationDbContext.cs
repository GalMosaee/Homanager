using Homanager.Models;
using Homanager.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Homanager.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext()
           : base()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserGroup>()
                .HasKey(c => new { c.GroupId, c.UserId});
            //modelBuilder.Entity<CartProduct>()
            //    .HasKey(c => new { c.CartId, c.ProductId});
        }

        public DbSet<Group> Group { get; set; }

        public DbSet<UserGroup> UserGroup { get; set; }

        public DbSet<Cart> Cart { get; set; }

        public DbSet<Supermarket> Supermarket { get; set; }

        public DbSet<CartProduct> CartProduct { get; set; }

        public DbSet<Product> Product { get; set; }
        
        public DbSet<ProductCategory> ProductCategory { get; set; }

    }
}

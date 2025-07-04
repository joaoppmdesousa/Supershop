﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Supershop.Data.Entities;
using System.Linq;

namespace Supershop.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Product> Products { get; set; }


        public DbSet<Order> Orders { get; set; }


        public DbSet<OrderDetail> OrderDetails { get; set; }


        public DbSet<OrderDetailTemp> OrderDetailsTemp { get; set; }


        public DbSet<Country> Countries { get; set; }


        public DbSet<City> Cities { get; set; }




        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }


        // Habilitar a regra de apagar em cascata(Cascade Delete Rule)
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{     
        //    var cascadeFKs = modelBuilder.Model
        //        .GetEntityTypes()
        //        .SelectMany(t => t.GetForeignKeys())
        //        .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
        //    foreach (var fk in cascadeFKs)
        //    {
        //        fk.DeleteBehavior = DeleteBehavior.Restrict;
        //    }

        //    base.OnModelCreating(modelBuilder);
        //}

    }
}

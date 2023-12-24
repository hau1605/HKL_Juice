using HKL_Juice.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration;

namespace HKL_Juice
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("MyDbConnection")
        {

        }

        public DbSet<Product> Product { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }

    }
}
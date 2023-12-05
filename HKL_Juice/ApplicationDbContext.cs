using HKL_Juice.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HKL_Juice
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("MyDbConnection")
        {

        }

        public DbSet<Product> Product { get; set; }
    }
}
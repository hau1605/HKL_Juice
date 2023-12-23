using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HKL_Juice.Models
{
    public class Category
    {
        [Key]
        public int categoryId { get; set; } // Auto-incremented by the database
        public string categoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
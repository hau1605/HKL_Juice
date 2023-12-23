using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace HKL_Juice.Models
{
    public class Product
    {
        [Key]
        public int productId { get; set; }
        public int categoryId { get; set; }
        public string productName { get; set; }
        public int price { get; set; }
        public string descript { get; set; }
        public string imgUrl { get; set; }

        // Navigation property to link to Category object
        public virtual Category Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
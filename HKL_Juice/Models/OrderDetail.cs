using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HKL_Juice.Models
{
    public class OrderDetail
    {
       [Key]
        public int orderDetailId { get; set; } // Auto-incremented by the database
        public int orderId { get; set; } // Foreign key reference to Order
        public int productId { get; set; } // Foreign key reference to Product
        public int quantity { get; set; }
        public int subTotal { get; set; }
        public bool isNew { get; set; }

        // Navigation properties to link to Order and Product objects
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
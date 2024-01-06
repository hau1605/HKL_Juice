using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HKL_Juice.Models
{
    public class Order
    {
        [Key]
        public int orderId { get; set; } // Auto-incremented by the database
        public DateTime orderDate { get; set; }
        public string paymentStatus { get; set; }
        public string orderStatus { get; set; }
        public int userId { get; set; } // Foreign key reference to User
        public string paymentMethod { get; set; }
        public int orderTotal { get; set; }
        public int numberTable { get; set; }

        // Navigation property to link to User object
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
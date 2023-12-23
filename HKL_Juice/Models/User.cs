using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HKL_Juice.Models
{
    public class User
    {
        [Key]
        public int userId { get; set; } // Auto-incremented by the database
        public string userName { get; set; }
        public string userPassword { get; set; }
        public int roleId { get; set; } // Foreign key reference to Role
        public string userFullname { get; set; }
        public string userPhone { get; set; }

        // Navigation property to link to Role object
        public virtual Role Role { get; set; }
    }
}
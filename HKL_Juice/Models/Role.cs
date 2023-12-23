using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HKL_Juice.Models
{
    public class Role
    {
        [Key]
        public int roleId { get; set; } // Auto-incremented by the database
        public string roleName { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
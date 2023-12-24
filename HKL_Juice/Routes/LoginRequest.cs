using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace HKL_Juice.Routes
{
    public class LoginRequest
    {
        public string userName { get; set; }
        public string userPassword { get; set; }
    }
}
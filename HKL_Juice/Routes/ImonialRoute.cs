using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HKL_Juice.Routes
{
    public class ImonialRoute : NancyModule
    {
        public ImonialRoute(ApplicationDbContext dbContext)
        {
            Get("/imonial", parameters =>
            {


                return View["testimonial.cshtml"];
            }
            );
        }
    }
}
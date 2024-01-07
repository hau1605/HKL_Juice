using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Data.Entity;

namespace HKL_Juice.Routes
{
    public class HistoryRoute : NancyModule
    {
        public HistoryRoute(ApplicationDbContext dbContext)
        {
            Get("/history", parameters => {

                return View["historyOrdered.cshtml"];
            });
        }
    }
}
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
            Get("/redirect/{id}", parameters => {
                int numberTable = parameters.id;
                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(numberTable);
                return View["redirect.cshtml", json];
            });
        }
    }
}
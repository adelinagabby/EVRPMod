using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EVRPMod.Models.DB;

namespace EVRPMod.Controllers
{
    public class RoadAccountingController : Controller
    {
        // GET: RoadAccounting
        public ActionResult RoadAccounting()
        {
            return View();
        }
        public ActionResult GetAddressList()
        {
            EVRPModContext db = new EVRPModContext();

            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();

            List<string> Address = new List<string>();

            foreach (var itemCustomerData in CustomerData)
            {
                Address.Add(itemCustomerData.address);
            }
            foreach (var itemDepotData in DepotData)
            {
                Address.Add(itemDepotData.address);
            }

            return Json(Address);
        }
    }
}
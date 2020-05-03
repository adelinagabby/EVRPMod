using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EVRPMod;

namespace EVRPMod.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult VehicleData()
        {
            ViewBag.Message = "Данные о ТС.";

            return View();
        }

        public ActionResult GetVehiceData()
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.VehicleData.ToList();

            return Json(Obj);
        }


        public ActionResult AddVehiceData(string Name, int Capacity, int ServiceCost, int CostRoads)
        {

            EVRPModContext db = new EVRPModContext();


            var Obj = db.VehicleData.FirstOrDefault(x => x.Name == Name && x.Capacity == Capacity && x.ServiceCost == ServiceCost && x.CostRoads == CostRoads);

            string Result;

            if (Obj != null)
            {
                Result = "Данное транспортное средство уже имеется в списке";

                return Json(Result);
            }
            else
            {

                var newObj = new VehicleData
                {
                    Name = Name,
                    Capacity = Capacity,
                    ServiceCost = ServiceCost,
                    CostRoads = CostRoads,
                };


                db.VehicleData.Add(newObj);

                db.SaveChanges();

                Result = "Новый ";

                return Json(Result);
            }
        }
    }
}
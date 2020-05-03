using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EVRPMod.Models.DB;

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
        [HttpPost]
        public ActionResult GetVehiceData()
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.vehicleData4.ToList();

            return Json(Obj);
        }
        [HttpPost]
        public ActionResult GetVehiceDataForId(string id)
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.vehicleData4.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            return Json(Obj);
        }
        public ActionResult AddVehiceData(string Name, int Capacity, int ServiceCost, int CostRoads)
        {

            EVRPModContext db = new EVRPModContext();


            var Obj = db.vehicleData4.FirstOrDefault(x => x.name == Name && x.capacity == Capacity && x.serviceCost == ServiceCost && x.costRoads == CostRoads);

            string Result;

            if (Obj != null)
            {
                Result = "Данное транспортное средство уже имеется в списке";

                return Json(Result);
            }
            else
            {

                var newObj = new vehicleData4
                {
                    //id = (db.vehicleData2.Max(x=>x.id)!=null? db.vehicleData2.Max(x => x.id)+1:1),
                    name = Name,
                    capacity = Capacity,
                    serviceCost = ServiceCost,
                    costRoads = CostRoads,
                   
                };


                db.vehicleData4.Add(newObj);

                db.SaveChanges();

                Result = "Новый ";

                return Json(Result);
            }
        }
        public ActionResult EditVehiceData(string id, string newName, int newCapacity, int newServiceCost, int newCostRoads, string oldName, int oldCapacity, int oldServiceCost, int oldCostRoads)
        {

            EVRPModContext db = new EVRPModContext();

            string Result;

            if (oldName == newName && oldCapacity == newCapacity && oldServiceCost == newServiceCost && oldCostRoads == newCostRoads)
            {

            }
            else
            {
                var Obj = db.vehicleData4.FirstOrDefault(x => x.name == newName && x.capacity == newCapacity && x.serviceCost == newServiceCost && x.costRoads == newCostRoads);

                if (Obj != null)
                {
                    Result = "Данное транспортное средство уже присутствует в списке";

                    return Json(Result);
                }
                else
                {
                    var ObjEdit = db.vehicleData4.FirstOrDefault(x => x.id == Convert.ToInt32(id));
                    ObjEdit.name = newName;
                    ObjEdit.capacity = newCapacity;
                    ObjEdit.serviceCost = newServiceCost;
                    ObjEdit.costRoads = newCostRoads;
                    db.SaveChanges();

                }


            }

            Result = "Данные изменены";

            return Json(Result);
        }
        public ActionResult DeleteVehiceData(string id)
        {
            EVRPModContext db = new EVRPModContext();


            var Obj = db.vehicleData4.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            if ((Obj != null))
            {
                db.vehicleData4.Remove(Obj);
                db.SaveChanges();
            }
            var Result = "Транспортное средство удалено из списка";

            return Json(Result);

        }
    }
}
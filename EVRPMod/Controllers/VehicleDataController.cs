using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EVRPMod.Models.DB;

namespace EVRPMod.Controllers
{
    public class VehicleDataController : Controller
    {
        // GET: VehicleData
        public ActionResult VehicleData()
        {
            ViewBag.Message = "Данные о ТС.";

            return View();
        }
        [HttpPost]
        public ActionResult GetVehiceData()
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.vehicleData.ToList();

            return Json(Obj);
        }
        [HttpPost]
        public ActionResult GetVehiceDataForId(string id)
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.vehicleData.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            return Json(Obj);
        }
        [HttpPost]
        public ActionResult AddVehiceData(string Name, int Capacity, int ServiceCost, int CostRoads)
        {

            EVRPModContext db = new EVRPModContext();


            var Obj = db.vehicleData.FirstOrDefault(x => x.name == Name && x.capacity == Capacity && x.serviceCost == ServiceCost && x.costRoads == CostRoads);

            string Result;

            if (Obj != null)
            {
                Result = "Данное транспортное средство уже имеется в списке";

                return Json(Result);
            }
            else
            {

                var newObj = new vehicleData
                {
                    //id = (db.vehicleData.Max(x=>x.id)!=null? db.vehicleData.Max(x => x.id)+1:1),
                    name = Name,
                    capacity = Capacity,
                    serviceCost = ServiceCost,
                    costRoads = CostRoads,

                };


                db.vehicleData.Add(newObj);

                db.SaveChanges();

                Result = "Новое транспортное средство добавлено";

                return Json(Result);
            }
        }
        [HttpPost]
        public ActionResult EditVehiceData(string id, string newName, int newCapacity, int newServiceCost, int newCostRoads, string oldName, int oldCapacity, int oldServiceCost, int oldCostRoads)
        {

            EVRPModContext db = new EVRPModContext();

            string Result;

            if (oldName == newName && oldCapacity == newCapacity && oldServiceCost == newServiceCost && oldCostRoads == newCostRoads)
            {

            }
            else
            {
                var Obj = db.vehicleData.FirstOrDefault(x => x.name == newName && x.capacity == newCapacity && x.serviceCost == newServiceCost && x.costRoads == newCostRoads);

                if (Obj != null)
                {
                    Result = "Данное транспортное средство уже присутствует в списке";

                    return Json(Result);
                }
                else
                {
                    var ObjEdit = db.vehicleData.FirstOrDefault(x => x.id == Convert.ToInt32(id));
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
        [HttpPost]
        public ActionResult DeleteVehiceData(string id)
        {
            EVRPModContext db = new EVRPModContext();


            var Obj = db.vehicleData.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            if ((Obj != null))
            {
                db.vehicleData.Remove(Obj);
                db.SaveChanges();
            }
            var Result = "Транспортное средство удалено из списка";

            return Json(Result);

        }
    }
}
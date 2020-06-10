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
        public ActionResult AddVehiceData(string Name, string Capacity, string ServiceCost, string CostRoads)
        {
            string Result;

            EVRPModContext db = new EVRPModContext();

            if (Name == "" || Capacity == "" || ServiceCost == "" || CostRoads == "")
            {
                Result = "Ошибка. Не все поля заполнены";
            }

            //var Obj = db.vehicleData.FirstOrDefault(x => x.name == Name && x.capacity == Capacity && x.serviceCost == ServiceCost && x.costRoads == CostRoads);



            //if (Obj != null)
            //{
            //    Result = "Данное транспортное средство уже имеется в списке";

            //    return Json(Result);
            //}
            else
            {

                try
                {
                    var newObj = new vehicleData
                    {
                        //id = (db.vehicleData.Max(x=>x.id)!=null? db.vehicleData.Max(x => x.id)+1:1),
                        name = Name,
                        capacity = string.IsNullOrEmpty(Capacity) ? 0 : float.Parse(Capacity.Replace(".", ",")),
                        serviceCost = string.IsNullOrEmpty(ServiceCost) ? 0 : float.Parse(ServiceCost.Replace(".", ",")),
                        costRoads = string.IsNullOrEmpty(CostRoads) ? 0 : float.Parse(CostRoads.Replace(".", ",")),
                    };


                    db.vehicleData.Add(newObj);

                    db.SaveChanges();


                    return Json("Новое транспортное средство добавлено");
                }
                catch
                {
                    return Json("Ошибка. Введено нечисловое значение");
                }
            }
            return Json(Result);
        }
        [HttpPost]
        public ActionResult EditVehiceData(string id, string newName, string newCapacity, string newServiceCost, string newCostRoads, string oldName, string oldCapacity, string oldServiceCost, string oldCostRoads)
        {

            EVRPModContext db = new EVRPModContext();

            string Result;
            if (newName == "" || newCapacity == "" || newServiceCost == "" || newCostRoads == "")
            {
                Result = "Ошибка. Не все поля заполнены";
            }

            //if (oldName == newName && oldCapacity == newCapacity && oldServiceCost == newServiceCost && oldCostRoads == newCostRoads)
            //{

            //}
            else
            {
                //var Obj = db.vehicleData.FirstOrDefault(x => x.name == newName && x.capacity == newCapacity && x.serviceCost == newServiceCost && x.costRoads == newCostRoads);

                //if (Obj != null)
                //{
                //    Result = "Данное транспортное средство уже присутствует в списке";

                //    return Json(Result);
                //}
                //else
                try
                {
                    var ObjEdit = db.vehicleData.FirstOrDefault(x => x.id == Convert.ToInt32(id));
                    ObjEdit.name = newName;
                    ObjEdit.capacity = string.IsNullOrEmpty(newCapacity) ? 0 : float.Parse(newCapacity.Replace(".", ","));
                    ObjEdit.serviceCost = string.IsNullOrEmpty(newServiceCost) ? 0 : float.Parse(newServiceCost.Replace(".", ","));
                    ObjEdit.costRoads = string.IsNullOrEmpty(newCostRoads) ? 0 : float.Parse(newCostRoads.Replace(".", ","));
                    db.SaveChanges();

                    Result = "Данные изменены";

                }
                catch
                {
                    return Json("Ошибка. Введено нечисловое значение");
                }

            }


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
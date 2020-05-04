using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EVRPMod.Models.DB;

namespace EVRPMod.Controllers
{
    public class VehicleInDepotController : Controller
    {
        // GET: vehicleInDepot
        public ActionResult VehicleInDepot()
        {
            ViewBag.Message = "Данные о транспортных средствах в депо.";

            return View();
        }
        [HttpPost]
        public ActionResult GetVehicleInDepot()
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.vehicleInDepot.ToList();

            return Json(Obj);
        }
        [HttpPost]
        public ActionResult GetVehicleInDepotForId(string id)
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.vehicleInDepot.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            return Json(Obj);
        }
        [HttpPost]
        public ActionResult AddVehicleInDepot(int depotId, int vehicleId, int count)
        {

            EVRPModContext db = new EVRPModContext();


            var Obj = db.vehicleInDepot.FirstOrDefault(x => x.depotId == depotId && x.vehicleId == vehicleId && x.count == count);

            string Result;

            if (Obj != null)
            {
                Result = "Данное транспортное средство в депо уже имеется в списке";

                return Json(Result);
            }
            else
            {

                var newObj = new vehicleInDepot
                {
                    //id = (db.vehicleData.Max(x=>x.id)!=null? db.vehicleData.Max(x => x.id)+1:1),
                    depotId = depotId,
                    vehicleId = vehicleId,
                    count = count,
                };


                db.vehicleInDepot.Add(newObj);

                db.SaveChanges();

                Result = "Новое транспортное средство в депо добавлено";

                return Json(Result);
            }
        }
        [HttpPost]
        public ActionResult EditVehicleInDepot(string id, int newDepotId, int newVehicleId, int newCount, int oldDepotId, int oldVehicleId, int oldCount)
        {

            EVRPModContext db = new EVRPModContext();

            string Result;

            if (oldDepotId == newDepotId && oldVehicleId == newVehicleId && oldCount == newCount)
            {

            }
            else
            {
                var Obj = db.vehicleInDepot.FirstOrDefault(x => x.vehicleId == newVehicleId && x.depotId == newDepotId && x.count == newCount);

                if (Obj != null)
                {
                    Result = "Данное транспортное средство в депо уже присутствует в списке";

                    return Json(Result);
                }
                else
                {
                    var ObjEdit = db.vehicleInDepot.FirstOrDefault(x => x.id == Convert.ToInt32(id));
                    ObjEdit.vehicleId = newVehicleId;
                    ObjEdit.depotId = newDepotId;
                    ObjEdit.count = newCount;
                    db.SaveChanges();

                }


            }

            Result = "Данные изменены";

            return Json(Result);
        }
        [HttpPost]
        public ActionResult DeleteVehicleInDepot(string id)
        {
            EVRPModContext db = new EVRPModContext();


            var Obj = db.vehicleInDepot.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            if ((Obj != null))
            {
                db.vehicleInDepot.Remove(Obj);
                db.SaveChanges();
            }
            var Result = "Транспортное средство в депо удалено из списка";

            return Json(Result);

        }
    }
}
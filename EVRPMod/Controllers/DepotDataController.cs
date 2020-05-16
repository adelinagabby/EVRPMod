using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EVRPMod.Models.DB;

namespace EVRPMod.Controllers
{
    public class DepotDataController : Controller
    {
        // GET: DepotData
        public ActionResult DepotData()
        {
            ViewBag.Message = "Данные о депо.";

            return View();
        }
        [HttpPost]
        public ActionResult GetDepotData()
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.depotData.ToList();

            return Json(Obj);
        }
        [HttpPost]
        public ActionResult GetDepotDataForId(string id)
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.depotData.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            return Json(Obj);
        }
        [HttpPost]
        public ActionResult AddDepotData(string name, string latitude, string longitude, string address)
        {

            EVRPModContext db = new EVRPModContext();


            var Obj = db.depotData.FirstOrDefault(x => x.name == name && x.latitude == latitude && x.longitude == longitude);

            string Result;

            if (Obj != null)
            {
                Result = "Данное депо уже имеется в списке";

                return Json(Result);
            }
            else
            {

                var newObj = new depotData
                {
                    //id = (db.vehicleData.Max(x=>x.id)!=null? db.vehicleData.Max(x => x.id)+1:1),
                    name = name,
                    latitude = latitude,
                    longitude = longitude,
                    address = address,
                };


                db.depotData.Add(newObj);

                db.SaveChanges();

                Result = "Новое депо добавлено";

                AdditionalVariablesAndFunctions.ArrangementOfAddresses();
                AdditionalVariablesAndFunctions.RoadAccountingTablesAreSaved = false;

                return Json(Result);
            }
        }
        [HttpPost]
        public ActionResult EditDepotData(string id, string newName, string newLatitude, 
            string newLongitude, string oldName, string oldLatitude, string oldLongitude, string address)
        {

            EVRPModContext db = new EVRPModContext();

            string Result;

            if (oldName == newName && oldLatitude == newLatitude && oldLongitude == newLongitude)
            {

            }
            else
            {
                var Obj = db.depotData.FirstOrDefault(x => x.name == newName && x.latitude == newLatitude && x.longitude == newLongitude);

                if (Obj != null)
                {
                    Result = "Данное депо уже присутствует в списке";

                    return Json(Result);
                }
                else
                {
                    var ObjEdit = db.depotData.FirstOrDefault(x => x.id == Convert.ToInt32(id));
                    ObjEdit.name = newName;
                    ObjEdit.latitude = newLatitude;
                    ObjEdit.longitude = newLongitude;
                    ObjEdit.address = address;
                    db.SaveChanges();

                }


            }

            Result = "Данные изменены";

            return Json(Result);
        }
        [HttpPost]
        public ActionResult DeleteDepotData(string id)
        {
            EVRPModContext db = new EVRPModContext();


            var Obj = db.depotData.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            if ((Obj != null))
            {
                db.depotData.Remove(Obj);
                db.SaveChanges();
            }
            var Result = "Депо удалено из списка";

            return Json(Result);

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EVRPMod.Models.DB;

namespace EVRPMod.Controllers
{
    public class KitTypeController : Controller
    {
        public ActionResult KitType()
        {
            ViewBag.Message = "Данные о комплектах.";

            return View();
        }
        [HttpPost]
        public ActionResult GetKitType()
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.kitType.ToList();

            return Json(Obj);
        }
        [HttpPost]
        public ActionResult GetKitTypeForId(string id)
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.kitType.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            return Json(Obj);
        }
        [HttpPost]
        public ActionResult AddKitType(string name, int weight)
        {

            EVRPModContext db = new EVRPModContext();


            var Obj = db.kitType.FirstOrDefault(x => x.name == name && x.weight == weight);

            string Result;

            if (Obj != null)
            {
                Result = "Данный комплект уже имеется в списке";

                return Json(Result);
            }
            else
            {

                var newObj = new kitType
                {
                    //id = (db.vehicleData.Max(x=>x.id)!=null? db.vehicleData.Max(x => x.id)+1:1),
                    name = name,
                    weight = weight,
                };


                db.kitType.Add(newObj);

                db.SaveChanges();

                Result = "Новый комплект добавлен";

                return Json(Result);
            }
        }
        [HttpPost]
        public ActionResult EditKitType(string id, string newName, int newWeight, string oldName, int oldWeight)
        {

            EVRPModContext db = new EVRPModContext();

            string Result;

            if (oldName == newName && oldWeight == newWeight)
            {

            }
            else
            {
                var Obj = db.kitType.FirstOrDefault(x => x.name == newName && x.weight == newWeight);

                if (Obj != null)
                {
                    Result = "Данный комплект уже присутствует в списке";

                    return Json(Result);
                }
                else
                {
                    var ObjEdit = db.kitType.FirstOrDefault(x => x.id == Convert.ToInt32(id));
                    ObjEdit.name = newName;
                    ObjEdit.weight = newWeight;
                    db.SaveChanges();

                }


            }

            Result = "Данные изменены";

            return Json(Result);
        }
        [HttpPost]
        public ActionResult DeleteKitType(string id)
        {
            EVRPModContext db = new EVRPModContext();


            var Obj = db.kitType.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            if ((Obj != null))
            {
                db.kitType.Remove(Obj);
                db.SaveChanges();
            }
            var Result = "Комплект удален из списка";

            return Json(Result);

        }
    }
}
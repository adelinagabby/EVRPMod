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
        public ActionResult AddKitType(string name, string weight)
        {


            string Result;
            EVRPModContext db = new EVRPModContext();

            if (name == "" || weight == "")
            {
                Result = "Ошибка. Не все поля заполнены";
            }


            //var Obj = db.kitType.FirstOrDefault(x => x.name == name && x.weight == weight);



            //if (Obj != null)
            //{
            //    Result = "Данный комплект уже имеется в списке";

            //    return Json(Result);
            //}
            else
            {
                try
                {

                    var newObj = new kitType
                    {
                        //id = (db.vehicleData.Max(x=>x.id)!=null? db.vehicleData.Max(x => x.id)+1:1),
                        name = name,
                        weight = string.IsNullOrEmpty(weight) ? 0 : float.Parse(weight.Replace(".", ",")),
                    };


                    db.kitType.Add(newObj);

                    db.SaveChanges();

                    Result = "Новый комплект добавлен";

                    return Json(Result);
                }
                catch
                {
                    return Json("Ошибка. Введено нечисловое значение");
                }
            }
                return Json(Result);
            }

        [HttpPost]
        public ActionResult EditKitType(string id, string newName, string newWeight, string oldName, string oldWeight)
        {

            EVRPModContext db = new EVRPModContext();

            string Result;
            if (newName == "" || newWeight == "" )
            {
                Result = "Ошибка. Не все поля заполнены";
            }
            //if (oldName == newName && oldWeight == newWeight)
            //{

            //}
            else
            {
                //var Obj = db.kitType.FirstOrDefault(x => x.name == newName && x.weight == newWeight);

                //if (Obj != null)
                //{
                //    Result = "Данный комплект уже присутствует в списке";

                //    return Json(Result);
                //}
                //else
                try
                {
                    var ObjEdit = db.kitType.FirstOrDefault(x => x.id == Convert.ToInt32(id));
                    ObjEdit.name = newName;
                    ObjEdit.weight = string.IsNullOrEmpty(newWeight) ? 0 : float.Parse(newWeight.Replace(".", ","));
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
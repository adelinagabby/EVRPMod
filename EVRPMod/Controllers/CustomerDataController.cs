﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EVRPMod.Models.DB;

namespace EVRPMod.Controllers
{
    public class CustomerDataController : Controller
    {
        // GET: CustomerData
        public ActionResult CustomerData()
        {
            ViewBag.Message = "Данные о заказах.";

            return View();
        }
        [HttpPost]
        public ActionResult GetCustomerData()
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.customerData.ToList();

            return Json(Obj);
        }
        [HttpPost]
        public ActionResult GetCustomerDataForId(string id)
        {
            EVRPModContext db = new EVRPModContext();

            var Obj = db.customerData.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            return Json(Obj);
        }
        [HttpPost]
        public ActionResult AddCustomerData(int kitType, string latitude, string longitude, int count)
        {

            EVRPModContext db = new EVRPModContext();


            var Obj = db.customerData.FirstOrDefault(x => x.kitType == kitType && x.latitude == latitude && x.longitude == longitude && x.count == count);

            string Result;

            if (Obj != null)
            {
                Result = "Данный заказ уже имеется в списке";

                return Json(Result);
            }
            else
            {

                var newObj = new customerData
                {
                    //id = (db.vehicleData.Max(x=>x.id)!=null? db.vehicleData.Max(x => x.id)+1:1),
                    kitType = kitType,
                    latitude = latitude,
                    longitude = longitude,
                    count = count,
                };


                db.customerData.Add(newObj);

                db.SaveChanges();

                Result = "Новый заказ добавлен";

                return Json(Result);
            }
        }
        [HttpPost]
        public ActionResult EditCustomerData(string id, int newKitType, string newLatitude, string newLongitude, int newCount, 
            int oldKitType, string oldLatitude, string oldLongitude, int oldCount)
        {

            EVRPModContext db = new EVRPModContext();

            string Result;

            if (oldKitType == newKitType && oldLatitude == newLatitude && oldLongitude == newLongitude && oldCount == newCount)
            {

            }
            else
            {
                var Obj = db.customerData.FirstOrDefault(x => x.kitType == newKitType && x.latitude == newLatitude && x.longitude == newLongitude && x.count == newCount);

                if (Obj != null)
                {
                    Result = "Данный заказ уже присутствует в списке";

                    return Json(Result);
                }
                else
                {
                    var ObjEdit = db.customerData.FirstOrDefault(x => x.id == Convert.ToInt32(id));
                    ObjEdit.kitType = newKitType;
                    ObjEdit.latitude = newLatitude;
                    ObjEdit.longitude = newLongitude;
                    ObjEdit.count = newCount;
                    db.SaveChanges();

                }


            }

            Result = "Данные изменены";

            return Json(Result);
        }
        [HttpPost]
        public ActionResult DeleteCustomerData(string id)
        {
            EVRPModContext db = new EVRPModContext();


            var Obj = db.customerData.FirstOrDefault(x => x.id == Convert.ToInt32(id));

            if ((Obj != null))
            {
                db.customerData.Remove(Obj);
                db.SaveChanges();
            }
            var Result = "Заказ удален из списка";

            return Json(Result);

        }
    }
}
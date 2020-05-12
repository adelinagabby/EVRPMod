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

        struct Coordinate
        {
            public string latitude, longitude;

        }

        struct CoordinateAndCountDepotAndCustomer
        {
            public int countDepot, countCustomer;
            public List<Coordinate> CoordinateDepot, CoordinateCustomer;

        }
        struct CoordinateAndCount
        {
            public int count;
            public List<Coordinate> Coordinate;
        }

        [HttpPost]
        public ActionResult FindingDistancesBetweenCustomersAndDepots()
        {
            EVRPModContext db = new EVRPModContext();

            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();

            List<Coordinate> CoordinateDepot = new List<Coordinate>();
            List<Coordinate> CoordinateCustomer = new List<Coordinate>();
            //for()
            //Coonew Coordinate
            //CoordinateAllAddress.Add(new Coordinate() {latitude = });

            foreach (var item in DepotData)
            {
                CoordinateDepot.Add(new Coordinate() { latitude = item.latitude, longitude = item.latitude });
            }
            foreach (var item in CustomerData)
            {
                CoordinateCustomer.Add(new Coordinate() { latitude = item.latitude, longitude = item.latitude });
            }

            CoordinateAndCountDepotAndCustomer CoordinateAndCount = new CoordinateAndCountDepotAndCustomer() { CoordinateDepot = CoordinateDepot, CoordinateCustomer = CoordinateCustomer, countDepot = DepotData.Count, countCustomer = CustomerData.Count };

            return Json(CoordinateAndCount);
        }

        [HttpPost]
        public ActionResult FindingMatrixsDistancesBetweenCustomersAndDepots()
        {
            EVRPModContext db = new EVRPModContext();

            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();


            List<CoordinateAndCount> MatrixCoordinateAllAddress = new List<CoordinateAndCount>();


            foreach (var itemDepot in DepotData)
            {
                List<CoordinateAndCount> CoordinateAndCount = new List<CoordinateAndCount>();
                List<Coordinate> CoordinateAllAddress = new List<Coordinate>();
                CoordinateAllAddress.Add(new Coordinate() { latitude = itemDepot.latitude, longitude = itemDepot.longitude });
                CustomerData = CustomerData.Where(x => x.depot == itemDepot.id).ToList();
                foreach (var itemCustomer in CustomerData)
                {
                    CoordinateAllAddress.Add(new Coordinate() { latitude = itemCustomer.latitude, longitude = itemCustomer.longitude });
                }
                CoordinateAndCount.Add(new CoordinateAndCount() { count = CustomerData.Count, Coordinate = CoordinateAllAddress });
                MatrixCoordinateAllAddress.Add(new CoordinateAndCount() { count = CustomerData.Count, Coordinate = CoordinateAllAddress });
            }

            return Json(MatrixCoordinateAllAddress);
        }


        public void DistributionOfCustomersByDepot(double[][] distanceFromOrdersToDepot)
        {
            EVRPModContext db = new EVRPModContext();

            var DepotData = db.depotData.ToList();
            var CustomerData = db.customerData.ToList();

            for (int i = 0; i < CustomerData.Count; i++)
            {
                double min = 999999999;
                int minDepot = -1;
                for (int j = 0; j < DepotData.Count; j++)
                {
                    if (min > distanceFromOrdersToDepot[j][i])
                    {
                        min = distanceFromOrdersToDepot[j][i];
                        minDepot = j;
                    }
                }

                CustomerData[i].depot = DepotData[minDepot].id;
            }
        }
        public ActionResult ModificationDistancesBetweenCustomersAndDepots(double[][] distanceFromOrdersToDepot, double[][] MatrixOfEstimatesOfPermittedVelocities,
             double[][] MatrixOfQualityOfRoads, double[][] MatrixOfNumbersOfLights)
        {
            distanceFromOrdersToDepot = MethodKiniRayfa.ModificationOfMatrix(distanceFromOrdersToDepot, MatrixOfEstimatesOfPermittedVelocities, MatrixOfQualityOfRoads, MatrixOfNumbersOfLights);
            return Json(distanceFromOrdersToDepot);
        }

    }
    }
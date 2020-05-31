using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EVRPMod.Models.DB;

namespace EVRPMod
{
    public class MethodKiniRayfa
    {

        //Метод Кини - Райфа
        public struct propertyValues
        {
            public double xmax, xavg075, xavg05, xavg025, xmin;
            public double ymax, yaverage075, yaverage05, yaverage025, ymin;
            public double rating, lambda, avgWeightCriteria;
        }

        public static propertyValues speedRestriction = new propertyValues();
        public static propertyValues roadQuality = new propertyValues();
        public static propertyValues avgPatency = new propertyValues();


        public static void SetPropertyValues() {

            EVRPModContext db = new EVRPModContext();

            var ParametersKiniRayfaMethods = db.parametersKiniRayfaMethods.ToList();
   
            var AverageSpeed = ParametersKiniRayfaMethods.Where(x => x.Criterion == "AverageSpeed").First();
            var RoadQuality = ParametersKiniRayfaMethods.Where(x => x.Criterion == "RoadQuality").First();
            var AverageRoadIntensity = ParametersKiniRayfaMethods.Where(x => x.Criterion == "AverageRoadIntensity").First();

            speedRestriction.xmax = (int) AverageSpeed.HighestValue;
            speedRestriction.xavg075 = (int)AverageSpeed.AverageValueFor__Values75;
            speedRestriction.xavg05 = (int)AverageSpeed.AverageValueFor__Values50;
            speedRestriction.xavg025 = (int)AverageSpeed.AverageValueFor__Values25;
            speedRestriction.xmin = (int)AverageSpeed.SmallestValue;

            speedRestriction.ymax = 1;
            speedRestriction.yaverage075 = 0.75;
            speedRestriction.yaverage05 = 0.5;
            speedRestriction.yaverage025 = 0.25;
            speedRestriction.ymin = 0;

            speedRestriction.rating = (int)AverageSpeed.Rating;
            speedRestriction.avgWeightCriteria = (int)AverageSpeed.ValueFor__WeightComparison__Criteria;


            roadQuality.xmax = (int)RoadQuality.HighestValue;
            roadQuality.xavg075 = (int)RoadQuality.AverageValueFor__Values75;
            roadQuality.xavg05 = (int)RoadQuality.AverageValueFor__Values50;
            roadQuality.xavg025 = (int)RoadQuality.AverageValueFor__Values25;
            roadQuality.xmin = (int)RoadQuality.SmallestValue;

            roadQuality.ymax = 1;
            roadQuality.yaverage075 = 0.75;
            roadQuality.yaverage05 = 0.5;
            roadQuality.yaverage025 = 0.25;
            roadQuality.ymin = 0;

            roadQuality.rating = (int)RoadQuality.Rating;
            roadQuality.avgWeightCriteria = (int)RoadQuality.ValueFor__WeightComparison__Criteria;


            avgPatency.xmax = (int)AverageRoadIntensity.HighestValue;
            avgPatency.xavg075 = (int)AverageRoadIntensity.AverageValueFor__Values75;
            avgPatency.xavg05 = (int)AverageRoadIntensity.AverageValueFor__Values50;
            avgPatency.xavg025 = (int)AverageRoadIntensity.AverageValueFor__Values25;
            avgPatency.xmin = (int)AverageRoadIntensity.SmallestValue;

            avgPatency.ymax = 0;
            avgPatency.yaverage075 = 0.25;
            avgPatency.yaverage05 = 0.5;
            avgPatency.yaverage025 = 0.75;
            avgPatency.ymin = 1;

            avgPatency.rating = (int)AverageRoadIntensity.Rating;
            avgPatency.avgWeightCriteria = (int)AverageRoadIntensity.ValueFor__WeightComparison__Criteria;

        }

        //Оценка скорости
        public static double vSpeed(double x)
        {
            double k = 1, b = 0, y = 0;


            //let speedRestriction = params.oneDimentionalFunctionValue.speedRestriction;
            if (x <= speedRestriction.xmax && x > speedRestriction.xavg075)
            {
                k = (speedRestriction.ymax - speedRestriction.yaverage075) / (speedRestriction.xmax - speedRestriction.xavg075);
                b = (speedRestriction.xmax * speedRestriction.yaverage075 - speedRestriction.ymax * speedRestriction.xavg075) / (speedRestriction.xmax - speedRestriction.xavg075);
            }
            else if (x <= speedRestriction.xavg075 && x > speedRestriction.xavg05)
            {
                k = (speedRestriction.yaverage075 - speedRestriction.yaverage05) / (speedRestriction.xavg075 - speedRestriction.xavg05);
                b = (speedRestriction.xavg075 * speedRestriction.yaverage05 - speedRestriction.yaverage075 * speedRestriction.xavg05) / (speedRestriction.xavg075 - speedRestriction.xavg05);
            }
            else if (x <= speedRestriction.xavg075 && x > speedRestriction.xavg025)
            {
                k = (speedRestriction.yaverage05 - speedRestriction.yaverage025) / (speedRestriction.xavg05 - speedRestriction.xavg025);
                b = (speedRestriction.xavg05 * speedRestriction.yaverage025 - speedRestriction.yaverage05 * speedRestriction.xavg025) / (speedRestriction.xavg05 - speedRestriction.xavg025);
            }
            else if (x <= speedRestriction.xavg025 && x >= speedRestriction.xmin)
            {
                k = (speedRestriction.yaverage025 - speedRestriction.ymin) / (speedRestriction.xavg025 - speedRestriction.xmin);
                b = (speedRestriction.xavg025 * speedRestriction.ymin - speedRestriction.yaverage025 * speedRestriction.xmin) / (speedRestriction.xavg025 - speedRestriction.xmin);
            }


            y = k * x + b;
            return y;
        }

        //Оценка качества дорог
        public static double vQuality(double x)
        {

            double k = 1, b = 0, y = 0;
            //let roadQuality = params.oneDimentionalFunctionValue.roadQuality;
            if (x <= roadQuality.xmax && x > roadQuality.xavg075)
            {
                k = (roadQuality.ymax - roadQuality.yaverage075) / (roadQuality.xmax - roadQuality.xavg075);
                b = (roadQuality.xmax * roadQuality.yaverage075 - roadQuality.ymax * roadQuality.xavg075) / (roadQuality.xmax - roadQuality.xavg075);
            }
            else if (x <= roadQuality.xavg075 && x > roadQuality.xavg05)
            {
                k = (roadQuality.yaverage075 - roadQuality.yaverage05) / (roadQuality.xavg075 - roadQuality.xavg05);
                b = (roadQuality.xavg075 * roadQuality.yaverage05 - roadQuality.yaverage075 * roadQuality.xavg05) / (roadQuality.xavg075 - roadQuality.xavg05);
            }
            else if (x <= roadQuality.xavg05 && x > roadQuality.xavg025)
            {
                k = (roadQuality.yaverage05 - roadQuality.yaverage025) / (roadQuality.xavg05 - roadQuality.xavg025);
                b = (roadQuality.xavg05 * roadQuality.yaverage025 - roadQuality.yaverage05 * roadQuality.xavg025) / (roadQuality.xavg05 - roadQuality.xavg025);
            }
            else if (x <= roadQuality.xavg025 && x >= roadQuality.xmin)
            {
                k = (roadQuality.yaverage025 - roadQuality.xmin) / (roadQuality.xavg025 - roadQuality.xmin);
                b = (roadQuality.xavg025 * roadQuality.xmin - roadQuality.yaverage025 * roadQuality.xmin) / (roadQuality.xavg025 - roadQuality.xmin);
            }

            y = k * x + b;
            return y;
        }

        //Оценка светофоров на дороге
        public static double vNumbersOfLight(double x)
        {

            double k = 1, b = 0, y = 0;

            //let avgPatency = params.oneDimentionalFunctionValue.avgPatency;
            if (x <= avgPatency.xmax && x > avgPatency.xavg075)
            {

                k = (avgPatency.yaverage025 - avgPatency.ymin) / (avgPatency.xavg075 - avgPatency.xmax);
                b = (avgPatency.xavg075 * avgPatency.ymin - avgPatency.yaverage025 * avgPatency.xmax) / (avgPatency.xavg075 - avgPatency.xmax);
            }
            else if (x <= avgPatency.xavg075 && x > avgPatency.xavg05)
            {
                k = (avgPatency.yaverage05 - avgPatency.yaverage025) / (avgPatency.xavg05 - avgPatency.xavg075);
                b = (avgPatency.xavg05 * avgPatency.yaverage025 - avgPatency.yaverage05 * avgPatency.xavg075) / (avgPatency.xavg05 - avgPatency.xavg075);
            }
            else if (x <= avgPatency.xavg05 && x > avgPatency.xavg025)
            {
                k = (avgPatency.yaverage075 - avgPatency.yaverage05) / (avgPatency.xavg025 - avgPatency.xavg05);
                b = (avgPatency.xavg025 * avgPatency.yaverage05 - avgPatency.yaverage075 * avgPatency.xavg05) / (avgPatency.xavg025 - avgPatency.xavg05);
            }
            else if (x <= avgPatency.xavg025 && x >= avgPatency.xmin)
            {
                k = (avgPatency.ymax - avgPatency.yaverage075) / (avgPatency.xmin - avgPatency.xavg025);
                b = (avgPatency.xmin * avgPatency.yaverage075 - avgPatency.ymax * avgPatency.xavg025) / (avgPatency.xmin - avgPatency.xavg025);
            }


            y = k * x + b;
            return y;
        }

        //Оценка дороги
        public static void lambda()
        {
            //let speedRestriction = params.coefficientCriteria.speedRestriction;
            //let avgPatency = params.coefficientCriteria.avgPatency;
            //let roadQuality = params.coefficientCriteria.roadQuality;

            /*if (speedRestriction.rating == 1) {
                speedRestriction.lambda = 1 / (vSpeed(avgPatency.avgWeightCriteria) + vSpeed(roadQuality.avgWeightCriteria) + 1);
                roadQuality.lambda = speedRestriction.lambda * vSpeed(roadQuality.avgWeightCriteria);
                avgPatency.lambda = speedRestriction.lambda * vSpeed(avgPatency.avgWeightCriteria);
            }
            else if (roadQuality.rating == 1) {
                roadQuality.lambda = 1 / (vQuality(speedRestriction.avgWeightCriteria) + vQuality(avgPatency.avgWeightCriteria) + 1);
                speedRestriction.lambda = roadQuality.lambda * vQuality(speedRestriction.avgWeightCriteria);
                avgPatency.lambda = roadQuality.lambda * vQuality(avgPatency.avgWeightCriteria);
            }
            else if (avgPatency.rating == 1) {
                avgPatency.lambda = 1 / (vNumbersOfLight(speedRestriction.avgWeightCriteria) + vNumbersOfLight(avgPatency.avgWeightCriteria) + 1);
                speedRestriction.lambda = avgPatency.lambda * vNumbersOfLight(speedRestriction.avgWeightCriteria);
                roadQuality.lambda = avgPatency.lambda * vNumbersOfLight(roadQuality.avgWeightCriteria);
            }*/
            if (speedRestriction.rating == 1)
            {
                speedRestriction.lambda = 1 / (vNumbersOfLight(avgPatency.avgWeightCriteria) + vQuality(roadQuality.avgWeightCriteria) + 1);
                roadQuality.lambda = speedRestriction.lambda * vQuality(roadQuality.avgWeightCriteria);
                avgPatency.lambda = speedRestriction.lambda * vNumbersOfLight(avgPatency.avgWeightCriteria);
            }
            else if (roadQuality.rating == 1)
            {
                roadQuality.lambda = 1 / (vSpeed(speedRestriction.avgWeightCriteria) + vNumbersOfLight(avgPatency.avgWeightCriteria) + 1);
                speedRestriction.lambda = roadQuality.lambda * vSpeed(speedRestriction.avgWeightCriteria);
                avgPatency.lambda = roadQuality.lambda * vNumbersOfLight(avgPatency.avgWeightCriteria);
            }
            else if (avgPatency.rating == 1)
            {
                avgPatency.lambda = 1 / (vSpeed(speedRestriction.avgWeightCriteria) + vQuality(roadQuality.avgWeightCriteria) + 1);
                speedRestriction.lambda = avgPatency.lambda * vSpeed(speedRestriction.avgWeightCriteria);
                roadQuality.lambda = avgPatency.lambda * vQuality(roadQuality.avgWeightCriteria);
            }

        }

        //Аддитивная оценка дороги
        public static double vAdditiveEstimation(double xSpeed, double xQuality, double xNumbersOfLights)
        {

            return speedRestriction.lambda* vSpeed(xSpeed) + roadQuality.lambda* vQuality(xQuality) + avgPatency.lambda* vNumbersOfLight(xNumbersOfLights);
        }

        //Функция модификации матрицы стоимости
        public static double[][]  ModificationOfMatrix(double[][] Matrix, double[][] MatrixOfEstimatesOfPermittedVelocities, 
            double[][] MatrixOfQualityOfRoads, double[][] MatrixOfNumbersOfLights)
        {

            SetPropertyValues();
            lambda();
            for (int i = 0; i < Matrix.Length; i++)
                for (int j = 0; j < Matrix[0].Length; j++)
                {
                    if (i != j)
                        Matrix[i][j] *= (1 - vAdditiveEstimation(MatrixOfEstimatesOfPermittedVelocities[i][j], MatrixOfQualityOfRoads[i][j], MatrixOfNumbersOfLights[i][j]));
                }
            return Matrix;
        }

    }
}
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
        public static propertyValues avgLightCount = new propertyValues();


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


            avgLightCount.xmax = (int)AverageRoadIntensity.HighestValue;
            avgLightCount.xavg075 = (int)AverageRoadIntensity.AverageValueFor__Values75;
            avgLightCount.xavg05 = (int)AverageRoadIntensity.AverageValueFor__Values50;
            avgLightCount.xavg025 = (int)AverageRoadIntensity.AverageValueFor__Values25;
            avgLightCount.xmin = (int)AverageRoadIntensity.SmallestValue;

            avgLightCount.ymax = 0;
            avgLightCount.yaverage075 = 0.25;
            avgLightCount.yaverage05 = 0.5;
            avgLightCount.yaverage025 = 0.75;
            avgLightCount.ymin = 1;

            avgLightCount.rating = (int)AverageRoadIntensity.Rating;
            avgLightCount.avgWeightCriteria = (int)AverageRoadIntensity.ValueFor__WeightComparison__Criteria;

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

            //let avgLightCount = params.oneDimentionalFunctionValue.avgLightCount;
            if (x <= avgLightCount.xmax && x > avgLightCount.xavg075)
            {

                k = (avgLightCount.yaverage025 - avgLightCount.ymin) / (avgLightCount.xavg075 - avgLightCount.xmax);
                b = (avgLightCount.xavg075 * avgLightCount.ymin - avgLightCount.yaverage025 * avgLightCount.xmax) / (avgLightCount.xavg075 - avgLightCount.xmax);
            }
            else if (x <= avgLightCount.xavg075 && x > avgLightCount.xavg05)
            {
                k = (avgLightCount.yaverage05 - avgLightCount.yaverage025) / (avgLightCount.xavg05 - avgLightCount.xavg075);
                b = (avgLightCount.xavg05 * avgLightCount.yaverage025 - avgLightCount.yaverage05 * avgLightCount.xavg075) / (avgLightCount.xavg05 - avgLightCount.xavg075);
            }
            else if (x <= avgLightCount.xavg05 && x > avgLightCount.xavg025)
            {
                k = (avgLightCount.yaverage075 - avgLightCount.yaverage05) / (avgLightCount.xavg025 - avgLightCount.xavg05);
                b = (avgLightCount.xavg025 * avgLightCount.yaverage05 - avgLightCount.yaverage075 * avgLightCount.xavg05) / (avgLightCount.xavg025 - avgLightCount.xavg05);
            }
            else if (x <= avgLightCount.xavg025 && x >= avgLightCount.xmin)
            {
                k = (avgLightCount.ymax - avgLightCount.yaverage075) / (avgLightCount.xmin - avgLightCount.xavg025);
                b = (avgLightCount.xmin * avgLightCount.yaverage075 - avgLightCount.ymax * avgLightCount.xavg025) / (avgLightCount.xmin - avgLightCount.xavg025);
            }


            y = k * x + b;
            return y;
        }

        //Оценка дороги
        public static void lambda()
        {
            //let speedRestriction = params.coefficientCriteria.speedRestriction;
            //let avgLightCount = params.coefficientCriteria.avgLightCount;
            //let roadQuality = params.coefficientCriteria.roadQuality;

            /*if (speedRestriction.rating == 1) {
                speedRestriction.lambda = 1 / (vSpeed(avgLightCount.avgWeightCriteria) + vSpeed(roadQuality.avgWeightCriteria) + 1);
                roadQuality.lambda = speedRestriction.lambda * vSpeed(roadQuality.avgWeightCriteria);
                avgLightCount.lambda = speedRestriction.lambda * vSpeed(avgLightCount.avgWeightCriteria);
            }
            else if (roadQuality.rating == 1) {
                roadQuality.lambda = 1 / (vQuality(speedRestriction.avgWeightCriteria) + vQuality(avgLightCount.avgWeightCriteria) + 1);
                speedRestriction.lambda = roadQuality.lambda * vQuality(speedRestriction.avgWeightCriteria);
                avgLightCount.lambda = roadQuality.lambda * vQuality(avgLightCount.avgWeightCriteria);
            }
            else if (avgLightCount.rating == 1) {
                avgLightCount.lambda = 1 / (vNumbersOfLight(speedRestriction.avgWeightCriteria) + vNumbersOfLight(avgLightCount.avgWeightCriteria) + 1);
                speedRestriction.lambda = avgLightCount.lambda * vNumbersOfLight(speedRestriction.avgWeightCriteria);
                roadQuality.lambda = avgLightCount.lambda * vNumbersOfLight(roadQuality.avgWeightCriteria);
            }*/
            if (speedRestriction.rating == 1)
            {
                speedRestriction.lambda = 1 / (vNumbersOfLight(avgLightCount.avgWeightCriteria) + vQuality(roadQuality.avgWeightCriteria) + 1);
                roadQuality.lambda = speedRestriction.lambda * vQuality(roadQuality.avgWeightCriteria);
                avgLightCount.lambda = speedRestriction.lambda * vNumbersOfLight(avgLightCount.avgWeightCriteria);
            }
            else if (roadQuality.rating == 1)
            {
                roadQuality.lambda = 1 / (vSpeed(speedRestriction.avgWeightCriteria) + vNumbersOfLight(avgLightCount.avgWeightCriteria) + 1);
                speedRestriction.lambda = roadQuality.lambda * vSpeed(speedRestriction.avgWeightCriteria);
                avgLightCount.lambda = roadQuality.lambda * vNumbersOfLight(avgLightCount.avgWeightCriteria);
            }
            else if (avgLightCount.rating == 1)
            {
                avgLightCount.lambda = 1 / (vSpeed(speedRestriction.avgWeightCriteria) + vQuality(roadQuality.avgWeightCriteria) + 1);
                speedRestriction.lambda = avgLightCount.lambda * vSpeed(speedRestriction.avgWeightCriteria);
                roadQuality.lambda = avgLightCount.lambda * vQuality(roadQuality.avgWeightCriteria);
            }

        }

        //Аддитивная оценка дороги
        public static double vAdditiveEstimation(double xSpeed, double xQuality, double xNumbersOfLights)
        {

            return speedRestriction.lambda* vSpeed(xSpeed) + roadQuality.lambda* vQuality(xQuality) + avgLightCount.lambda* vNumbersOfLight(xNumbersOfLights);
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
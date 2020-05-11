﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVRPMod
{
    public class GeneticAlgoritm
    {


        Random rnd = new Random();
        double infinity = 999999;
        // Генетический алгоритм

        //Функция для подсчета длины пути в генетическом алгоритме
        double CostWayGeneticAlgorithm(double[][] matrixWay, int[] way)
        {
            double costWay = 0;

            for (int i = 0; i < way.Length - 1; i++)
            {
                costWay += matrixWay[way[i]][way[i + 1]];
            }

            return costWay;
        }

        //Создание популяции
        List<int[]> PrimaryPopulation(int sizePopulation, int sizeReshuffle, int firstCity)
        {

            List<int[]> population = new List<int[]>();
            for (int i = 0; i < sizePopulation; i++)
            {
                int[] reshuffle = new int[sizeReshuffle + 1];
                int flag;
                int city;

                reshuffle[0] = reshuffle[sizeReshuffle] = firstCity;
                for (int j = 1; j < sizeReshuffle; j++)
                {
                    flag = 0;
                    while (flag == 0)
                    {

                        //city = Math.floor(Math.random() * sizeReshuffle);
                        city = rnd.Next(sizeReshuffle);
                        if (!reshuffle.Contains(city))
                        {
                            flag = 1;
                            reshuffle[j] = city;
                        }
                    }
                }

                population.Add(reshuffle);
            }
            return population;
        }


        //Кроссинговер
        private List<int[]> Crossing(List<int[]> population)
        {

            List<int> index = new List<int>();
            List<int[]> newPopulation = new List<int[]>();

            int flag;
            int k = 0;
            int l = 0;
            int k2 = 0;
            int l2 = 0;


            for (int i = 0; i < population.Count; i++)//по наборам
            {
                newPopulation.Add(population[i]);
                //for (int j = 0; j < population[0].Length; j++)//по набору
                //{

                //    //newPopulation[i].push(population[i][j]);
                //    newPopulation[i][j] = population[i][j];
                //}
            }
            while ((index.Count != population.Count) && (index.Count != (population.Count - 1)))
            {
                flag = 0;

                while (flag == 0)
                {
                    k = rnd.Next(population.Count);
                    l = rnd.Next(population.Count);

                    if (k != l && !index.Contains(k) && !index.Contains(l))
                    {
                        flag = 1;
                        index.Add(k);
                        index.Add(l);
                    }

                }
                l2 = rnd.Next(1, population[0].Length - 2);
                k2 = rnd.Next(l2, population[0].Length - 1);
                //l2 = Math.floor(Math.random() * ((population[0].Length - 2) - 1)) + 1;
                //k2 = Math.floor(Math.random() * ((population[0].Length - 1) - (l2 + 1))) + (l2 + 1);


                for (int i = l2; i < k2; i++)
                {

                    newPopulation[k][i] = population[l][i];
                    newPopulation[l][i] = population[k][i];
                }
                CheckOnUniqueness(newPopulation[k]);
                CheckOnUniqueness(newPopulation[l]);

            }
            return newPopulation;
        }

        //Проверка на уникальность
        void CheckOnUniqueness(int[] reshuffle)
        {
            int flag;
            int city;

            for (int i = 1; i < reshuffle.Length - 1; i++)
                for (int j = 0; j < i; j++)
                    if (reshuffle[i] == reshuffle[j])
                    {

                        flag = 0;
                        while (flag == 0)
                        {
                            city = rnd.Next(reshuffle.Length - 1);
                            //city = Math.floor(Math.random() * (reshuffle.Length - 1));
                            if (!reshuffle.Contains(city))
                            {
                                reshuffle[i] = city;
                                flag = 1;
                            }
                        }
                    }
            //return reshuffle;
        }


        //Создание новой популяции
        List<int[]> CreatingANewPopulation(List<int[]> population, double[][] matrixWay,int firstCity)
        {
            
            List<int[]> newPopulation = new List<int[]>();
            newPopulation = Crossing(population);
            double[] costWays = new double[population.Count];
            //int costWays = [];

            for (int i = 0; i < population.Count; i++)//по наборам
            {
                costWays[i] = CostWayGeneticAlgorithm(matrixWay, newPopulation[i]);
            }

            //int newPopulation2 = [];

            //for (int i = 0; i < population.Count; i++)//по наборам
            //{
            //    newPopulation2[i] = [];
            //    for (int j = 0; j < population[0].Length; j++)//по набору
            //    {

            //        //newPopulation2[i].push(newPopulation[i][j]);
            //        newPopulation2[i][j] = newPopulation[i][j];
            //    }
            //}
            List<int[]> newPopulation2 = new List<int[]>();
            for (int i = 0; i < population.Count; i++)
            {
                newPopulation2.Add(newPopulation2[i]);
            }
            //Расположим наборы в порядке возрастания F
            newPopulation2 = Sort(costWays, newPopulation, newPopulation2);
            int R;

            int flag = 0;

            //Генерируем новые наборы вместо последних
            for (int i = population.Count / 2; i < population.Count; i++)
            {
                
                int[] newReshuffle = new int[population[0].Length + 1];
                newReshuffle[0] = newReshuffle[population[0].Length - 1] = firstCity;
                for (int j = 1; j < population[0].Length - 1; j++)
                {
                    flag = 0;
                    while (flag == 0)
                    {
                        R = rnd.Next(population[0].Length);
                        if (!newReshuffle.Contains(R))
                        {
                            flag = 1;
                            newReshuffle[j] = R;
                        }
                    }
                }
                newPopulation2[i] = newReshuffle;
            }

            return newPopulation2;
        }

        // Получить  i-ый набор из популяции
        int[] GetSet(List<int[]> population, int i)
        {
            int[] reshuffle = new int[population[0].Length];
            
            for (int j = 0; j < population[0].Length; j++)
                reshuffle[j] = population[i][j];
            return reshuffle;
        }

        //сортировка путей по стоимости
        List<int[]> Sort(double[] costWays, List<int[]> population, List<int[]> newPopulation)
        {

            double tmp;
            for (int i = 1, j; i < costWays.Length; ++i) // цикл проходов, i - номер прохода
            {
                tmp = costWays[i];
                for (j = i - 1; j >= 0 && costWays[j] < tmp; --j) // поиск места элемента в готовой последовательности
                {
                    costWays[j + 1] = costWays[j];    // сдвигаем элемент направо, пока не дошли
                    newPopulation[j + 1] = newPopulation[j];
                }
                costWays[j + 1] = tmp; // место найдено, вставить элемент
                newPopulation[j + 1] = population[i];
            }
            
            List<int[]> newPopulation2 = new List<int[]>();
            for (int j = 0; j < population[0].Length; j++)
            {
                newPopulation2.Add(newPopulation[costWays.Length - j - 1]);
            }
            //for (int i = 0; i < population.Count; i++)
            //{
            //    newPopulation2[i] = []
            //    for (int j = 0; j < population[0].Length; j++)
            //    {
            //        newPopulation2[i][j] = newPopulation[costWays.Length - i - 1][j];
            //    }
            //}
            return newPopulation2;
        }

        List<int[]> Mutation(List<int[]> population)
        {
            int k = 0;
            int l = 0;
            int tmp;
            int flag;


            for (int i = 0; i < population.Count; i++)
            {
                flag = 0;
                while (flag == 0)
                {
                    k = rnd.Next(population[0].Length);
                    l = rnd.Next(population[0].Length);

                    if (k != l)
                    {
                        tmp = population[i][k];
                        population[i][k] = population[i][l];
                        population[i][l] = tmp;
                        flag = 1;
                    }


                }

            }
            return population;
        }
        //Генетический алгоритм
        int[] GeneticAlgorithm(double[][] matrixWay,int sizePopulation, int firstCity)
        {

            int numberIteration = 1000;
           
            int[] bestWay = new int[matrixWay.Length + 1];
            double bestCostWay = infinity;

            List<int[]> population = new List<int[]>();
            List<int[]> newPopulation = new List<int[]>();

            population = PrimaryPopulation(sizePopulation, matrixWay.Length, firstCity);
            int[] tmp;

            for (int i = 0; i < sizePopulation; i++)//по наборам
            {
                //newPopulation.push(GetSet(population, matrixWay.Length + 1, i));
                tmp = GetSet(population, i);
                newPopulation.Add(GetSet(population, i));
            }

            double costWay;

            //Поиск текущего рекорда
            for (int i = 0; i < sizePopulation; i++)//по наборам
            {
                costWay = CostWayGeneticAlgorithm(matrixWay, population[i]);
                if (costWay < bestCostWay)
                {
                    bestCostWay = costWay;
                    //bestWay = population[i];
                    for (int j = 0; j < population[i].Length; j++)//по наборам
                    {
                        bestWay[j] = population[i][j];
                    }
                }
            }


            while (numberIteration > 0)
            {
                newPopulation = CreatingANewPopulation(newPopulation, matrixWay, firstCity);


                for (int i = 0; i < sizePopulation; i++)//по наборам
                {
                    costWay = CostWayGeneticAlgorithm(matrixWay, newPopulation[i]);
                    if (costWay < bestCostWay)
                    {
                        bestCostWay = costWay;
                        //bestWay = GetSet(newPopulation, i);

                        //newPopulation.push(GetSet(population, matrixWay.Length + 1, i));
                        tmp = GetSet(newPopulation, i);
                        for (int j = 0; j < population[i].Length; j++)//по наборам
                        {
                            bestWay[j] = tmp[j];
                        }

                    }
                }
                //if (params.useMutation == true) {
                    for (int i = 0; i < sizePopulation; i++)//по наборам
                    {
                        Mutation(newPopulation);
                    }
               // }

                numberIteration--;
            }


            return bestWay;

        }












    }
}
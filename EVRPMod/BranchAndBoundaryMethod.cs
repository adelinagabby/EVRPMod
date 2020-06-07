using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EVRPMod
{
    public class BranchAndBoundaryMethod
    {
        public static double bestCostWayBranchAndBoundaryMethod = double.MaxValue;

        public static bool flag = false;
        //Метод ветвей и границ
       public static int[] Branch_And_Boundary_Method(double[,] A, int[] I, int[] J, int[] X0)
        {
            if (flag == true)
                return X0;
            else
            {
                int N = A.Length;
                double[,] C = new double[N, N];
                //for (int i = 0; i < N; i++)
                //    C[i] = [];

                int p = 0;

                //Считаем количество городов во множестве I
                for (int i = 0; i < N; i++)
                    if (I[i] != int.MaxValue)
                        p += 1;
                int q = 0;

                //Считаем количество городов во множестве J
                for (int i = 0; i < N; i++)
                    if (J[i] != int.MaxValue)
                        q += 1;

                //Считаем матрицу допустимых переходов
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                        C[i,j] = A[i,j];


                //Запрещаем проезд из городов множества I в города множества J


                for (int j = 0; j < q; j++)
                    C[I[p - 1],J[j]] = double.MaxValue;


                //Если количество городов во множестве I больше 1, то
                //1) Запрещаем проезд из города I[i+1] в город I[i]
                //2) Запрещаем проезд из города I[i] во все города, кроме I[i+1]
                //3) Запрещаем проезд в город I[i+1] для всех, кроме I[i]


                if (p > 1)
                {
                    //2)
                    for (int k = 0; k < p - 1; k++)// по множеству I
                        for (int j = 0; j < N; j++)
                            if (j != I[k + 1])
                                C[I[k],j] = double.MaxValue;

                    //1)
                    //C[I[p - 2], I[p - 1]] = double.MaxValue;

                    //3)
                    for (int k = 1; k < p; k++)// по множеству I
                        for (int j = 0; j < N; j++)
                            if (j != I[k - 1])
                                //if (I[k] != I[k + 1])
                                C[j,I[k]] = double.MaxValue;


                }


                double[] Alpha = new double[A.GetLength(0)];
                double[] Betta = new double[A.GetLength(0)];
                double min;


                for (int i = 0; i < N; i++)
                {
                    if (!I.Contains(i) || (I.Contains(i) && I[i] == I[p - 1]))
                    {

                        min = double.MaxValue;
                        for (int k = 0; k < N; k++)
                            if (C[i,k] < min)
                                min = C[i,k];
                        Alpha[i] = min;
                    }
                    else Alpha[i] = 0;
                }


                for (int k = 0; k < N; k++)
                {
                    if (!I.Contains(k) || (I.Contains(k) && I[k] == I[0]))
                    {
                        min = double.MaxValue;
                        for (int i = 0; i < N; i++)
                            if (!I.Contains(i) || (I.Contains(i) && I[i] == I[p - 1]))
                            {
                                if (C[i,k] - Alpha[i] < min)
                                    min = C[i,k] - Alpha[i];
                            }
                        Betta[k] = min;
                    }
                    else Betta[k] = 0;
                }
                //Вычисление верхней оценки
                double LB = 0;

                for (int i = 0; i < p - 1; i++)
                    //LB += A[i, i + 1];
                    LB += A[I[i],I[i + 1]];

                for (int i = p - 1; i < N; i++)
                    LB += Alpha[i];

                for (int i = p; i < N; i++)
                    LB += Betta[i];
                LB += Betta[0];

                //Вычисление нижней оценки
                double UB;
                int[] Y = new int[N];
                Y = GreedyAlgorithm(C, I, J);
                UB = CostWayBranchAndBoundaryMethod(C, Y);

                if (UB < bestCostWayBranchAndBoundaryMethod)
                {
                    bestCostWayBranchAndBoundaryMethod = UB;
                    for (int i = 0; i < N; i++)
                        X0[i] = Y[i];

                }

                //Если верхняя оценка равна нижней оценки, то конец алгоритма, выводим рекорд
                if (LB == UB)
                {
                    flag = true;
                    return X0;
                }


                if (LB < bestCostWayBranchAndBoundaryMethod)
                {
                    // Осуществляем ветвление D на два подмножества

                    for (int i = 0; i < N; i++)
                    {
                        if (!I.Contains(i) && !J.Contains(i))
                        {

                            int[] I1 = new int[N];
                            int[] J1 = new int[N];
                            int[] J2 = new int[N];

                            for (int j = 0; j < N; j++)
                            {
                                I1[j] = int.MaxValue;
                                J1[j] = int.MaxValue;
                                J2[j] = int.MaxValue;
                            }

                            for (int j = 0; j < N; j++)
                                I1[j] = I[j];

                            I1[p] = i;


                            for (int j = 0; j < N; j++)
                            {
                                J2[j] = J[j];
                            }

                            J2[q] = i;


                            X0 = Branch_And_Boundary_Method(A, I1, J1, X0);
                            X0 = Branch_And_Boundary_Method(A, I, J2, X0);
                            break;
                        }

                    }


                }
                return X0;
            }
        }

        //Функция для подсчета стоимости дороги
        public static double CostWayBranchAndBoundaryMethod(double[,] matrixWay, int[] way)
        {
            double costWay = 0;


            for (int i = 0; i < way.Length - 1; i++)
            {
                costWay += matrixWay[way[i],way[i + 1]];
            }

            costWay += matrixWay[way[way.Length - 1],way[0]];
            return costWay;
        }

        //Жадный алгоритм
        public static int[] GreedyAlgorithm(double [,] matrixWay, int[] I, int[] J)
        {

            int n = matrixWay.GetLength(0);
            int[] Y = new int[n];

            for (int i = 0; i < n; i++)
                Y[i] = int.MaxValue;

            int p = 0;

            for (int i = 0; i < n; i++)
                if (I[i] != int.MaxValue)
                    p += 1;

            int l = 0;
            double min;
            int count = p;

            for (int i = 0; i < p; i++)
            {
                Y[i] = I[i];
            }

            int k = p - 1;

            min = double.MaxValue;

            bool flag = false;

            for (int j = 0; j < n; j++)
            {

                if (min > matrixWay[I[k],j] && !Y.Contains(j) && !J.Contains(j))
                {
                    min = matrixWay[I[k],j];
                    l = j;
                    flag = true;
                }

            }
            if (flag == true)
            {
                Y[count] = l;
                count += 1;
                k = l;
            }


            while (count != n)
            {
                min = double.MaxValue;

                for (int j = 0; j < n; j++)
                {

                    if (min > matrixWay[k,j] && !Y.Contains(j))
                    {
                        min = matrixWay[k,j];
                        l = j;
                    }

                }
                Y[count] = l;
                count += 1;
                k = l;
            }

            return Y;
        }

    }
}
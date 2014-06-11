using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace СравнениеМетодаПотенциалов_СимплексМетода
{
    class Program
    {
        public static void generate_data(ref double[,] A, ref double[] potrebnosti, ref double[] zapasu,int n,int k)   //potr = n   ; zapac= k 
        {
            A = new double[n, k];
            zapasu = new double[n];
            potrebnosti = new double[k];

            Random random = new Random();
            int max_value = 6;
            int all_points = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    A[i, j] = random.Next(max_value*2)+max_value;
                    Console.Write("{0,4}", A[i, j]);
                    zapasu[i] += A[i,j]*max_value;
                }
                all_points += (int)zapasu[i];
                Console.Write("{0,4}", zapasu[i]);
                Console.WriteLine();
            }

            int all = all_points;
            for (int i = 0; i < k-1; i++)
            {
                potrebnosti[i] = all/k;
                all_points -= (int)potrebnosti[i];
                Console.Write("{0,4}", potrebnosti[i]);
            }
            potrebnosti[k - 1] = all_points;
            Console.Write("{0,4}", potrebnosti[k - 1]);

        }

        public static void Time()
        {
            double[,] A = new double[,]   //стоимость перевозок
                        {{0},
                        {0}};

            double[] post = new double[] { 0};   //поставки 
            double[] zapac = new double[] { 0 };   //запасы

            Console.WriteLine("Set process priority REALTIME !");
            Console.ReadLine();
            Console.WriteLine("Go!");

            int maxDimension = 10;  //максимальное колличество измерений
            int tasks = 10; //количество задач на одной размерности

            long[] timeSimlex = new long[maxDimension];
            long[] timeTransport = new long[maxDimension];

            for (int i = 0; i < maxDimension; i++)
                {
                    timeSimlex[i] = 0;
                    timeTransport[i] = 0;
                }

            string path = "E:\\Dropbox\\Visual Studio\\Projects\\Transportation_theory_vs_M_metod\\";    //TODO ! установите свою папку вывода результатов

            for (int i = 2; i < maxDimension; i++)
            {
                        for (int q = 0; q < tasks; q++)
                        {
                            generate_data(ref A, ref post, ref zapac, i, i);
                            Stopwatch swatch = new Stopwatch(); // создаем объект
                            
                                swatch.Start(); // старт
                                Транспортная_задача.calculate_without_printText(A, zapac, post);
                                swatch.Stop(); // стоп
                             
                            timeSimlex[i] += swatch.ElapsedTicks;
                            //Console.WriteLine(swatch.ElapsedTicks); // выводим результат в консоль   
                            timeTransport[i] += swatch.ElapsedTicks;
                            M_metod.Converter(ref A, ref post, ref zapac);

                            M_metod obj = new M_metod(A, post, zapac);
                            Stopwatch swatch1 = new Stopwatch(); // создаем объект
                            try
                            {
                                swatch1.Start();
                                obj.iteration();
                                swatch1.Stop(); // стоп
                            }
                            catch (StackOverflowException r)
                            {
                                Console.WriteLine(r);
                                q = q - 1;
                            }
                            timeSimlex[i] += swatch1.ElapsedTicks;
                            //Console.WriteLine(swatch1.ElapsedTicks); // выводим результат в консоль
                            Console.WriteLine(q*i); 
                        }
            }
            for (int i = 0; i < maxDimension; i++)
            {
                    timeSimlex[i] /=tasks;
                    timeTransport[i] /=tasks;
                }

            StreamWriter fSimplex = new System.IO.StreamWriter(@"" + path + "simple.txt");
            StreamWriter fTransport = new System.IO.StreamWriter(@"" + path + "transport.txt");

            for (int i =2; i < maxDimension; i++)
            {
                fSimplex.WriteLine("{0} ", timeSimlex[i]);
                fTransport.WriteLine("{0} ", timeTransport[i]);
            }
            fSimplex.Close();
            fTransport.Close();

            Console.WriteLine("Finish");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            Time();
        }
    }
}

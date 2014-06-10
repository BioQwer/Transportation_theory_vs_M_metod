using System;
using System.Diagnostics;
using System.IO;

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
            int max_value = 50;
            int all_points = 0;

            for (int i = 0; i < n; i++)
            {
                zapasu[i] = random.Next(n*max_value);
                all_points += (int)zapasu[i];
                for (int j = 0; j < k; j++)
                    A[i, j] = random.Next(max_value);
            }

            for (int i = 0; i < k-1; i++)
            {
                potrebnosti[i]= random.Next(n*max_value);
                all_points -= (int)potrebnosti[i];
            }
            potrebnosti[k - 1] = all_points;
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

            int maxDimension = 23;  //максимальное колличество измерений
            int tasks = 10; //количество задач на одной размерности

            long[,] timeSimlex = new long[maxDimension,maxDimension];
            long[,] timeTransport = new long[maxDimension, maxDimension];

            for (int i = 0; i < maxDimension; i++)
                for (int j = 0; j < maxDimension; j++)
                {
                    timeSimlex[i, j] = 0;
                    timeTransport[i, j] = 0;
                }

            string path = "E:\\Dropbox\\Visual Studio\\Projects\\Transportation_theory_vs_M_metod\\";    //TODO ! установите свою папку вывода результатов

            for (int i = 2; i < maxDimension; i++)
            {
                for (int j = 2; j < maxDimension; j++)
                {
                    if(Math.Abs(i-j)<6)
                        for (int q = 0; q < tasks; q++)
                        {
                            generate_data(ref A, ref post, ref zapac, i, j);
                            Stopwatch swatch = new Stopwatch(); // создаем объект
                            swatch.Start(); // старт
                            Транспортная_задача.calculate_without_printText(A, zapac, post);
                            swatch.Stop(); // стоп
                            timeSimlex[i, j] += swatch.ElapsedTicks;
                            //Console.WriteLine(swatch.ElapsedTicks); // выводим результат в консоль   
                            timeTransport[i, j] += swatch.ElapsedTicks;
                            M_metod.Converter(ref A, ref post, ref zapac);

                            M_metod obj = new M_metod(A, post, zapac);
                            Stopwatch swatch1 = new Stopwatch(); // создаем объект
                            swatch1.Start();
                            obj.iteration();
                            swatch1.Stop(); // стоп
                            timeSimlex[i, j] += swatch1.ElapsedTicks;
                            //Console.WriteLine(swatch1.ElapsedTicks); // выводим результат в консоль
                        }
                }
            }
            for (int i = 0; i < maxDimension; i++)
                for (int j = 0; j < maxDimension; j++)
                {
                    timeSimlex[i, j] /=tasks;
                    timeTransport[i, j] /=tasks;
                }

            StreamWriter fSimplex = new System.IO.StreamWriter(@"" + path + "simple.txt");
            StreamWriter fTransport = new System.IO.StreamWriter(@"" + path + "transport.txt");

            for (int i = 0; i < maxDimension; i++)
            {
                for (int j = 0; j < maxDimension; j++)
                {
                    timeSimlex[i, j] /= tasks;
                    fSimplex.Write("{0} ", timeSimlex[i, j]);
                    timeTransport[i, j] /= tasks;
                    fTransport.Write("{0} ", timeTransport[i, j]);
                }
                fSimplex.Write(Environment.NewLine);
                fTransport.Write(Environment.NewLine);
            }

            Console.WriteLine("Finish");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            Time();
        }
    }
}

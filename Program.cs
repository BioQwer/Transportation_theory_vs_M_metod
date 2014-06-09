using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СравнениеМетодаПотенциалов_СимплексМетода
{
    class Program
    {
        static void Main(string[] args)
        {
            double[,] A = new double[,]   //стоимость перевозок
                        {{2,3,1,2,1},
                        { 7,8,6,3,1},
                        { 3,4,2,7,5}};

            double[] post = new double[] { 30, 30, 30, 20, 40 };   //поставки 
            double[] zapac = new double[] { 50, 50, 50 };   //запасы

            //A = new double[,]   //стоимость перевозок
            //   {{2,3,1,2,1},
            //   { 7,8,6,3,1},
            //   { 3,4,2,7,5}};

            //post = new double[] { 10, 10, 50, 10, 70 };   //поставки
            //zapac = new double[] { 50, 50, 50 };   //запасы

            A = new double[,]   //стоимость перевозок
               {{1,2,5},
               { 3,4,8}};

            post = new double[] { 7, 4, 2 };   //поставки
            zapac = new double[] { 7, 6 };   //запасы

            Транспортная_задача.calculate_without_printText(A, zapac, post);

            M_metod.Converter(ref A,ref post,ref zapac);

            M_metod obj = new M_metod(A,post,zapac);
            obj.iteration();
            Console.ReadLine();

        }
    }
}

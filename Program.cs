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

             A = new double[,]   //стоимость перевозок
               {{2,3,1,2,1},
               { 7,8,6,3,1},
               { 3,4,2,7,5}};

            post = new double[] { 10,10,50,10,70 };   //поставки
            zapac = new double[] { 50, 50, 50 };   //запасы

            A = new double[,]   //стоимость перевозок
               {{1,2,5},
               { 3,4,8}};

            post = new double[] { 7,4,2 };   //поставки
            zapac = new double[] { 7,6 };   //запасы


            Транспортная_задача.calculate(A, zapac, post);

            SimplexNew.Converter(ref A,ref post,ref zapac);
            //A = new double[,]   //стоимость перевозок
            //            {{1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
            //            { 0,0,0,0,0,1,1,1,1,1,0,0,0,0,0},
            //            { 0,0,0,0,0,0,0,0,0,0,1,1,1,1,1},
            //            { 1,0,0,0,0,1,0,0,0,0,1,0,0,0,0},
            //            { 0,1,0,0,0,0,1,0,0,0,0,1,0,0,0}, 
            //            { 0,0,1,0,0,0,0,1,0,0,0,0,1,0,0},
            //            { 0,0,0,1,0,0,0,0,1,0,0,0,0,1,0},
            //            { 0,0,0,0,1,0,0,0,0,1,0,0,0,0,1}};

            //post = new double[] { 2, 3, 1, 2, 1, 7, 8, 6, 3, 1, 3, 4, 2, 7, 5 };   //поставки
            //zapac = new double[] { 50, 50, 50, 30, 30, 30, 20, 40 };   //запасы



            //A = new double[,]   //стоимость перевозок
            //            {{1,1,0,0},
            //            { 0,0,1,1},
            //            { 1,0,1,0},
            //            { 0,1,0,1}};

            //post = new double[] { 1, 2, 3, 4 };   //поставки
            //zapac = new double[] { 5,6,7,4 };   //запасы

            //Транспортная_задача.calculate(A,zapac,post);

            //double[,] A1 = new double[,] 
            //            {{98,7,1},
            //            {34,1,2},
            //            {20,1,1}};
            //double[] c = new double[] { -1, -4 };
            //Simplex o = new Simplex(ref A1,ref c);
            //o.iteration();

            //A = new double[,] 
            //            {{2,1,-2,-1},
            //            {1,3,0,3}};
            //post = new double[] { -1, -2, 3, -1 };  //ограниченния с
            //zapac = new double[] { 5, 2 };  //столбец b

            //A = new double[,] 
            //            {{2,1,3,-1},
            //            {1,3,-1,3}};
            //post = new double[] { -3, 2, -3, -1 };  //ограниченния с
            //zapac = new double[] { 4, 4 };  //столбец b

            SimplexNew obj = new SimplexNew(A,post,zapac);
            obj.iteration();
            Console.ReadLine();

        }
    }
}

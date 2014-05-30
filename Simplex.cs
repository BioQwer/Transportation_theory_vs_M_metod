using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СравнениеМетодаПотенциалов_СимплексМетода
{
    class Simplex
    {
        protected double[,] A;  //матрица ограничений
        public double[] c;     //условия целовой функции
        protected double[] c_basis;   //собственный базис
        protected int[] position_basis;  //позиции базиса
        protected double[] z_fuction;  //значения z функции
        //главный эл-т
        protected int rules_i;
        protected int rules_j;



        public Simplex(ref double[,] A1, ref double[] c1)  //конструктор с созданием переменных класса и тут же заполнение
        {
            int n = A1.GetLength(0);
            int k = A1.GetLength(1);
            A = new double[n, k + n];   //размерность = [строки + столбцы(для базиса); колво строк остоётся таким же(как в исходной)] 
            position_basis = new int[n];
            c_basis = new double[n];
            z_fuction = new double[A.GetLength(1)];
            //заполнение массива с = начальные условия целевой функции
            c = new double[A.GetLength(1)];
            c[0] = 0;
            for (int i = 0; i < A.GetLength(0) - 1; i++)
                c[i + 1] = c1[i];
            for (int i = A.GetLength(0); i < A.GetLength(1); i++)
                c[i] = 0;
               //заполнение A
            for (int i = 0; i < n; i++)
                for (int j = 0; j < k; j++)
                    A[i, j] = A1[i, j];
            for (int i = n-1; i>0; i--)                   //заполнение 2й части матрицы базисом
                for (int j = n+k-1; j > k ; j--)
                    if (j-k==i)
                        A[i, j] = 1;
                    else
                        A[i, j] = 0;
            A[0,k] = 1;    //magic don't touch
        }

        private void c_basis_full()
        {
            find_basis();
            for (int i = 0; i < A.GetLength(0); i++)
                c_basis[i] =  c[position_basis[i]];
        }

        private void z_full()
        {
            for (int i = 0; i < z_fuction.GetLength(0); i++)
                z_fuction[i] = 0;
            for (int i = 0; i < A.GetLength(0); i++)
                for (int j = 0; j < A.GetLength(1); j++)
                    z_fuction[j] = z_fuction[j] + A[i,j] * c_basis[i];
            for (int i = 0; i < A.GetLength(0); i++)
                z_fuction[i] = z_fuction[i] -c[i];
        }

        private void find_basis()
        {
            int flag = 0;
            for (int j = 0; j < A.GetLength(1); j++)   //проход по строкам из за этого сначала по j
            {
                int ed = 0;
                int nol = 0;
                int point_j = 0;
                int point_i = 0;
                for (int i = 0; i < A.GetLength(0); i++)   // и тепереь в каждом столбце ищем базис
                {
                    if ((A[i, j] == 1.0)||(A[i, j] == -1.0))
                    {
                        ed++;
                        point_j = j;    //запомнить какой базис (столбец)
                        point_i=i;
                    }
                    if (A[i, j] == 0.0)
                        nol++;
                    if ((ed == 1) && (nol == A.GetLength(0) - 1))
                    {
                        position_basis[point_i] = point_j;
                        flag++;
                    }
                }
            }
        }

        public void print_simplex()
        {
            Console.WriteLine("                     Simplex table");
            find_basis();
            Console.Write("Position Basis \n");
            for (int i = 0; i < A.GetLength(0); i++)
            {
                Console.WriteLine("{0,8} {1,5}", position_basis[i], c_basis[i]);
            }
            //отображение матрицы
            int cnt =0;
            Console.WriteLine("Коэфициенты при Z функции");
            foreach (int i in c)
            {
                Console.Write(" с[{0,2}]={1,4}", cnt, i);
                cnt++;
            }
            Console.WriteLine("\n");
            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                    Console.Write(" [" + i + "," + j + "]={0,4}", A[i, j]);   //7+4
                if (i < A.GetLength(0) - 1)
                    Console.WriteLine();
            }
            find_basis();

            Console.Write("\nZ_function = \n");
            for (int i = 0; i < A.GetLength(1); i++)
                Console.Write("{0,9:F3}", z_fuction[i]);
            Console.WriteLine("\n");
        }

        private void rulez_element()    //Поиск главного элемента там где z_func ==  max  
        {
            //ищем мах среди этих столбов
            double z_max = 0;
            int z_index = 0;
            for (int i = 0; i < z_fuction.GetLength(0); i++)
                if ((z_fuction[i] > 0) && (z_max <= z_fuction[i]))
                {
                    z_max = z_fuction[i];
                    z_index = i;
                }
            //ищем min в стобце
            double min = A[0, z_index];
            int min_index = 0;
            for (int i = 1; i < A.GetLength(0); i++)
                if (A[i, z_index] >= min)
                {
                    min = A[i, z_index];
                    min_index = i;
                }
            //присваиваем найденое переменным класса
            rules_i = min_index;
            rules_j = z_index;
            ////Вывод для себя  
            //Console.WriteLine("\nRules = [{0,2},{1,2}]", rules_i, rules_j);
        }

        public void jordan_gaus()
        {
            rulez_element();
            double rules = A[rules_i, rules_j];
            for (int j = 0; j < A.GetLength(1); j++)
            {
                A[rules_i, j] = A[rules_i, j] / rules;
            }
            for (int i = 0; i < A.GetLength(0); i++)
            {
                if (i != rules_i)
                {
                    double korrector = A[i, rules_j];
                    for (int j = 0; j < A.GetLength(1); j++)
                    {
                        A[i, j] = A[i, j] - korrector * A[rules_i, j];
                    }
                }
            }
        }

        public void iteration()
        {
            c_basis_full();             //заполняем с_базис
            rulez_element();
            z_full();
            print_simplex();
            
            //первое условие остановки
            //помечаем столбцы где все элементы отрицательны
            for (int i = 0; i < A.GetLength(1); i++)
            {
                int cnt = 0;
                for (int j = 0; j < A.GetLength(0); j++)
                    if (A[j, i] < 0)
                        cnt++;
                if ((A.GetLength(0) - cnt) == 0)
                {
                    Console.WriteLine("Есть отрицательный столбец, решение прекращается");
                    return; 
                }
            }
            //второе условвие остановки
            int otr = 0;
            for (int i = 0; i < z_fuction.GetLength(0); i++)
            {
                if (z_fuction[i] <= 0)
                    otr++;
                if ((z_fuction.GetLength(0) - otr) == 0)
                {
                    Console.WriteLine("Задача решена все z - ci <= 0");
                    Console.WriteLine("Ответ : ");
                    double[] x = new double[A.GetLength(1)-1];
                    for (int j = 0; j < x.GetLength(0); j++)
                        x[j] = 0;
                    for (int j = 0; j < position_basis.GetLength(0); j++)
                        x[position_basis[j]-1] = A[j, 0];

                    for (int j = 0; j < A.GetLength(1)-A.GetLength(0)-1; j++)
                        Console.Write("{0,3}", x[j]);
                    return;
                }
            }
            //последовательные действия алгоритма симплекс метода
            rulez_element();
            jordan_gaus();              //кореектируем матрицу
            z_full();                   //заполняем целевую функцию
            iteration();
        }
    }
}

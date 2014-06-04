using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СравнениеМетодаПотенциалов_СимплексМетода
{
    public class SimplexNew
    {
        protected double[,] A;  //матрица ограничений
        public double[] potrebnosti;     //условия целовой функции
        protected double[] c_basis;   //собственный базис
        protected int[] position_basis;  //позиции базиса
        protected double[] z_fuction;  //значения z функции
        protected double[] z_fuction_M;  //коэфициэнты при M в z функции
        double[] zapasu;  //вектор b правая часть
        //главный эл-т
        protected int rules_i;
        protected int rules_j;
        int M = 200;   //искуственная переменная большая цифра
        double F = 0;   //целевая


        public SimplexNew(double[,] A, double[] potrebnosti, double[] zapasu)  //конструктор с созданием переменных класса и тут же заполнение
        {
            int n = A.GetLength(0);   //строки
            int k = A.GetLength(1);   //столбцы
            this.zapasu = zapasu;
            this.A = new double[n, k + 2*n + 1];   //размерность = [строки + столбцы(для базиса) + 1 для вектора B 
            //колво строк остоётся таким же(как в исходной)] 
            position_basis = new int[2*n];
            c_basis = new double[2*n];
            z_fuction = new double[k + 2*n + 1];
            z_fuction_M = new double[k + 2*n + 1];
            //заполнение массива с = начальные условия целевой функции
            this.potrebnosti = new double[2*n + k];  //строка в симплекс таблице с c1,c2,c3... нумерацию вести с "0"

            for (int i = 0; i < k; i++)
                this.potrebnosti[i] = potrebnosti[i];
            for (int i = k; i < n + k; i++)
                this.potrebnosti[i] = 0;
            for (int i = n + k; i <2*n+k ; i++)
                this.potrebnosti[i] = M;

            //заполнение A
            for (int i = 0; i < n; i++)
                for (int j = 0; j < k; j++)
                    this.A[i, j] = A[i, j];
            for (int j = 0; j < n; j++)
                this.A[j, 2*n + k] = zapasu[j];

            for (int i = n - 1; i > 0; i--)                   //заполнение 2й части матрицы базисом
                for (int j = n + k - 1; j > k; j--)
                    if (j - k == i)
                        this.A[i, j] = -1;
                    else
                        this.A[i, j] = 0;
            this.A[0, k] = -1;    //magic don't touch

            for (int i = n - 1; i > 0; i--)                   //заполнение 2й части матрицы базисом
                for (int j = 2*n + k - 1; j > k+n; j--)
                    if (i==j-n-k)
                        this.A[i, j] = 1;
                    else
                        this.A[i, j] = 0;
            this.A[0, k+n] = 1;    //magic don't touch
        }

        private void c_basis_full()
        {
            find_basis();
            for (int i = 0; i < A.GetLength(0); i++)
                c_basis[i] = potrebnosti[position_basis[i]];
        }

        private bool is_M_Basis_Empty()
        {
            bool result = true;
            for (int i = 0; i < c_basis.Length; i++)
            {
                if (c_basis[i] == M)
                    result = false;
            }
            return result;
        }

        private void z_full()
        {
            int n = z_fuction.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                z_fuction[i] = 0;
                z_fuction_M[i] = 0;
            }

            if (is_M_Basis_Empty())
            {
                for (int i = 0; i < A.GetLength(0); i++)
                {
                    for (int j = 0; j < A.GetLength(1); j++)
                        z_fuction[j] += A[i, j] * c_basis[i];
                }
                for (int i = 0; i < potrebnosti.Length; i++)
                    z_fuction[i] -= potrebnosti[i];
            }
            else //модификация для искуственного базиса
            {
                for (int i = 0; i < A.GetLength(0); i++)
                {
                    for (int j = 0; j < potrebnosti.Length + 1; j++)
                        if (c_basis[i] == M)
                            z_fuction_M[j] += A[i, j];
                        else
                            z_fuction[j] += A[i, j] * c_basis[i];
                }
                for (int i = 0; i < potrebnosti.Length; i++)
                    if (potrebnosti[i] == M)
                        z_fuction_M[i]--;
                    else
                        z_fuction[i] -= potrebnosti[i];
            }
        }

        private void find_basis()
        {
            int flag = 0;
            for (int j = 0; j < A.GetLength(1) - 1; j++)   //проход по строкам из за этого сначала по j   
            //последний столб не считаем так как это правый столбец
            {
                int ed = 0;
                int nol = 0;
                int point_j = 0;
                int point_i = 0;
                for (int i = 0; i < A.GetLength(0); i++)   // и тепереь в каждом столбце ищем базис
                {
                    if ((A[i, j] == 1.0) || (A[i, j] == -1.0))
                    {
                        ed++;
                        point_j = j;    //запомнить какой базис (столбец)
                        point_i = i;
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
            Console.Write("Position Basis\n");
            for (int i = 0; i < A.GetLength(0); i++)
            {
                Console.WriteLine("{0,8} {1,5} ", position_basis[i], c_basis[i]);
            }
            //отображение матрицы
            int cnt = 0;
            foreach (int i in potrebnosti)
            {
                Console.Write(" c[{0,2}]={1,4}", cnt, i);
                cnt++;
            }
            Console.WriteLine("      b  \n");
            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                    Console.Write(" [" + i + "," + j + "]={0,4:F1}", A[i, j]);

                if (i < A.GetLength(0) - 1)
                    Console.WriteLine();
            }

            Console.Write("\nZ_function = \n");
            for (int i = 0; i < A.GetLength(1); i++)
                Console.Write("{0,11:F1}", z_fuction[i]);
            Console.Write("\nZ_function_M = \n");
            for (int i = 0; i < A.GetLength(1); i++)
                Console.Write("{0,11:F1}", z_fuction_M[i]);
            Console.WriteLine();
            Console.ReadKey();  //!!!!
        }

        private void rulez_element()    //Поиск главного элемента там где z_func ==  max  
        {
            rules_i = -1;
            rules_j = -1;
            bool[] polozhit_Z = new bool[z_fuction_M.Length];
            double[] min_A0_to_Aj = new double[z_fuction_M.Length];
            int[] index_min_A0_to_Aj = new int[z_fuction_M.Length];
            if (is_M_Basis_Empty())
            {
                double[,] tao = new double[A.GetLength(0), A.GetLength(1)];
                for (int i = 0; i < tao.GetLength(0); i++)
                {
                    for (int j = 0; j < tao.GetLength(1); j++)
                    {
                        tao[i, j] = (A[i, A.GetLength(1) - 1] / A[i, j]) * z_fuction[j];
                    }
                }
                //for (int i = 0; i < A.GetLength(0); i++)
                //{
                //    for (int j = 0; j < A.GetLength(1) - 1; j++)
                //        Console.Write(" [" + i + "," + j + "]={0,4:F1}", tao[i, j]);

                //    Console.WriteLine();
                //}
                
                for (int i = 0; i < z_fuction.Length - 1; i++)
                {
                    min_A0_to_Aj[i] = Int16.MinValue;
                    if (z_fuction[i] > 0)
                    {
                        polozhit_Z[i] = true;
                        
                    }
                    else
                        polozhit_Z[i] = false;
                }
                

                for (int i = 0; i < z_fuction_M.Length - 1; i++)
                {
                    if (polozhit_Z[i])
                    {
                        double min = Int16.MaxValue;
                        for (int j = 0; j < A.GetLength(0); j++)
                        {
                            if (tao[j, i] > 0 && tao[j, i] < min)
                            {
                                min = tao[j, i];
                                index_min_A0_to_Aj[i] = j;
                            }
                        }
                        min_A0_to_Aj[i] = min;
                    }
                }
                double max = Int16.MinValue;
                for (int i = 0; i < z_fuction_M.Length - 1; i++)
                {
                    if (polozhit_Z[i])
                        if (min_A0_to_Aj[i] > max)
                        {
                            max = min_A0_to_Aj[i];
                            rules_i = index_min_A0_to_Aj[i];
                            rules_j = i;
                        }
                }
            }
            else  //модификация для искуственного базисса
            {
                double[,] tao = new double[A.GetLength(0), A.GetLength(1)];
                for (int i = 0; i < tao.GetLength(0); i++)
                {
                    for (int j = 0; j < tao.GetLength(1); j++)
                    {
                        tao[i, j] = (A[i, A.GetLength(1) - 1] / A[i, j]) * z_fuction_M[j];
                    }
                }
                //for (int i = 0; i < A.GetLength(0); i++)
                //{
                //    for (int j = 0; j < A.GetLength(1) - 1; j++)
                //        Console.Write(" [" + i + "," + j + "]={0,4:F1}", tao[i, j]);

                //    Console.WriteLine();
                //}
                int counter = 0;
                for (int i = 0; i < z_fuction.Length - 1; i++)
                {
                    min_A0_to_Aj[i] = Int16.MaxValue;
                    if (z_fuction_M[i] > 0)
                    {
                        polozhit_Z[i] = true;
                        counter++;
                    }
                    else
                        polozhit_Z[i] = false;
                }
                if (counter == 0)
                {
                    return;
                }
                    
                for (int i = 0; i < z_fuction_M.Length - 1; i++)
                {
                    if (polozhit_Z[i])
                    {
                        double min = Int16.MaxValue;
                        int min_index = 0;
                        for (int j = 0; j < A.GetLength(0); j++)
                        {
                            if (tao[j, i] > 0 && tao[j, i] <= min)
                            {
                                min = tao[j, i];
                                min_index = j;
                            }
                        }
                        min_A0_to_Aj[i] = min;
                        index_min_A0_to_Aj[i] = min_index;
                    }
                }
                //Console.WriteLine("Минимальные элементы");
                //for (int i = 0; i < z_fuction.Length - 1; i++)
                //    if (min_A0_to_Aj[i] != Int16.MaxValue)
                //        Console.Write(" [" + index_min_A0_to_Aj[i] + "  ]={0,4:F1}", min_A0_to_Aj[i]);
                //    else
                //        Console.Write(" [" + index_min_A0_to_Aj[i] + "  ]=max ");

                double max = Int16.MinValue;
                for (int i = 0; i < z_fuction_M.Length - 1; i++)
                {
                    if (polozhit_Z[i])
                        if (min_A0_to_Aj[i] > max)
                        {
                            max = min_A0_to_Aj[i];
                            rules_i = index_min_A0_to_Aj[i];
                            rules_j = i;
                        }
                }
            }
            ////Вывод для себя  
            Console.WriteLine("\nRules = [{0,2},{1,2}] = {2,2}", rules_i, rules_j, A[rules_i, rules_j]);
        }

        public void jordan_gaus()
        {
            rulez_element();
            if (rules_i == -1 && rules_j == -1)
            {
                //Console.WriteLine("ЗАДАЧА НЕСОВМЕСТНА!");
                return;
            }

            double rules = A[rules_i, rules_j];
            for (int j = 0; j < A.GetLength(1); j++)
            {
                A[rules_i, j] = A[rules_i, j] / rules;
                zapasu[rules_i] = zapasu[rules_i] / rules;
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
                if (z_fuction[i] <= 0 && is_M_Basis_Empty())
                    otr++;
                if ((z_fuction.GetLength(0) - otr) == 0)
                {
                    Console.WriteLine("Задача решена все z - ci <= 0");
                    Console.WriteLine("Ответ : ");
                    double[] x = new double[A.GetLength(1) - 1];
                    for (int j = 0; j < x.GetLength(0); j++)
                        x[j] = 0;
                    for (int j = 0; j < position_basis.GetLength(0); j++)
                        x[position_basis[j]] = A[j, A.GetLength(1) - 1];

                    for (int j = 0; j < A.GetLength(1) - A.GetLength(0) - 1; j++)
                        Console.Write("{0,3}", x[j]);

                    F = z_fuction[z_fuction.Length - 1];
                    Console.WriteLine("\nЦелевая = {0,6:F3}", F);
                    return;
                }
            }
            //последовательные действия алгоритма симплекс метода
            jordan_gaus();              //коректируем матрицу
            if (rules_i == -1 && rules_j == -1)
            {
                Console.WriteLine("ЗАДАЧА НЕСОВМЕСТНА!");
                return;
            }
            iteration();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace СравнениеМетодаПотенциалов_СимплексМетода
{
    public static class Транспортная_задача
    {

        public static void calculate(double[,] A, double[] zapasu, double[] potrebnosti)
        {
            int N, M;        //Размерность задачи
            double[] a;                   //адрес массива запасов поставщиков
            double[] b;                        //адрес массива потребностей потребителей
            double[,] C;                  //адресмассива(двумерного) стоимости перевозки
            double[,] X;                 //адрес массива(двумерного) плана доставки
            double[] u;                  //адрес массива потенциалов поставщиков
            double[] v;              //адрес массива потенциалов потребителей
            double[,] D;                 //адрес массива(двумерного) оценок свободных ячеек таблицы
            bool stop;              //признак достижения оптимального плана
            bool[,] T;              //массив будет хранить коодинаты ячеек, в которые уже вписывались
            bool ok = true;           //нулевые поставки при попытках устранить вырожденность плана


            int n = A.GetLength(0);  //кол-во строк таблицы стоимости
            int m = A.GetLength(1);  //кол-во столбцов таблицы стоимости

            //-------------------Проверка на сбалансированность
            double Sa = 0.0;
            double Sb = 0.0;


            for (int i = 0; i < n; i++)          //находим суммарные запасы
                Sa = Sa + zapasu[i];

            for (int i = 0; i < m; i++)          //находим суммарную потребность
                Sb = Sb + potrebnosti[i];

            Console.WriteLine("SAj = {0,3:F3} \\ SBi = {1,3:F3}", Sa, Sb);

            if (Sa == Sb) 
            {
                Console.WriteLine("Транспортная задача - сбалансированность");
            }
            else
            {
                Console.WriteLine("Я не решаю несбалансированные задачи");
                return;
            }

            //----------------------Инициализация динамических массивов:

            //запоминаем размерность в глобальных переменных:
            N = n;              //кол-во строк (поставщиков)
            M = m;              //кол-во столбцов (потребителей)

            //выделение памяти под динамические массивы и их заполнение:
            a = zapasu;        //массив для Запасов

            b = potrebnosti;    //Массив для Потребностей

            //Двумерный массив для Стоимости:
            C = A;

            //Двумерный массив для Доставки:
            X = new double[N + 1, M + 1];//выделяем память под массив адресов начала строк
            /*
             В последней строке(столбце) массива Х будем записывать
             сумму заполненных клеток в соответствующем столбце(строке)
            */
            for (int i = 0; i < N + 1; i++)
                for (int j = 0; j < M + 1; j++)
                {
                    X[i, j] = -1;           //вначале все клетки не заполнены
                    if (i == N) 
                        X[i, j] = 0;        //сумма заполненных клеток в j-м столбце
                    if (j == M) 
                        X[i, j] = 0;        //сумма заполненных клеток в i-й строке
                }


            //-----------------Метод минимального элемента:

            double Sij = 0;
            do
            {
                int im = 0;
                int jm = 0;
                double Cmin = -1;
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < M; j++)
                        if (X[N, j] != b[j])//если не исчерпана Потребность Bj
                            if (X[i, M] != a[i])//если не исчерпан Запас Аі
                                if (X[i, j] < 0)//если клетка ещё не заполнена
                                {
                                    if (Cmin == -1)//если это первая подходящая ячейка
                                    {
                                        Cmin = C[i, j];
                                        im = i;
                                        jm = j;
                                    }
                                    else //если это не первая подходящая ячейка
                                        if (C[i, j] <= Cmin)//если в ячейке меньше,чем уже найдено
                                        {
                                            Cmin = C[i, j];
                                            im = i;
                                            jm = j;
                                        }
                                }

                X[im, jm] = Math.Min(a[im] - X[im, M], b[jm] - X[N, jm]);//выбираем поставку
                X[N, jm] = X[N, jm] + X[im, jm];//добавляем поставку jm-му потребителю
                X[im, M] = X[im, M] + X[im, jm];//добавляем поставку im-му поставщику
                Sij = Sij + X[im, jm]; //Подсчёт суммы добавленых поставок

            } while (Sij < Math.Max(Sa, Sb));//условие продолжения
          
            int L = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                    if (X[i, j] >= 0) L++;          //подсчёт заполненных ячеек
            int d = M + N - 1 - L;                  //если d>0,то задача - вырожденная,придётся добавлять d нулевых поставок
            int d1 = d;                             //запоминаем значение d

            Console.WriteLine("Начальный опорный план:" + Environment.NewLine + Environment.NewLine);

            double F = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                {
                    Console.Write("{0,5}", X[i, j]);
                    if (X[i, j] > 0)
                        F = F + X[i, j] * C[i, j];
                    if (j == M - 1)
                        Console.Write(Environment.NewLine);
                }
            Console.WriteLine("--------");
            Console.WriteLine("F= {0,4}", F);
            
            //--------------------------Метод потенциалов

            T = new bool[N, M];

            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                    T[i, j] = false;

            do
            {//цикл поиска оптимального решения

                stop = true;//признак оптимальности плана(после проверки может стать false)
                u = new double[N];//выделение массивов под значения потециалов
                v = new double[M];
                bool[] ub = new bool[N];//вспомогательные массвы
                bool[] vb = new bool[M];
                for (int i = 0; i < N; i++)
                    ub[i] = false;
                for (int i = 0; i < M; i++)
                    vb[i] = false;


                //цикл расчёта потенциалов (несколько избыточен):
                u[0] = 0;              //значение одного потенциала выбираем произвольно
                ub[0] = true;
                int count = 1;
                int tmp = 0;
                do
                {
                    for (int i = 0; i < N; i++)
                        if (ub[i] == true)
                            for (int j = 0; j < M; j++)
                                if (X[i, j] >= 0)
                                    if (vb[j] == false)
                                    {
                                        v[j] = C[i, j] - u[i];
                                        vb[j] = true;
                                        count++;
                                    }
                    for (int j = 0; j < M; j++)
                        if (vb[j] == true)
                            for (int i = 0; i < N; i++)
                                if (X[i, j] >= 0)
                                    if (ub[i] == false)
                                    {
                                        u[i] = C[i, j] - v[j];
                                        ub[i] = true;
                                        count++;
                                    }


                    tmp++;
                } while ((count < (M + N - d * 2)) && (tmp < M * N));
                Console.WriteLine("--------");


                //цикл добавления нулевых поставок(в случае вырожденности):
                bool t = false;

                if ((d > 0) || ok == false) t = true;//цикл начинается, если d>0
                while (t)//цикл продолжается до тех пор, пока все потенциалы не будут найдены
                {
                    for (int i = 0; (i < N); i++)//просматриваем потенциалы поставщиков
                        if (ub[i] == false)//если среди них не заполненный потенциал
                            for (int j = 0; (j < M); j++)
                                if (vb[j] == true)
                                {
                                    if (d > 0)
                                        if (T[i, j] == false)//если раньше не пытались использовать
                                        {
                                            X[i, j] = 0;        //добавляем нулевую поставку
                                            d--;                //уменьшаем кол-во требуемых добавлений нулевых поставок
                                            T[i, j] = true;     //отмечаем, что эту ячейку уже использовали
                                        }
                                    if (X[i, j] >= 0)
                                    {
                                        u[i] = C[i, j] - v[j];//дозаполняем потенциалы
                                        ub[i] = true;
                                    }
                                }
                    for (int j = 0; (j < M); j++)//просматриваем потенциалы потребителей
                        if (vb[j] == false)//если среди них не заполненный потенциал
                            for (int i = 0; (i < N); i++)
                                if (ub[i] == true)
                                {
                                    if (d > 0)
                                        if (T[i, j] == false)//если раньше не пытались использовать
                                        {
                                            X[i, j] = 0;//добавляем нулевую поставку
                                            d--;//уменьшаем кол-во требуемых добавлений нулевых поставок
                                            T[i, j] = true;//отмечаем, что эту ячейку уже использовали
                                        }
                                    if (X[i, j] >= 0)
                                    {
                                        v[j] = C[i, j] - u[i];//дозаполняем потенциалы
                                        vb[j] = true;
                                    }
                                }
                    t = false; //проверяем, все ли потенциалы найдены
                    for (int i = 0; i < N; i++)
                        if (ub[i] == false) t = true;
                    for (int j = 0; j < M; j++)
                        if (vb[j] == false) t = true;

                    if (t == false)
                    {
                        Console.WriteLine("Опорный план после устранения вырожденности:");
                        Console.WriteLine("");
                        for (int i = 0; i < N; i++)
                            for (int j = 0; j < M; j++)
                            {
                                Console.Write("{0,4}", X[i, j]);
                                if (j == M - 1)
                                    Console.WriteLine(Environment.NewLine);
                            }
                        Console.WriteLine("--------");
                        //ShowMessage("Для продолжения нажмите \"ОК\" ");
                    }

                }
                //-----------

                Console.WriteLine("Потенциалы:");
                Console.WriteLine("u: ");
                for (int i = 0; i < N; i++)
                    Console.WriteLine("{0,3}", u[i]);
                Console.WriteLine(Environment.NewLine + "v: ");
                for (int i = 0; i < M; i++)
                    Console.Write("{0,3}", v[i]);

                Console.WriteLine(Environment.NewLine + "--------");
                //ShowMessage("Для продолжения нажмите \"ОК\" ");

                //----------------------

                D = new double[N, M];//выделяем память под массив оценок свободных ячеек

                //int Dmin=0;
                //int im=-1;
                //int jm=-1;
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < M; j++)
                    {
                        if (X[i, j] >= 0)//если ячейка не свободна
                            D[i, j] = 88;//Заполняем любыми положительными числами
                        else  //если ячейка свободна
                            D[i, j] = C[i, j] - u[i] - v[j];//находим оценку

                        if (D[i, j] < 0)
                        {
                            stop = false;//признак того, что план не оптимальный
                        }

                    }
                //
                Console.WriteLine("Матрица оценок свободных ячеек (если ячейка занята - ставим 88)");
                Console.WriteLine("");
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < M; j++)
                    {
                        Console.Write("{0,4}", D[i, j]);
                        if (j == M - 1)
                            Console.WriteLine("");
                    }
                Console.WriteLine();
                Console.WriteLine("--------");
                //
                if (stop == false)//если план не оптимальный
                {
                    double[,] Y = new double[N, M];//массив для хранения цикла перераспределения поставок

                    double find1, find2;//величина перераспределения поставки для цикла
                    double best1 = 0;//наилучшая оценка улучшения среди всех допустимых перераспределений
                    double best2 = 0;
                    int ib1 = -1;
                    int jb1 = -1;
                    int ib2 = -1;
                    int jb2 = -1;
                    //Ищем наилучший цикл перераспределения поставок:
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < M; j++)
                            if (D[i, j] < 0)//Идём по ВСЕМ ячейкам с отрицательной оценкой
                            {  //и ищем допустимые циклы перераспределения ДЛЯ КАЖДОЙ такой ячейки
                                //Обнуляем матрицу Y:
                                for (int i1 = 0; i1 < N; i1++)
                                    for (int j1 = 0; j1 < M; j1++)
                                        Y[i1, j1] = 0;
                                //Ищем цикл для ячейки с оценкой D[i,j]:
                                find1 = find_gor(i, j, i, j, N, M, X, Y, 0, -1);   //Начинаем идти по горизонтали

                                //Обнуляем матрицу Y:
                                for (int i1 = 0; i1 < N; i1++)
                                    for (int j1 = 0; j1 < M; j1++)
                                        Y[i, j] = 0;

                                find2 = find_ver(i, j, i, j, N, M, X, Y, 0, -1);//Начинаем по вертикали

                                if (find1 > 0)
                                    if (best1 > D[i, j] * find1)
                                    {
                                        best1 = D[i, j] * find1;     //наилучшая оценка
                                        ib1 = i;                   //запомминаем координаты ячейки
                                        jb1 = j;                   //цикл из которой даёт наибольшее улучшение
                                    }
                                if (find2 > 0)
                                    if (best2 > D[i, j] * find2)
                                    {
                                        best2 = D[i, j] * find2; //наилучшая оценка
                                        ib2 = i;              //запомминаем координаты ячейки
                                        jb2 = j;              //цикл из которой даёт наибольшее улучшение
                                    }
                            }
                    if ((best1 == 0) && (best2 == 0))
                    {
                        //stop=true;
                        //ShowMessage("Цикл перераспределения поставок не найден");
                        ok = false;
                        d = d1;//откат назад
                        for (int i = 0; i < N; i++)
                            for (int j = 0; j < M; j++)
                                if (X[i, j] == 0) X[i, j] = -1;
                        continue;
                    }
                    else
                    {   //Обнуляем матрицу Y:
                        for (int i = 0; i < N; i++)
                            for (int j = 0; j < M; j++)
                                Y[i, j] = 0;
                        //возвращаемся к вычислению цикла с наилучшим перераспределением:
                        int ib, jb;
                        if (best1 < best2)
                        {
                            find_gor(ib1, jb1, ib1, jb1, N, M, X, Y, 0, -1);
                            ib = ib1;
                            jb = jb1;
                        }
                        else
                        {
                            find_ver(ib2, jb2, ib2, jb2, N, M, X, Y, 0, -1);
                            ib = ib2;
                            jb = jb2;
                        }
                        for (int i = 0; i < N; i++)
                        {
                            for (int j = 0; j < M; j++)
                            {
                                if ((X[i, j] == 0) && (Y[i, j] < 0))
                                {
                                    stop = true;
                                    ok = false;
                                    d = d1;//откат назад
                                    Console.WriteLine("Попытка отрицательной поставки!");
                                    //ShowMessage("Попытка отрицательной поставки!");
                                    //break;
                                }
                                X[i, j] = X[i, j] + Y[i, j];//перераспределяем поставки
                                if ((i == ib) && (j == jb)) X[i, j] = X[i, j] + 1;//добавляем 1 (т.к. до этого было -1 )
                                if ((Y[i, j] <= 0) && (X[i, j] == 0)) X[i, j] = -1;//если ячейка обнулилась, то выбрасываем её из рассмотрения
                            }
                            //if(stop)break;
                        }
                    }
                    //
                    Console.WriteLine("Матрица цикла перерасчёта:");
                    Console.WriteLine("");
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < M; j++)
                        {
                            Console.Write("{0,4}", Y[i, j]);
                            if (j == M - 1)
                                Console.WriteLine("");
                        }
                    Console.WriteLine("--------");
                    //ShowMessage("Для продолжения нажмите \"ОК\" ");

                    Console.WriteLine("Новый план:");
                    Console.WriteLine("");
                    double F1 = 0;
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < M; j++)
                        {
                            Console.Write("{0,4}", X[i, j]);
                            if (X[i, j] > 0)
                                F1 = F1 + X[i, j] * C[i, j];
                            if (j == M - 1)
                                Console.WriteLine("");
                        }
                    Console.WriteLine("F= {0,4}", (F1));
                    Console.WriteLine("--------");
                    // ShowMessage("Для продолжения нажмите \"ОК\" ");
                    //

                    ok = true;
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < M; j++)
                            T[i, j] = false;

                    //проверка на вырожденность: (?)
                    L = 0;
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < M; j++)
                            if (X[i, j] >= 0) L++;//подсчёт заполненных ячеек
                    d = M + N - 1 - L;//если d>0,то задача - вырожденная
                    d1 = d;
                    if (d > 0) ok = false;

                }
            } while (stop == false);

            Console.WriteLine("Оптимальный план:");
            Console.WriteLine("");
            double Fmin = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                {
                    Console.Write("{0,4}", X[i, j]);
                    if (X[i, j] > 0)
                        Fmin = Fmin + X[i, j] * C[i, j];
                    if (j == M - 1)
                        Console.WriteLine("");
                }
            Console.WriteLine("Fmin= {0,4}", (Fmin));

        }

        //---------------------------------------------------------------------------
        //Функуция поиска ячеек,подлежащих циклу перераспределения (вдоль строки)
        static double find_gor(int i_next, int j_next, int im, int jm, int n, int m, double[,] X, double[,] Y, int odd, double Xmin)
        {
            double rez = -1;
            for (int j = 0; j < m; j++)//идём вдоль строки, на которой стоим
                //ищем заполненную ячейку(кроме той,где стоим) или начальная ячейка(но уже в конце цикла:odd!=0 )
                if (((X[i_next, j] >= 0) && (j != j_next)) || ((j == jm) && (i_next == im) && (odd != 0)))
                {
                    odd++;//номер ячейки в цикле перерасчёта(начало с нуля)
                    double Xmin_old = -1;
                    if ((odd % 2) == 1)//если ячейка нечётная в цикле ( начальная- нулевая )
                    {
                        Xmin_old = Xmin;//Запоминаем значение минимальной поставки в цикле (на случай отката назад)
                        if (Xmin < 0) Xmin = X[i_next, j];//если это первая встреченная ненулевая ячейка
                        else if ((X[i_next, j] < Xmin) && (X[i_next, j] >= 0))
                        {
                            Xmin = X[i_next, j];

                        }
                    }
                    if ((j == jm) && (i_next == im) && ((odd % 2) == 0))//если замкнулся круг и цикл имеет чётное число ячеек
                    {
                        Y[im, jm] = Xmin;//Значение минимальной поставки, на величину которой будем изменять
                        return Xmin;
                    }
                    //если круг еще не замкнулся - переходим к поиску по вертикали:
                    else rez = find_ver(i_next, j, im, jm, n, m, X, Y, odd, Xmin);//рекурсивный вызов
                    if (rez >= 0)//как бы обратный ход рекурсии(в случае если круг замкнулся)
                    {
                        //для каждой ячейки цикла заполняем матрицу перерасчёта поставок:
                        if (odd % 2 == 0) Y[i_next, j] = Y[im, jm];//в чётных узлах прибавляем
                        else Y[i_next, j] = -Y[im, jm];//в нечётных узлах вычитаем
                        break;
                    }
                    else //откат назад в случае неудачи(круг не замкнулся):
                    {
                        odd--;
                        if (Xmin_old >= 0)//если мы изменяли Xmin на этой итерации
                            Xmin = Xmin_old;
                    }
                }

            return rez;//если круг замкнулся (вернулись в исходную за чётное число шагов) -
            // возвращает найденное минимальное значение поставки в нечётных ячейках цикла,
            // если круг не замкнулся, то возвращает -1.
        }
        //-----------------------------------------------------------------------------
        //Функуция поиска ячеек,подлежащих циклу перераспределения (вдоль столбца)
        static double find_ver(int i_next, int j_next, int im, int jm, int n, int m, double[,] X, double[,] Y, int odd, double Xmin)
        {
            double rez = -1;
            int i;
            for (i = 0; i < n; i++)//идём вдоль столбца, на котором стоим
                //ищем заполненную ячейку(кроме той,где стоим) или начальная ячейка(но уже в конце цикла:odd!=0 )
                if (((X[i, j_next] >= 0)) && (i != i_next) || ((j_next == jm) && (i == im) && (odd != 0)))
                {
                    odd++;//номер ячейки в цикле перерасчёта(начало с нуля)
                    double Xmin_old = -1;
                    if ((odd % 2) == 1)//если ячейка нечётная в цикле ( начальная- нулевая )
                    {
                        Xmin_old = Xmin;//Запоминаем значение минимальной поставки в цикле (на случай отката назад)
                        if (Xmin < 0) Xmin = X[i, j_next];//если это первая встреченная ненулевая ячейка
                        else
                            if ((X[i, j_next] < Xmin) && (X[i, j_next] >= 0))
                                Xmin = X[i, j_next];
                    }
                    if ((i == im) && (j_next == jm) && ((odd % 2) == 0))//если замкнулся круг и цикл имеет чётное число ячеек
                    {
                        Y[im, jm] = Xmin;//Значение минимальной поставки, на величину которой будем изменять
                        return Xmin;
                    }
                    //если круг еще не замкнулся - переходим к поиску по горизонтали:
                    else 
                        rez = find_gor(i, j_next, im, jm, n, m, X, Y, odd, Xmin);//- рекурсивный вызов
                    if (rez >= 0)//как бы обратный ход (в случае если круг замкнулся)
                    {
                        //для каждой ячейки цикла заполняем матрицу перерасчёта поставок:
                        if (odd % 2 == 0) Y[i, j_next] = Y[im, jm];//эти прибавляются
                        else Y[i, j_next] = -Y[im, jm];//эти вычитаются
                        break;
                    }
                    else //откат назад в случае неудачи (круг не замкнулся):
                    {
                        odd--;
                        if (Xmin_old >= 0)//если мы изменяли Xmin на этой итерации
                            Xmin = Xmin_old;
                    }
                }

            return rez;
        }

        //static void Main(string[] args)
        //{
        //    double[,] A = new double[,]   //стоимость перевозок
        //                {{2,3,1,2,1},
        //                { 7,8,6,3,1},
        //                { 3,4,2,7,5}};

        //    double[] post = new double[] { 30, 30, 30, 20, 40 };   //поставки
        //    double[] zapac = new double[] { 50, 50, 50 };   //запасы

        //    //double[,] A = new double[,] 
        //    //           {{5,1,4,5,2},
        //    //           { 6,2,1,5,3},
        //    //           {5,3,2,3,6}};

        //    //double[] post = new double[] { 20, 20, 50, 30, 30 };
        //    //double[] zapac = new double[] { 50, 50, 50 };

        //    calculate(A, zapac, post);
        //    Console.ReadLine();
            
        //}
    }
}

//Атомарные операторы
using System;
using System.Collections.Generic;
using System.Threading;
namespace Lab3
{
    class Program5
    {
        static DateTime dt1, dt2;
        static int R = 2; // Параметр N - число писателей
        static int W = 2; // Параметр M - число читателей
        static int n = 1000000; // Параметр NumMessages - количество сообщений
        static string buffer;
        static Thread[] Writers = new Thread[W];
        static Thread[] Readers = new Thread[R];

        //список дял проверки массивов писателей
        //static List<string[]> ResultWri = new List<string[]>();
        //список для проверки массивов читателей
        //static List<List<string>> ResultRea = new List<List<string>>();

        static int intFull = 0;//буфер сначала не наполнен
        static int intEmpty = 1;//буфер сначала пуст

        static bool bEmpty = true;
        static bool finish = false;
        static void Read()
        {
            List<string> MyMessagesRead = new List<string>();//локальный массив читателя
            while (!finish)

                if (Interlocked.CompareExchange(ref intFull, 0, 1) == 1)
                {
                    MyMessagesRead.Add(buffer);
                    intEmpty = 1;//буффер пуст
                }

            //заносим в статический список, чтобы проверить содержимое
            //ResultRea.Add(MyMessagesRead);
        }
        static void Write()
        {
            string[] MyMessagesWri = new string[n];//локальный массив писателя
            for (int j = 0; j < n; j++)
                MyMessagesWri[j] = j.ToString();
            //MyMessagesWri[j] = "Thread WRI #" + Thread.CurrentThread.Name + ", Message: " + j.ToString();//заменить
            int i = 0;
            while (i < n)
                if (Interlocked.CompareExchange(ref intEmpty, 0, 1) == 1)//если пуст
                {
                    buffer = MyMessagesWri[i++];
                    intFull = 1;//буффер полон
                }
            //заносим в статический список, чтобы проверить содержимое
            // ResultWri.Add(MyMessagesWri);
        }
        static void Start()
        {
            dt1 = DateTime.Now;
            for (int i = 0; i < W; i++)
            {
                Writers[i] = new Thread(Write);
                Writers[i].Name = i.ToString();
                Writers[i].Start();
            }
            for (int i = 0; i < R; i++)
            {
                Readers[i] = new Thread(Read);
                Readers[i].Start();
            }
            for (int i = 0; i < W; i++)
                Writers[i].Join();
            finish = true;//завершаем работу читателей
            for (int i = 0; i < R; i++)
                Readers[i].Join();
            dt2 = DateTime.Now;
            Console.WriteLine((dt2 - dt1).TotalMilliseconds);
            /*        int cnt = 0;
                    for (int i = 0; i < ResultWri.Count; i++)
                    {
                            cnt += ResultWri[i].GetLength(0);
                    }
                    Console.WriteLine("Всего сообщений отправлено:{0}", cnt);
                    cnt = 0;
                    for (int i = 0; i < ResultRea.Count; i++)
                    {
                        if (ResultRea[i] != null)
                            cnt+= ResultRea[i].Count;
                    }
                    Console.WriteLine("Получено сообщений: {0}",cnt);*/
            Console.ReadKey();
        }
    }
}
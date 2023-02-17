//Семафоры(как критическая секция)
using System;
using System.Collections.Generic;
using System.Threading;
namespace Lab3
{
    class Program4
    {
        static DateTime dt1, dt2;
        static int R = 6; // Параметр N - число писателей
        static int W = 4; // Параметр M - число читателей
        static int n = 1000000; // Параметр NumMessages - количество сообщений
        static string buffer;
        static Thread[] Writers = new Thread[W];
        static Thread[] Readers = new Thread[R];
        //семафор
        static SemaphoreSlim ssEmpty;

        //список для проверки массивов писателей
        //static List<string[]> ResultWri = new List<string[]>();
        //список для проверки массивов читателей
        //static List<List<string>> ResultRea = new List<List<string>>();

        static bool bEmpty = true;
        static bool finish = false;
        static void Read(object o)
        {
            var ssRead = o as SemaphoreSlim;
            List<string> MyMessagesRead = new List<string>();//локальный массив читателя
            while (!finish)
                if (!bEmpty)
                {
                    ssRead.Wait();
                    if (!bEmpty)
                    {
                        bEmpty = true;
                        MyMessagesRead.Add(buffer);
                    }
                    ssRead.Release();
                }
            //заносим в статический список, чтобы проверить содержимое
            //ResultRea.Add(MyMessagesRead);
        }
        static void Write(object o)
        {
            var ssWrit = o as SemaphoreSlim;
            string[] MyMessagesWri = new string[n];//локальный массив писателя
            for (int j = 0; j < n; j++)
                MyMessagesWri[j] = j.ToString();
            int i = 0;
            while (i < n)
                if (bEmpty)
                {
                    ssWrit.Wait();
                    if (bEmpty)
                    {
                        buffer = MyMessagesWri[i++];
                        bEmpty = false;
                    }
                    ssWrit.Release();
                }
            //заносим в статический список, чтобы проверить содержимое
            //ResultWri.Add(MyMessagesWri);
        }
        static void Start()
        {
            dt1 = DateTime.Now;
            ssEmpty = new SemaphoreSlim(1);//только один запрос может выполняться одновременно
            for (int i = 0; i < W; i++)
            {
                Writers[i] = new Thread(Write);
                Writers[i].Start(ssEmpty);
            }
            for (int i = 0; i < R; i++)
            {
                Readers[i] = new Thread(Read);
                Readers[i].Start(ssEmpty);
            }
            for (int i = 0; i < W; i++)
                Writers[i].Join();
            finish = true;//завершаем работу читателей
            for (int i = 0; i < R; i++)
                Readers[i].Join();
            dt2 = DateTime.Now;
            Console.WriteLine((dt2 - dt1).TotalMilliseconds);
            /*     int cnt = 0;
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
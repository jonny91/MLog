using System;
using MUtils;

namespace ServerLogTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MLog.InitSettings();
            MLog.Log("test log");
            Console.ReadKey();
        }
    }
}
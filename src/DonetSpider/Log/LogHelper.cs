using System;
using System.Collections.Generic;
using System.Text;

namespace DonetSpider.Log
{
    public class LogHelper : ILog
    {
        public void Debugger(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"【Debugger】{DateTime.Now}::{msg}");
        }

        public void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"【Error】{DateTime.Now}::{msg}");
        }

        public void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"【Info】{DateTime.Now}::{msg}");
        }

        public void Waring(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"【Info】{DateTime.Now}::{msg}");
        }
    }
}

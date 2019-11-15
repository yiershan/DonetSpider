using System;
using System.Collections.Generic;
using System.Text;

namespace DonetSpider.Log
{
    public interface ILog
    {
        void Info(string msg);
        void Debugger(string msg);
        void Waring(string msg);
        void Error(string msg);
    }
}

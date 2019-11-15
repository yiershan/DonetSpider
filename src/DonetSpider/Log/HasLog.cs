using System;
using System.Collections.Generic;
using System.Text;

namespace DonetSpider.Log
{
    public class HasLog
    {
        protected ILog _log { get; set; }
        protected void Info(string msg)
        {
            if (_log != null) _log.Info(msg);
        }
        protected void Debugger(string msg)
        {
            if (_log != null) _log.Debugger(msg);
        }
        protected void Error(string msg)
        {
            if (_log != null) _log.Error(msg);
        }
        protected void Waring(string msg)
        {
            if (_log != null) _log.Waring(msg);
        }
    }
}

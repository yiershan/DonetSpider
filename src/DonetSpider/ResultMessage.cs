using System;
using System.Collections.Generic;
using System.Text;

namespace DonetSpider
{
    public class ResultMessage : Dictionary<string, ResultMessage>
    {
        public string Result { get; set; }
    }
}

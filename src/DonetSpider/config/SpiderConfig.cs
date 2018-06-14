using System;
using System.Collections.Generic;
using System.Text;

namespace DonetSpider
{
    public class SpiderConfig
    {
        public Login Login { get; set; }
        /// <summary>
        /// 访问入口
        /// </summary>
        public string MainUrl { get; set; }
        /// <summary>
        /// htttp 请求配置
        /// </summary>
        public HttpConfig HttpConfig { get; set; }
        /// <summary>
        /// 获取内容
        /// </summary>
        public List<SelectQuery> Select { get; set; }
        /// <summary>
        /// 获取下一页的方式
        /// </summary>
        public NextPage NextPage { get; set; }
    }
    public class Login {
        /// <summary>
        /// 登陆页面地址
        /// </summary>
        public string LoginPageUrl { get; set; }
        /// <summary>
        /// 登陆地址
        /// </summary>
        public string LoginUrl { get; set; }
        /// <summary>
        /// 抓取验证码的条件
        /// </summary>
        public SelectQuery ValiCodeQuery { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }

    }
    public class NextPage {
        /// <summary>
        /// 有规律的下一页
        /// ?page=1
        /// </summary>
        public NextPageByParam param { get; set; }
        /// <summary>
        /// 下一页链接到下一页
        /// </summary>
        public NextPageByNext next { get; set; }
    }
    public class NextPageByParam {
        public string Param { get; set; }
        public string Total { get; set; }
    }
    public class NextPageByNext {
        public SelectQuery Select { get; set; }
    }
    public class SelectQuery {
        public HtmlQuery Query { get; set; }
        public List<HtmlSelect> Select { get; set; }
        public string Name { get; set; }

    }
    public class HtmlQuery {
        public String Query { get; set; }
        public HtmlWhere Where { get; set; }
        public HtmlQuery Children { get; set; }
    }
    public class HtmlWhere {
        public String Key { get; set; }
        public String Value { get; set; }
       
    }

    public class HtmlSelect {
        public String Query { get; set; }
        public string Attribute { get; set; }
        public string ResultKey { get; set; }
        public List<SelectQuery> Url { get; set; }
    }

    public class HttpConfig {
        /// <summary>
        /// 访问超时时间
        /// </summary>
        public int? Timeout { get; set; }
    }
}

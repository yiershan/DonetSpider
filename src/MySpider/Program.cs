using DonetSpider;
using DonetSpider.http;
using System;
using System.Collections.Generic;

namespace MySpider
{
    class Program
    {
        static void Main(string[] args)
        {
            DonetSpider.SaveMessage save = SaveMessage;
            SpiderConfig config = new SpiderConfig
            {
                MainUrl = "http://www.dytt8.net/html/gndy/dyzz/index.html",
                HttpConfig = new HttpConfig
                {
                    Timeout = 20000
                },
                Select = new List<SelectQuery> {
                    new SelectQuery{
                        Name = "name",
                        Query = new HtmlQuery {
                            Query = "div.co_content8 table a"
                        },
                        Select = new List<HtmlSelect> {
                            new HtmlSelect {
                                ResultKey = "name",
                                Attribute = "html"
                            },
                            new HtmlSelect {
                                ResultKey = "url",
                                Attribute = "href"
                            },
                            new HtmlSelect {
                                ResultKey = "url",
                                Attribute = "href",
                                Url = new List<SelectQuery> {
                                    new SelectQuery{
                                        Query = new HtmlQuery {
                                            Query = "#Zoom table a",
                                        },
                                        Name = "Details",
                                        Select = new List<HtmlSelect> {
                                            new HtmlSelect {
                                                Attribute = "href",
                                            }

                                        }
                                    },
                                    //new SelectQuery {
                                    //    Query = new HtmlQuery{
                                    //        Query = "#Zoom span",
                                    //    },
                                    //    Select = new List<HtmlSelect> {
                                    //        new HtmlSelect {
                                    //            Attribute = "html",
                                    //        },
                                    //    }
                                        
                                    //},
                                    new SelectQuery {
                                        Query = new HtmlQuery{
                                            Query = "#Zoom span img",
                                        },
                                        Select = new List<HtmlSelect> {
                                            new HtmlSelect {
                                                Attribute = "src",
                                            },
                                        }

                                    }
                                }
                            }
                        }


                    },
                },
                NextPage = new NextPage
                {
                    next = new NextPageByNext
                    {

                    }
                }
            };
            SpiderBase s = new SpiderBase(new HttpHelper(),config,save);
            s.Start();
            Console.WriteLine("完毕");
            Console.ReadLine();
        }
        public static void SaveMessage(ResultMessage message)
        {
            Print(message);
        }

        private static void Print(ResultMessage message)
        {
            Console.Write("……");
            foreach (var d in message)
            {
                if (d.Value.Count > 0)
                {
                    Print(d.Value);
                }
                else
                {
                    Console.WriteLine(string.Format("{0}:{1}", d.Key, d.Value.Result));
                }

            }
        }
    }
}
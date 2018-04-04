using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO;

namespace Skywolf.MarketDataGrabber
{
    public abstract class BaseMarketDataGrabber : IMarketDataGrabber
    {
        public string HttpGet(string url)
        {
            int i = 0;

            while (i < 3)
            {
                try
                {
                    WebClient wc = new WebClient();
                    using (Stream stream = wc.OpenRead(url))
                    {
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (i < 3)
                    {
                        i++;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            return null;
        }

        public HtmlDocument HtmlGet(string url)
        {
            int i = 0;
            while (i < 3)
            {
                try
                {
                    HtmlWeb htmlWeb = new HtmlWeb();
                    HtmlDocument htmlDoc = htmlWeb.Load(url);
                    return htmlDoc;
                }
                catch(Exception ex)
                {
                    if (i < 3)
                    {
                        i++;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            return null;
        }
    }
}

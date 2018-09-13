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
using Skywolf.Contracts.DataContracts.MarketData;

namespace Skywolf.MarketDataGrabber
{
    public abstract class BaseMarketDataGrabber : IMarketDataGrabber
    {
        public string HttpPost(string url, string form, string contenttype, Dictionary<string, string> headers)
        {
            return Post(url, form, contenttype, string.Empty, string.Empty, headers);
        }


        /// <summary>
        /// Name:Panda
        /// POST请求
        /// Date:2016-06-22
        /// </summary>
        /// <param name="Url">接口地址</param>
        /// <param name="Param">接口参数</param>
        /// <param name="ContentType">请求标头--由于多个接口在同一个程序中,标头不一致才写成传递形式,如表头都一致的话,可以写死</param>
        /// <param name="RecCode">发送请求时编码格式--由于多个接口在同一个程序中,发送请求过去的编码不同,所以写成传递形式,如果都一致,可写死</param>
        /// <param name="SendCode">接收返回编码格式--由于多个接口在同一个程序中,得到的返回结果的编码不同,所以写成传递形式,如果都一致,可写死</param>
        /// <returns>返回POST结果</returns>
        protected string Post(string Url, string Param, string ContentType, string RecCode, string SendCode, Dictionary<string, string> headers)
        {
            string responseFromServer = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("" + Url + "");//创建一个请求，把接口地址填进来
            request.Method = "POST";//设置请求的方法属性
            //创建后数据并将其转换为字节数组
            string postData = "" + Param + "";
            byte[] byteArray = null;
            if (RecCode.Equals("UTF8"))
            {
                byteArray = Encoding.UTF8.GetBytes(postData);
            }
            else if (RecCode.Equals("GBK"))
            {
                byteArray = Encoding.UTF8.GetBytes(postData);
            }
            else
            {
                byteArray = Encoding.Default.GetBytes(postData);
            }
            request.ContentType = "" + ContentType + "";//设置请求标头
            request.ContentLength = byteArray.Length;//请求长度
            
            if (headers != null)
            {

                foreach (var pair in headers)
                {
                    if (WebHeaderCollection.IsRestricted(pair.Key))
                    {
                        switch (pair.Key)
                        {
                            case "Host": { request.Host = pair.Value; break; }
                            case "Accept": { request.Accept = pair.Value; break; }
                            case "Referer": { request.Referer = pair.Value; break; }
                            case "User-Agent": { request.UserAgent = pair.Value; break; }
                        }
                    }
                    else
                    {
                        request.Headers.Add(pair.Key, pair.Value);
                    }
                }
            }

            Stream dataStream = request.GetRequestStream();//得到请求流
            dataStream.Write(byteArray, 0, byteArray.Length);//将数据写入请求流
            dataStream.Close();//关闭流对象

            WebResponse response = request.GetResponse();//得到的响应
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);//显示状态
            dataStream = response.GetResponseStream();//获取服务器返回的流
            //StreamReader php = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
            StreamReader reader = null;//;new StreamReader(dataStream);//打开流
            if (SendCode.Equals("UTF8"))
            {
                reader = new StreamReader(dataStream, Encoding.GetEncoding("UTF-8"));//打开流
            }
            else if (SendCode.Equals("GBK"))
            {
                reader = new StreamReader(dataStream, Encoding.GetEncoding("GBK"));//打开流
            }
            else
            {
                reader = new StreamReader(dataStream, Encoding.Default);
            }

            responseFromServer = reader.ReadToEnd();//读取内容            

            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

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

        public abstract TimeSeriesDataOutput GetTimeSeriesData(TimeSeriesDataInput input);
        public abstract Quote[] StockBatchQuote(IEnumerable<string> symbols);
    }
}

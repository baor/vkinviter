using System;
using System.IO;
using System.Net;
using System.Text;

namespace vkinviter
{
    public static class HttpConnector
    {
        public const string VKCOM = "vk.com";
        public static string HTTPVKCOM = "http://vk.com/";

        public static TimeSpan TIMEOUT = new TimeSpan(0, 0, 3); //3 sec

        public static CookieContainer HttpCookie;
        public static HttpWebResponseEx SendHttpWebRequestAndGetResponse(
            string url, 
            HttpMethod method, 
            string host = VKCOM, 
            string referer = "", 
            string requestBody = "", 
            bool withoutLogging = false)
        {
            if (withoutLogging)
                Logger.LogMethod();
            else
                Logger.LogMethod(url, method, host, referer, requestBody, withoutLogging);

            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(url);

            httpWReq.Method = method.ToString();
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            httpWReq.Host = host;
            httpWReq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:9.0.1) Gecko/20100101 Firefox/9.0.1";
            if (referer != "")
                httpWReq.Referer = referer;
            httpWReq.KeepAlive = true;
            httpWReq.AllowAutoRedirect = false;

            httpWReq.CookieContainer = HttpCookie;

            httpWReq.Headers.Add("Accept-Language", "ru-ru,ru;q=0.8,en-us;q=0.5,en;q=0.3");
            httpWReq.Headers.Add("Accept-Charset", "windows-1251,utf-8;q=0.7,*;q=0.7");
            httpWReq.Headers.Add("DNT", "1");

            Logger.AddText("Cookie=={0}", HttpCookie.CookieContainerToString());
            if (method == HttpMethod.POST)
            {
                if (requestBody == "")
                    throw new System.ArgumentException("When method is POST, parameter cannot be null", "requestBody");
                
                byte[] data = Encoding.ASCII.GetBytes(requestBody);
                httpWReq.ContentLength = data.Length;

                using (Stream stream = httpWReq.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            HttpWebResponseEx responseEx = new HttpWebResponseEx();
            responseEx.HttpWebResponse = (HttpWebResponse)httpWReq.GetResponse();
            if (!withoutLogging)
                Logger.AddText("Response={0}", responseEx.ToString());
            return responseEx;
        }
    }

    public enum HttpMethod
    {
        POST,
        GET
    }
}

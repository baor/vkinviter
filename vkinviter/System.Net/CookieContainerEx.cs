using System;
using System.Net;

namespace vkinviter
{
    public static class CookieContainerEx
    {
        public static string CookieContainerToString(this CookieContainer cookieContainer)
        {
            string output = string.Empty;
            if (cookieContainer != null)
            {
                CookieCollection cookieCollection = cookieContainer.GetCookies(new Uri(HttpConnector.HTTPVKCOM));
                foreach (Cookie cookie in cookieCollection)
                {
                    output += cookie.ToString();
                }
            }
            return output;
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vkinviter;
using System.Net;

namespace UnitTestProject
{
    [TestClass]
    public class HttpConnectorTest
    {
        [TestMethod]
        public void TestHttpconnection()
        {
            string url = "http://m.vk.com/";
            HttpWebResponseEx answer = HttpConnector.SendHttpWebRequestAndGetResponse(url, HttpMethod.GET);
            Assert.IsTrue(answer.HttpWebResponse.StatusCode == HttpStatusCode.OK);
        }
      
    }
}

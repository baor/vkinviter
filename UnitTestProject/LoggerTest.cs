using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vkinviter;
using System.IO;
using System.Text;

namespace UnitTestProject
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void AddTextToLog()
        {
            string line = "testtesttest";
            string logfilePath = Logger.OpenLogFile();
            string writtenLine = Logger.AddText(line);
            Logger.CloseLogFile();
            Assert.IsTrue(writtenLine.Contains(line));

            FileInfo logFile = new FileInfo(logfilePath);
            StreamReader streamReader = logFile.OpenText();
            string actualLine = streamReader.ReadLine();
            Assert.AreEqual(writtenLine, actualLine);

            streamReader.Close();
            logFile.Delete();
        }

        [TestMethod]
        public void CustomFolderForLogs()
        {
            string line = "testtesttest";
            string path = "c:\\temp";
            string logfilePath = Logger.OpenLogFile(path);
            string writtenLine = Logger.AddText(line);
            Logger.CloseLogFile();
            Assert.IsTrue(logfilePath.Contains(path));

            FileInfo logFile = new FileInfo(logfilePath);
            StreamReader streamReader = logFile.OpenText();
            string actualLine = streamReader.ReadLine();
            Assert.AreEqual(writtenLine, actualLine);

            streamReader.Close();
            logFile.Delete();
        }

        [TestMethod]
        public void AddTwoLines()
        {
            string line = "testtesttest";
            string logfilePath = Logger.OpenLogFile();
            string writtenLine1 = Logger.AddText(line);
            string writtenLine2 = Logger.AddText(line);
            Logger.CloseLogFile();

            FileInfo logFile = new FileInfo(logfilePath);
            StreamReader streamReader = logFile.OpenText();
            string actualLine = streamReader.ReadLine();
            Assert.AreEqual(writtenLine1, actualLine);
            actualLine = streamReader.ReadLine();
            Assert.AreEqual(writtenLine2, actualLine);

            streamReader.Close();
            logFile.Delete();
        }
    }
}

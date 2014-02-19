using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SqlServer.Management.Smo;
using vkinviter;


namespace UnitTestProject
{
    [TestClass]
    public class DatabaseConnectorTest
    {
        [TestMethod]
        public void TestServerConnection()
        {
            string ver = DatabaseConnector.ServerConnect();
            Assert.AreEqual("11.0.3128", ver);
        }

        [TestMethod]
        public void TestDbCreation()
        {
            string dbName = "MyTempDb";
            DatabaseConnector.ServerConnect();
            DatabaseConnector.CreateDatabase(dbName);
            DatabaseConnector.DropDatabase(dbName);
        }
        [TestMethod]
        public void TestTableCreation()
        {
            string dbName = "MyTempDb";
            string tableName = "myTempTable";
            List<DcColumn> colums = new List<DcColumn>
            {
                new DcColumn {Name = "Name", DataType = DataType.NChar(50)},
                new DcColumn {Name = "ID", DataType = DataType.Int}
            };

            DatabaseConnector.ServerConnect();
            DatabaseConnector.CreateDatabase(dbName);
            DatabaseConnector.CreateTable(dbName, tableName, colums);
            DatabaseConnector.DropDatabase(dbName);
        }
    }
}

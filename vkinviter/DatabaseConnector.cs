using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System;
using System.Collections.Generic;

namespace vkinviter
{    
    public static class DatabaseConnector
    {
        private static Server _server;

        public static string ServerConnect(string serverInstanceName = "(local)\\SQLEXPRESS")
        {
            _server = new Server(serverInstanceName);
            return _server.Information.Version.ToString();
        }
        public static void CreateDatabase(string dbName)
        {
            (new Database(_server, dbName)).Create();
        }

        public static void DropDatabase(string dbName)
        {
            _server.Databases[dbName].Drop();
        }

        public static void CreateTable(string dbName, string tableName, List<DcColumn> columns )
        {
            Database db = _server.Databases[dbName];

            Table tb = new Table(db, tableName);
            foreach(DcColumn col in columns)
            {
                if (col.Name == null)
                    throw new ArgumentNullException("Name");
                if (col.DataType == null)
                    throw new ArgumentNullException("DataType");

                tb.Columns.Add(new Column(tb, col.Name, col.DataType));
            }
            tb.Create();
        }
    }

    public class DcColumn
    {
        public string Name;
        public DataType DataType;
    }
}

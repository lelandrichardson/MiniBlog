using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Configuration;
using StackExchange.Profiling;

namespace MiniBlog.Includes.Data
{
    public static class Db
    {
        private const string DefaultDatabase = "Default";

        public static IDbConnection GetConnection(string db = DefaultDatabase)
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings[db].ConnectionString).WithProfiler();
        }

        public static IDbConnection GetOpenConnection(string db = DefaultDatabase)
        {
            var connection = GetConnection(db);
            connection.Open();
            return connection;
        }

        private static IDbConnection WithProfiler(this DbConnection conn)
        {
            return new StackExchange.Profiling.Data.ProfiledDbConnection(conn, MiniProfiler.Current);
        }
    }
}

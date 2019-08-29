using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace sstocker.core.Helpers
{
    public static class DatabaseHelper
    {
        public static bool TryConnection()
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static T QueryFirstOrDefault<T>(string sql, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.QueryFirstOrDefault<T>(sql, parameters);
            }
        }

        public static List<T> Query<T>(string sql, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.Query<T>(sql, parameters).ToList();
            }
        }

        public static int Execute(string sql, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return connection.Execute(sql, parameters);
            }
        }

        private static IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(ConfigurationHelper.GetConfiguration("Database:connectionstring"));
            return connection;
        }
    }
}

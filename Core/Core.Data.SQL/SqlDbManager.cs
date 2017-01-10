using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.SQL
{
    public class SqlDbManager : IDbManager
    {
        private Dictionary<DBContext, string> connectionStringLookup = new Dictionary<DBContext, string>();

        public SqlDbManager()
        {

            foreach (var dbContext in Enum.GetValues(typeof(DBContext)))
            {
                if (ConfigurationManager.ConnectionStrings[dbContext.ToString()] != null)
                {
                    connectionStringLookup.Add((DBContext)Enum.Parse(typeof(DBContext), dbContext.ToString()),
                    ConfigurationManager.ConnectionStrings[dbContext.ToString()].ConnectionString);
                }
            }
        }

        public SqlConnection CreateSQLConnection(DBContext dbContext)
        {
            IDbConnection dbConnection = new SqlConnection(connectionStringLookup[dbContext]);
            return dbConnection as SqlConnection;
        }
    }
}

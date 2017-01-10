using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Core.Logging;

namespace Core.Data.SQL
{
    public class DapperSQLRepository : IRepository
    {
        private IDbManager dbManager;
        private ILogger logger;
        private DBContext dbContext;
        private bool logDebugInfo;
        private int commandTimeout = 120;

        public DapperSQLRepository(DBContext context)
        {
            this.dbManager = new SqlDbManager();
            this.logger = new DatabaseLogger(this.GetType().ToString());
            this.dbContext = context;

            if (ConfigurationManager.AppSettings["DapperCommandTimeout"] != null)
            {
                this.commandTimeout = Convert.ToInt16(ConfigurationManager.AppSettings["DapperCommandTimeout"]);
            }

            if (ConfigurationManager.AppSettings["RepositoryLogDebugInfo"] != null)
            {
                this.logDebugInfo = Convert.ToBoolean(ConfigurationManager.AppSettings["RepositoryLogDebugInfo"]);
            }
        }

        public void ExecuteNonQuery(string query, dynamic param = null, CommandType? commandType = null)
        {
            this.logger.CatchAndLog("ExecuteNonQuery", () =>
            {
                LogDebugInfo(query, param, commandType);

                using (SqlConnection connection = this.dbManager.CreateSQLConnection(this.dbContext))
                {
                    SqlMapper.Execute(connection, query, param, null, this.commandTimeout, commandType);
                }
            });
        }

        public IEnumerable<T> Query<T>(string query, dynamic param = null, CommandType? commandType = null)
        {
            return this.logger.CatchAndLog<IEnumerable<T>>("Query", () =>
            {
                LogDebugInfo(query, param, commandType);

                using (SqlConnection connection = this.dbManager.CreateSQLConnection(this.dbContext))
                {
                    var result = SqlMapper.Query<T>(connection, query, param, null, true, this.commandTimeout, commandType);
                    return result;
                }
            });
        }

        public T QueryMultiple<T>(string query, dynamic param = null, CommandType? commandType = null) where T : class
        {
            return this.logger.CatchAndLog<T>("Query", () =>
            {
                using (SqlConnection connection = this.dbManager.CreateSQLConnection(this.dbContext))
                {
                    T result = SqlMapper.QueryMultiple(connection, query, param, null, this.commandTimeout, commandType);
                    return result;
                }
            });
        }

        public T SelectOne<T>(string query, object param = null, CommandType? commandType = null)
        {
            return this.logger.CatchAndLog<T>("SelectOne", () =>
            {
                LogDebugInfo(query, param, commandType);
                return Query<T>(query, param, commandType).FirstOrDefault();
            });
        }

        private void LogDebugInfo(string query, dynamic param, CommandType? commandType)
        {
            if (!logDebugInfo) { return; }
            try
            {
                StringBuilder sb = new StringBuilder(query);
                sb.AppendLine(Environment.NewLine);
                if (commandType != null)
                {
                    sb.AppendLine(commandType.ToString());
                }
                if (param != null)
                {
                    if (param is DynamicParameters)
                    {
                        var dynamicParams = param as DynamicParameters;
                        sb.AppendLine(ParametersToString(dynamicParams));
                    }
                    else
                    {
                        sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(param));
                    }
                }
                this.logger.LogDebug(sb.ToString());
            }
            catch (Exception ex) { this.logger.LogException("LogDebugInfo", ex); }
        }

        private string ParametersToString(DynamicParameters parameters)
        {
            var result = new StringBuilder();

            if (parameters != null)
            {
                var firstParam = true;
                var parametersLookup = (SqlMapper.IParameterLookup)parameters;
                foreach (var paramName in parameters.ParameterNames)
                {
                    if (!firstParam)
                    {
                        result.Append(", ");
                    }
                    firstParam = false;

                    result.Append('@');
                    result.Append(paramName);
                    result.Append(" = ");
                    try
                    {
                        var value = parametersLookup[paramName];// parameters.Get<dynamic>(paramName);
                        result.Append((value != null) ? value.ToString() : "{null}");
                    }
                    catch
                    {
                        result.Append("unknown");
                    }
                }
            }
            return result.ToString();
        }
    }
}

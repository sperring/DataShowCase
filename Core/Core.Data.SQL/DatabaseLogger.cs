using Dapper;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Web;
using Core;
using Core.Logging;

namespace Core.Data.SQL
{
    public class DatabaseLogger : ILogger
    {
        private int logLevel = 0;
        private string application = null;
        private string source = null;
        private string logger;
        private string logSchema;

        private IDbManager dbManager;

        public DatabaseLogger(string source)
        {
            this.logger = source;
            this.dbManager = new SqlDbManager();

            this.logLevel = Int16.Parse(ConfigurationManager.AppSettings["LogLevel"] ?? "1");

            if (ConfigurationManager.AppSettings["LogSchema"] != null)
            {
                this.logSchema = ConfigurationManager.AppSettings["LogSchema"].ToString();
            }
            else
            {
                this.logSchema = "dbo";
            }

            if (ConfigurationManager.AppSettings["Application"] != null)
            {
                this.application = ConfigurationManager.AppSettings["Application"].ToString();
            }
            else
            {
                this.application = Assembly.GetExecutingAssembly().FullName;
            }

            if (ConfigurationManager.AppSettings["Source"] != null)
            {
                this.source = ConfigurationManager.AppSettings["Source"].ToString();
            }
            else
            {
                this.source = System.Environment.MachineName;
            }
        }

        public void LogException(string message, Exception ex)
        {
            LogException(message, ex, this.logger);
        }

        public void LogDebug(string message)
        {
            LogDebug(message, logger);
        }

        public void LogInfo(string message)
        {
            LogInfo(message, this.logger);
        }

        public void CatchAndLog(string methodName, Action actionToCatchAndLog)
        {
            try
            {
                actionToCatchAndLog();
            }
            catch (Exception ex)
            {
                LogException(methodName, ex);
                throw;
            }
        }

        public T CatchAndLog<T>(string methodName, Func<T> actionToCatchAndLog)
        {
            T result;
            try
            {
                result = default(T);
                result = actionToCatchAndLog();
            }
            catch (Exception ex)
            {
                LogException(methodName, ex);
                throw;
            }
            return result;
        }

        private void InsertLog(Log model)
        {
            AddLogEntry(model);
        }

        private void AddLogEntry(Log model)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request.Headers["CorrelationId"] != null)
            {
                model.CorrelationId = new Guid(HttpContext.Current.Request.Headers["CorrelationId"].ToString());
            }

            using (SqlConnection connection = this.dbManager.CreateSQLConnection(DBContext.showcase))
            {
                SqlMapper.Execute(connection, string.Format("[ZBackOffice].[{0}].[InsertLog]", logSchema), model, null, null, CommandType.StoredProcedure);
            }
        }

        public void LogDebugWithCorrelation(string message, Guid id)
        {
            if (this.logLevel >= (int)LogLevel.Debug)
            {
                //this.log.Debug(message);
                var model = new Log()
                {
                    Application = application,
                    Source = source,
                    Date = DateTime.UtcNow,
                    Exception = "",
                    Message = message,
                    Level = LogLevel.Debug.ToString(),
                    Logger = this.logger,
                    Thread = Thread.CurrentThread.ManagedThreadId,
                    CorrelationId = id
                };
                InsertLog(model);
            }
        }

        public void LogInfoWithCorrelation(string message, Guid id)
        {
            if (this.logLevel >= (int)LogLevel.Info)
            {
                //this.log.Info(message);
                var model = new Log()
                {
                    Application = application,
                    Source = source,
                    Date = DateTime.UtcNow,
                    Exception = "",
                    Message = message,
                    Level = LogLevel.Info.ToString(),
                    Logger = this.logger,
                    Thread = Thread.CurrentThread.ManagedThreadId,
                    CorrelationId = id
                };
                InsertLog(model);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
            this.LogException(string.Format(format, args), new Exception(string.Format(format, args)));
        }

        public void DebugFormat(string format, params object[] args)
        {
            this.LogDebug(string.Format(format, args));
        }

        public void InfoFormat(string format, params object[] args)
        {
            this.LogInfo(string.Format(format, args));
        }

        public bool IsDebugEnabled
        {
            get { return (this.logLevel >= (int)LogLevel.Debug); }
        }

        public void Error(object message)
        {
            this.LogException(message.ToString(), new Exception(message.ToString()));
        }

        public void Error(object message, Exception exception)
        {
            this.LogException(message.ToString(), exception);
        }

        public void LogSQLCommand(System.Data.SqlClient.SqlCommand command)
        {
            try
            {
                try
                {
                    StackTrace stackTrace = new StackTrace();

                    // Get calling method name
                    Console.WriteLine(stackTrace.GetFrame(1).GetMethod().Name);

                    this.LogDebug(stackTrace.GetFrame(1).GetMethod().Name);

                    this.LogDebug(string.Format("Runnable Command: {0}", DataHelpers.CommandAsSql(command)));
                }
                catch (Exception)
                {
                }

                this.LogDebug(string.Format("CommandText: {0}", command.CommandText));
                foreach (SqlParameter param in command.Parameters)
                {
                    this.LogDebug(string.Format("{0}: {1}", param.ParameterName, param.Value));
                }
            }
            catch (Exception ex)
            {
                this.LogException("LogSQLCommand", ex);
            }
        }


        public void LogDebug(string message, string logger)
        {
            if (this.logLevel >= (int)LogLevel.Debug)
            {
                //this.log.Debug(message);
                var model = new Log()
                {
                    Application = application,
                    Source = source,
                    Date = DateTime.UtcNow,
                    Exception = "",
                    Message = message,
                    Level = LogLevel.Debug.ToString(),
                    Logger = logger,
                    Thread = Thread.CurrentThread.ManagedThreadId
                };
                InsertLog(model);
            }
        }

        public void LogInfo(string message, string logger)
        {
            if (this.logLevel >= (int)LogLevel.Info)
            {
                //this.log.Info(message);
                var model = new Log()
                {
                    Application = application,
                    Source = source,
                    Date = DateTime.UtcNow,
                    Exception = "",
                    Message = message,
                    Level = LogLevel.Info.ToString(),
                    Logger = logger,
                    Thread = Thread.CurrentThread.ManagedThreadId
                };
                InsertLog(model);
            }
        }

        public void Error(object message, Exception exception, string logger)
        {
            LogException(message.ToString(), exception, logger);
        }


        public void LogDebugWithCorrelation(string message, Guid id, string logger)
        {
            if (this.logLevel >= (int)LogLevel.Debug)
            {
                var model = new Log()
                {
                    Application = application,
                    Source = source,
                    Date = DateTime.UtcNow,
                    Exception = "",
                    Message = message,
                    Level = LogLevel.Debug.ToString(),
                    Logger = logger,
                    Thread = Thread.CurrentThread.ManagedThreadId,
                    CorrelationId = id
                };
                InsertLog(model);
            }
        }

        public void LogInfoWithCorrelation(string message, Guid id, string logger)
        {
            if (this.logLevel >= (int)LogLevel.Info)
            {
                var model = new Log()
                {
                    Application = application,
                    Source = source,
                    Date = DateTime.UtcNow,
                    Exception = "",
                    Message = message,
                    Level = LogLevel.Debug.ToString(),
                    Logger = logger,
                    Thread = Thread.CurrentThread.ManagedThreadId,
                    CorrelationId = id
                };
                InsertLog(model);
            }
        }


        public void LogException(string message, Exception ex, string logger)
        {
            if (this.logLevel >= (int)LogLevel.Error)
            {
                //this.log.Error(message, ex);
                var model = new Log()
                {
                    Application = application,
                    Source = source,
                    Date = DateTime.UtcNow,
                    Exception = Newtonsoft.Json.JsonConvert.SerializeObject(ex),
                    Message = message,
                    Level = LogLevel.Error.ToString(),
                    Logger = logger,
                    Thread = Thread.CurrentThread.ManagedThreadId
                };
                InsertLog(model);
            }
        }
    }
}

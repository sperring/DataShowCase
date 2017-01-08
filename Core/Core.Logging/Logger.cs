using log4net;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using Core;
namespace Core.Logging

{
    public enum LogLevel
    {
        None = 0,
        Error = 1,
        Info = 2,
        Debug = 3
    }
    public class Logger : ILogger
    {
        private readonly ILog log;
        private int logLevel = 0;

        public Logger(string source)
        {
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(source);
            this.logLevel = Int16.Parse(ConfigurationManager.AppSettings["LogLevel"] ?? "1");
        }

        public void LogException(string message, Exception ex)
        {
            if (this.logLevel >= (int)LogLevel.Error)
            {
                this.log.Error(message, ex);
            }
        }

        public void LogDebug(string message)
        {
            if (this.logLevel >= (int)LogLevel.Debug)
            {
                this.log.Debug(message);
            }
        }

        public void LogInfo(string message)
        {
            if (this.logLevel >= (int)LogLevel.Info)
            {
                this.log.Info(message);
            }
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

        public void LogDebugWithCorrelation(string message, Guid id)
        {
        }

        public void LogInfoWithCorrelation(string message, Guid id)
        {
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (this.logLevel >= (int)LogLevel.Error)
            {
                this.log.Debug(string.Format(format, args));
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (this.logLevel >= (int)LogLevel.Debug)
            {
                this.log.Debug(string.Format(format, args));
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (this.logLevel >= (int)LogLevel.Info)
            {
                this.log.Debug(string.Format(format, args));
            }
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


        public void LogException(string message, Exception ex, string logger)
        {
        }

        public void LogDebug(string message, string logger)
        {
        }

        public void LogInfo(string message, string logger)
        {
        }

        public void Error(object message, Exception exception, string logger)
        {
        }

        public void LogDebugWithCorrelation(string message, Guid id, string logger)
        {
        }

        public void LogInfoWithCorrelation(string message, Guid id, string logger)
        {
        }
    }
}

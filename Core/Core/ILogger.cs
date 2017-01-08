using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface ILogger
    {
        void LogException(string message, Exception ex);

        void LogException(string message, Exception ex, string logger);

        void LogDebug(string message, string logger);

        void LogInfo(string message, string logger);

        void LogDebug(string message);

        void LogInfo(string message);

        bool IsDebugEnabled { get; }

        void Error(object message);

        void Error(object message, Exception exception);

        void Error(object message, Exception exception, string logger);

        void LogDebugWithCorrelation(string message, Guid id, string logger);

        void LogInfoWithCorrelation(string message, Guid id, string logger);

        void ErrorFormat(string format, params object[] args);

        void DebugFormat(string format, params object[] args);

        void InfoFormat(string format, params object[] args);

        void CatchAndLog(string methodName, Action actionToCatchAndLog);

        T CatchAndLog<T>(string methodName, Func<T> actionToCatchAndLog);

        void LogSQLCommand(SqlCommand command);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KonfDB.Infrastructure.Logging
{
    public interface ILogger
    {
        void Info(object message);
        void InfoFormat(string format, params object[] args);
        void Debug(object message);
        void DebugFormat(string format, params object[] args);
        void Error(object message);
        void Error(object message, Exception exception);
        void ErrorFormat(string format, params object[] args);
        void SvcInfo(object message);
        void SvcInfo(object message, Exception exception);
    }
}

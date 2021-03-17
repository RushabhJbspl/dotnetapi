using System;
using System.Collections.Generic;
using System.Text;

namespace LoggingNlog
{
    public interface INLogger<T>
    {
        void WriteInfoLog(string methodName, string LogData = null);

         void WriteErrorLog(string mthodName, Exception Error);

        void WriteTraceLog(string methodName, string eventName, string LogData = null);
    }
}

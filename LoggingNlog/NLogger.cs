using NLog;
using System;
using System.Threading.Tasks;

namespace LoggingNlog
{
    public class NLogger<T> : INLogger<T>
    {
        private readonly Logger _logger;
        public NLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void WriteErrorLog(string MethodName, Exception Error)
        {
            try
            {
                Task.Run(() => _logger.Error("#Class Name: " + typeof(T).Name + "##MethodName:" + MethodName + Environment.NewLine + "Error: " + Error + Environment.NewLine
                    + "==================================================================================================================="));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void WriteInfoLog(string methodName, string LogData = null)
        {
            try
            {
                Task.Run(() => _logger.Info("#Class Name: " + typeof(T).Name + "##MethodName:" + methodName + Environment.NewLine + "LogData: " + LogData + Environment.NewLine
                    + "==================================================================================================================="));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void WriteTraceLog(string methodName, string eventName, string LogData = null)
        {
            try
            {
                Task.Run(() => _logger.Trace("#Class Name: " + typeof(T).Name + "##MethodName:" + methodName + Environment.NewLine
                    + "EventName:" + eventName + Environment.NewLine
                    + "LogData: " + LogData + Environment.NewLine
                    + "==================================================================================================================="));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}

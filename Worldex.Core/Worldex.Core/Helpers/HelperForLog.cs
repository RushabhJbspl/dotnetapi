using System;
using System.Threading.Tasks;

namespace Worldex.Core.Helpers
{
    public class HelperForLog
    {
        public async static void WriteLogForSocket(string MethodName, string Controllername, string LogData = null, string accessToken = null,bool IsWrite = true)
        {            
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                if (IsWrite)
                {
                    Task.Run(() => logger.Trace("DateTime:" + Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff") + " MethodName:" + MethodName + ", Controllername: " + Controllername + Environment.NewLine + "LogData: " + LogData + Environment.NewLine + "___________________________________________________________________________________________________________________________"));
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => logger.Error(ex));
            }
        }

        public async static void WriteLogForConnection(string MethodName, string Controllername, string LogData = null, string accessToken = null)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                Task.Run(() => logger.Debug("DateTime:" + Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff") + " MethodName:" + MethodName + ", Controllername: " + Controllername + Environment.NewLine + "LogData: " + LogData + Environment.NewLine + "___________________________________________________________________________________________________________________________"));
            }
            catch (Exception ex)
            {
                Task.Run(() => logger.Error(ex));
            }
        }
        public async static void WriteLogForActivity(string MethodName, string Controllername, string LogData = null, string accessToken = null)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                Task.Run(() => logger.Debug("DateTime:" + Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff") + " MethodName:" + MethodName + ", Controllername: " + Controllername + Environment.NewLine + "LogData: " + LogData + Environment.NewLine + "___________________________________________________________________________________________________________________________"));
            }
            catch (Exception ex)
            {
                Task.Run(() => logger.Error(ex));
            }
        }
        //Rita 26-10-2018 for all log used in below project of core
        public async static void WriteLogIntoFile(string MethodName, string Controllername, string LogData = null, string accessToken = null)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                Task.Run(() => logger.Info("DateTime:" + Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff") + "##MethodName:" + MethodName + "##Controllername: " + Controllername + Environment.NewLine + "LogData: " + LogData + Environment.NewLine + "==================================================================================================================="));
            }
            catch (Exception ex)
            {
                Task.Run(() => logger.Error(ex));
            }
        }
        public async static void WriteErrorLog(string MethodName, string Controllername, Exception Error, string accessToken = null)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                Task.Run(() => logger.Error("DateTime:" + Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff") + "##MethodName:" + MethodName + "##Controllername: " + Controllername + Environment.NewLine + "Error: " + Error + Environment.NewLine + "==================================================================================================================="));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public static async Task WriteLogIntoFileAsync(string MethodName, string Controllername, string LogData = null, string accessToken = null)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                Task.Run(() => logger.Info("DateTime:" + Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff") + "##MethodName:" + MethodName + "##Controllername: " + Controllername + Environment.NewLine + "LogData: " + LogData + Environment.NewLine + "==================================================================================================================="));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public async static Task WriteLogIntoFileAsyncDtTm(string MethodName, string Controllername, string LogData,DateTime DtTm)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                logger.Info("DateTime:" + DtTm.ToString("dd/MM/yyyy HH:mm:ss.ffff") + "##MethodName:" + MethodName + "##Controllername: " + Controllername + Environment.NewLine + "LogData: " + LogData + Environment.NewLine + "===================================================================================================================");
            }
            catch (Exception ex)
            {
                Task.Run(() => logger.Error(ex));
            }
        }
    }
}

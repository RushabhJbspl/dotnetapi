using Worldex.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Worldex.Core.Helpers;

namespace Worldex.Infrastructure.Data
{
    public class BasePage : IBasePage
    {
        readonly ILogger<BasePage> _log;

        public BasePage(ILogger<BasePage> log)
        {
            _log = log;           
        }

        //komal 7-10-2019 5:23 PM make UTC time
        public  DateTime UTC_To_IST()
        {
            try
            {
                //DateTime myUTC = DateTime.UtcNow;
                //// 'Dim utcdate As DateTime = DateTime.ParseExact(DateTime.UtcNow, "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                //// Dim utcdate As DateTime = DateTime.ParseExact(myUTC, "M/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                //// 'Dim utcdate As DateTime = DateTime.ParseExact("11/09/2016 6:31:00 PM", "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                //DateTime istdate = TimeZoneInfo.ConvertTimeFromUtc(myUTC, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                //// MsgBox(myUTC & " - " & utcdate & " - " & istdate)
                return Helpers.UTC_To_IST();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }
    }
}

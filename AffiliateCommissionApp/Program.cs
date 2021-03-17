using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using DepositStatusCheckApp;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Timers;

namespace AffiliateCommissionApp
{
    public class Program
    {
        static System.Timers.Timer TransactionTick = new System.Timers.Timer();
        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            try
            {
                Console.Write("start");
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                Configuration = builder.Build();

                var conStr = Configuration["SqlServerConnectionString"];

                TransactionTick.Interval = Convert.ToInt32(Configuration["Interval"]); // 1000;
                TransactionTick.Elapsed += new ElapsedEventHandler(transaction_tick);
                TransactionTick.Start();

                Console.WriteLine("Press \'q\' to quit");
                while (Console.Read() != 'q') ;
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Program", "main method");
            }
        }


        #region TimerTick
        private static void transaction_tick(object sender, System.EventArgs e)
        {
            try
            {
                TransactionTick.Stop();
                TransactionTick.Interval = Convert.ToInt32(Configuration["Interval"]); //1800000;
                RunCommissionCron();
                TransactionTick.Start();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Program", "transaction_tick");
            }
            finally
            {
                TransactionTick.Start();
            }
        }
        #endregion

        #region Commission Cron Method
        public static void RunCommissionCron()
        {
            string SqlStr = string.Empty;
            DataSet dSet = new DataSet();
            BizResponseClass bizResponseClass = new BizResponseClass();
            try
            {
                SqlStr = "Select  * from AffiliateSchemeTypeMapping where status=1";
                dSet = (new DataCommon()).OpenDataSet("AffiliateSchemeTypeMapping", SqlStr, dSet, 30);

                if (dSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dRow in dSet.Tables[0].Rows)
                    {
                        DataSet dSet1 = new DataSet();

                        SqlStr = "Select top 1 * from AffiliateCommissionCron Where SchemeMappingId = " + dRow["Id"].ToString() + " Order By Id Desc";
                        dSet1 = (new DataCommon()).OpenDataSet("AffiliateCommissionCron", SqlStr, dSet1, 30);

                        if (dSet1.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dRow1 in dSet1.Tables[0].Rows)
                            {
                                DateTime LastCronTime = Convert.ToDateTime(dRow1["ToDate"]);
                                LastCronTime = LastCronTime.AddHours(Convert.ToDouble(dRow["CommissionHour"]));
                                if (LastCronTime < UTC_To_IST())
                                {
                                    DateTime FromDate = Convert.ToDateTime(dRow1["ToDate"]);
                                    DateTime ToDate = UTC_To_IST();

                                    if (FromDate != null)
                                    {
                                        WriteLogIntoFile(2, UTC_To_IST(), "Cron Executed For:", dRow["Description"].ToString());

                                        Console.WriteLine("Execute Cron For : " + dRow["Description"].ToString());

                                        SqlStr = "INSERT INTO [dbo].[AffiliateCommissionCron]([CreatedDate],[CreatedBy],[UpdatedBy],[UpdatedDate],[Status],[SchemeMappingId],[Remarks],[FromDate],[ToDate]) VALUES(dbo.GetISTDate(),1,1,null,1," + dRow["Id"] + "," + "'Cron for :" + dRow["Description"].ToString() + "'" + ",'" + FromDate + "','" + ToDate + "');Select Cast(scope_identity() as varchar(10)) AS int;";

                                        var Id = (new DataCommon()).ExecuteScalar(SqlStr);

                                        SqlParameter[] param1 = new SqlParameter[]{
                                             new SqlParameter("@SchemeMappingId",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(dRow["Id"])),
                                                                    new SqlParameter("@CronRefNo",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(Id)),
                                                                 new SqlParameter("@FromDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,FromDate ),
                                                                   new SqlParameter("@ToDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,ToDate ),
                                                                 new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                                    new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                                    new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)};

                                        var res = (new DataCommon()).ExecuteSP("sp_AffiliateCron", ref param1);

                                        bizResponseClass.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[4].Value);
                                        bizResponseClass.ReturnMsg = @param1[5].Value.ToString();
                                        bizResponseClass.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[6].Value);

                                        WriteLogIntoFile(2, UTC_To_IST(), "Cron executed For:", dRow["Description"].ToString(), "#ReturnCode#" + bizResponseClass.ReturnCode + "#ReturnMsg#" + bizResponseClass.ReturnMsg + "#ErrorCode#" + bizResponseClass.ErrorCode);
                                    }
                                }
                                #region OldCode
                                //                if (Convert.ToInt64(enAffiliateSchemeTypeMapping.MLMDeposit) == Convert.ToInt64(dRow["Id"]))
                                //                {
                                //                    if (LastCronTime < UTC_To_IST())
                                //                    {
                                //                        DateTime now = UTC_To_IST();
                                //                        DateTime dt = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0);

                                //                        if (LastCronTime < dt)
                                //                        {
                                //                            Console.WriteLine("Execute Cron For : " + dRow["Description"].ToString());

                                //                            var FromDate = dt.AddDays(-1).AddHours(-1);
                                //                            var ToDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);


                                //                            SqlStr = "INSERT INTO AffiliateCommissionCron(CreatedDate,CreatedBy,Status,SchemeMappingId) VALUES(dbo.GetISTDate(),1,1," + dRow["Id"] + ");Select Cast(scope_identity() as varchar(10)) AS int;";
                                //                            var Id = (new DataCommon()).ExecuteScalar(SqlStr);

                                //                            SqlParameter[] param1 = new SqlParameter[]{
                                //                                new SqlParameter("@CronRefNo",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(Id)),
                                //                             new SqlParameter("@FromDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,FromDate ),
                                //                               new SqlParameter("@ToDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,ToDate ),
                                //                             new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                                //new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                                //new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)};

                                //                            var res = (new DataCommon()).ExecuteSP("sp_AffiliateCommissionMLMDeposition", ref param1);

                                //                            bizResponseClass.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[3].Value);
                                //                            bizResponseClass.ReturnMsg = @param1[4].Value.ToString();
                                //                            bizResponseClass.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[5].Value);
                                //                        }
                                //                    }
                                //                }

                                //                else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.MLMBuyTrading) == Convert.ToInt64(dRow["Id"]))
                                //                {
                                //                    if (LastCronTime < UTC_To_IST())
                                //                    {
                                //                        DateTime now = UTC_To_IST();
                                //                        DateTime dt = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0);

                                //                        if (LastCronTime < dt)
                                //                        {
                                //                            Console.WriteLine("Execute Cron For : " + dRow["Description"].ToString());
                                //                            var FromDate = dt.AddDays(-1).AddHours(-1);
                                //                            var ToDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);
                                //                            SqlStr = "INSERT INTO AffiliateCommissionCron(CreatedDate,CreatedBy,Status,SchemeMappingId) VALUES(dbo.GetISTDate(),1,1," + dRow["Id"] + ");Select Cast(scope_identity() as varchar(10)) AS int;";
                                //                            var Id = (new DataCommon()).ExecuteScalar(SqlStr);

                                //                            SqlParameter[] param1 = new SqlParameter[]{
                                //                                new SqlParameter("@CronRefNo",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(Id)),
                                //                             new SqlParameter("@FromDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,FromDate ),
                                //                               new SqlParameter("@ToDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,ToDate ),
                                //                             new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                                //new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                                //new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)};

                                //                            var res = (new DataCommon()).ExecuteSP("sp_AffiliateCommissionMLMBuyTrading", ref param1);
                                //                        }
                                //                    }
                                //                }

                                //                else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.MLMSellTrading) == Convert.ToInt64(dRow["Id"]))
                                //                {
                                //                    if (LastCronTime < UTC_To_IST())
                                //                    {
                                //                        DateTime now = UTC_To_IST();
                                //                        DateTime dt = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0);

                                //                        if (LastCronTime > dt)
                                //                        {
                                //                            Console.WriteLine("Execute Cron For : " + dRow["Description"].ToString());

                                //                            var FromDate = dt.AddDays(-1).AddHours(-1);
                                //                            var ToDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);

                                //                            SqlStr = "INSERT INTO AffiliateCommissionCron(CreatedDate,CreatedBy,Status,SchemeMappingId) VALUES(dbo.GetISTDate(),1,1," + dRow["Id"] + ");Select Cast(scope_identity() as varchar(10)) AS int;";
                                //                            var Id = (new DataCommon()).ExecuteScalar(SqlStr);

                                //                            SqlParameter[] param1 = new SqlParameter[]{
                                //                                new SqlParameter("@CronRefNo",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(Id)),
                                //                             new SqlParameter("@FromDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,FromDate ),
                                //                               new SqlParameter("@ToDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,ToDate ),
                                //                             new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                                //new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                                //new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)};

                                //                            var res = (new DataCommon()).ExecuteSP("sp_AffiliateCommissionMLMSellTrading", ref param1);

                                //                        }
                                //                    }
                                //                }

                                //                else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.SlabSignUp) == Convert.ToInt64(dRow["Id"]))
                                //                {
                                //                    if (LastCronTime < UTC_To_IST())
                                //                    {
                                //                        DateTime now = UTC_To_IST();
                                //                        DateTime dt = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0);

                                //                        if (LastCronTime > dt)
                                //                        {
                                //                            Console.WriteLine("Execute Cron For : " + dRow["Description"].ToString());

                                //                            var FromDate = dt.AddDays(-1).AddHours(-1);
                                //                            var ToDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);

                                //                            SqlStr = "INSERT INTO AffiliateCommissionCron(CreatedDate,CreatedBy,Status,SchemeMappingId) VALUES(dbo.GetISTDate(),1,1," + dRow["Id"] + ");Select Cast(scope_identity() as varchar(10)) AS int;";
                                //                            var Id = (new DataCommon()).ExecuteScalar(SqlStr);

                                //                            SqlParameter[] param1 = new SqlParameter[]{
                                //                                new SqlParameter("@CronRefNo",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(Id)),
                                //                             new SqlParameter("@FromDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,FromDate ),
                                //                               new SqlParameter("@ToDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,ToDate ),
                                //                             new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                                //new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                                //new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)};

                                //                            var res = (new DataCommon()).ExecuteSP("sp_AffiliateCommissionSlabSignUp", ref param1);

                                //                        }
                                //                    }
                                //                }

                                //                else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.SlabDeposit) == Convert.ToInt64(dRow["Id"]))
                                //                {
                                //                    if (LastCronTime < UTC_To_IST())
                                //                    {
                                //                        DateTime now = UTC_To_IST();
                                //                        DateTime dt = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0);

                                //                        if (LastCronTime > dt)
                                //                        {
                                //                            Console.WriteLine("Execute Cron For : " + dRow["Description"].ToString());

                                //                            var FromDate = dt.AddDays(-1).AddHours(-1);
                                //                            var ToDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);

                                //                            SqlStr = "INSERT INTO AffiliateCommissionCron(CreatedDate,CreatedBy,Status,SchemeMappingId) VALUES(dbo.GetISTDate(),1,1," + dRow["Id"] + ");Select Cast(scope_identity() as varchar(10)) AS int;";
                                //                            var Id = (new DataCommon()).ExecuteScalar(SqlStr);


                                //                            SqlParameter[] param1 = new SqlParameter[]{
                                //                                new SqlParameter("@CronRefNo",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(Id)),
                                //                             new SqlParameter("@FromDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,FromDate ),
                                //                               new SqlParameter("@ToDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,ToDate ),
                                //                             new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                                //new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                                //new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)};

                                //                            var res = (new DataCommon()).ExecuteSP("sp_AffiliateCommissionSlabDeposition", ref param1);
                                //                        }
                                //                    }
                                //                }

                                //                else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.SlabBuyTrading) == Convert.ToInt64(dRow["Id"]))
                                //                {
                                //                    if (LastCronTime < UTC_To_IST())
                                //                    {
                                //                        DateTime now = UTC_To_IST();
                                //                        DateTime dt = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0);

                                //                        if (LastCronTime > dt)
                                //                        {
                                //                            Console.WriteLine("Execute Cron For : " + dRow["Description"].ToString());

                                //                            var FromDate = dt.AddDays(-1).AddHours(-1);
                                //                            var ToDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);

                                //                            SqlStr = "INSERT INTO AffiliateCommissionCron(CreatedDate,CreatedBy,Status,SchemeMappingId) VALUES(dbo.GetISTDate(),1,1," + dRow["Id"] + ");Select Cast(scope_identity() as varchar(10)) AS int;";
                                //                            var Id = (new DataCommon()).ExecuteScalar(SqlStr);

                                //                            SqlParameter[] param1 = new SqlParameter[]{
                                //                                new SqlParameter("@CronRefNo",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(Id)),
                                //                             new SqlParameter("@FromDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,FromDate ),
                                //                               new SqlParameter("@ToDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,ToDate ),
                                //                             new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                                //new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                                //new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)};

                                //                            var res = (new DataCommon()).ExecuteSP("sp_AffiliateCommissionSlabBuyTrading", ref param1);
                                //                        }
                                //                    }
                                //                }

                                //                else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.SlabSellTrading) == Convert.ToInt64(dRow["Id"]))
                                //                {
                                //                    if (LastCronTime < UTC_To_IST())
                                //                    {
                                //                        DateTime now = UTC_To_IST();
                                //                        DateTime dt = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0);

                                //                        if (LastCronTime > dt)
                                //                        {
                                //                            Console.WriteLine("Execute Cron For : " + dRow["Description"].ToString());

                                //                            var FromDate = dt.AddDays(-1).AddHours(-1);
                                //                            var ToDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);

                                //                            SqlStr = "INSERT INTO AffiliateCommissionCron(CreatedDate,CreatedBy,Status,SchemeMappingId) VALUES(dbo.GetISTDate(),1,1," + dRow["Id"] + ");Select Cast(scope_identity() as varchar(10)) AS int;";
                                //                            var Id = (new DataCommon()).ExecuteScalar(SqlStr);

                                //                            SqlParameter[] param1 = new SqlParameter[]{
                                //                                new SqlParameter("@CronRefNo",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(Id)),
                                //                             new SqlParameter("@FromDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,FromDate ),
                                //                               new SqlParameter("@ToDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,ToDate ),
                                //                             new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                                //new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                                //new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)};

                                //                            var res = (new DataCommon()).ExecuteSP("sp_AffiliateCommissionSlabSellTrading", ref param1);
                                //                        }
                                //                    }
                                //                }

                                //                else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.FlatSignUp) == Convert.ToInt64(dRow["Id"]))
                                //                {
                                //                    if (LastCronTime < UTC_To_IST())
                                //                    {
                                //                        DateTime now = UTC_To_IST();
                                //                        DateTime dt = new DateTime(now.Year, now.Month, now.Day, 1, 0, 0);

                                //                        if (LastCronTime > dt)
                                //                        {
                                //                            Console.WriteLine("Execute Cron For : " + dRow["Description"].ToString());

                                //                            var FromDate = dt.AddDays(-1).AddHours(-1);
                                //                            var ToDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);

                                //                            SqlStr = "INSERT INTO AffiliateCommissionCron(CreatedDate,CreatedBy,Status,SchemeMappingId) VALUES(dbo.GetISTDate(),1,1," + dRow["Id"] + ");Select Cast(scope_identity() as varchar(10)) AS int;";
                                //                            var Id = (new DataCommon()).ExecuteScalar(SqlStr);

                                //                            SqlParameter[] param1 = new SqlParameter[]{
                                //                                new SqlParameter("@CronRefNo",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(Id)),
                                //                             new SqlParameter("@FromDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,FromDate ),
                                //                               new SqlParameter("@ToDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,ToDate ),
                                //                             new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                                //new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                                //new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)};

                                //                            var res = (new DataCommon()).ExecuteSP("sp_AffiliateCommissionFlatSignUp", ref param1);
                                //                        }
                                //                    }
                                //                }
                                #endregion
                            }
                        }
                        else
                        {
                            Console.WriteLine("Execute Cron For : " + dRow["Description"].ToString());
                            WriteLogIntoFile(2, UTC_To_IST(), "Cron executed For: ", dRow["Description"].ToString());
                            int hour = Convert.ToInt32(dRow["CommissionHour"]);
                            DateTime now = UTC_To_IST();
                            hour = 0 - hour;
                            DateTime dt = now.AddHours(hour); //new DateTime(now.Year, now.Month, now.Day, hour, 0, 0);

                            DateTime FromDate = dt;
                            DateTime ToDate = now;

                            SqlStr = "INSERT INTO [dbo].[AffiliateCommissionCron]([CreatedDate],[CreatedBy],[UpdatedBy],[UpdatedDate],[Status],[SchemeMappingId],[Remarks],[FromDate],[ToDate]) VALUES(dbo.GetISTDate(),1,1,null,1," + dRow["Id"] + "," + "'Cron for :" + dRow["Description"].ToString() + "'" + ",'" + FromDate + "','" + ToDate + "');Select Cast(scope_identity() as varchar(10)) AS int;";

                            var Id = (new DataCommon()).ExecuteScalar(SqlStr);

                            SqlParameter[] param1 = new SqlParameter[]{
                                             new SqlParameter("@SchemeMappingId",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(dRow["Id"])),
                                                                    new SqlParameter("@CronRefNo",SqlDbType.BigInt, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, Convert.ToInt64(Id)),
                                                                 new SqlParameter("@FromDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,FromDate ),
                                                                   new SqlParameter("@ToDate",SqlDbType.DateTime, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default,ToDate ),
                                                                 new SqlParameter("@ReturnCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnCode) ,
                                    new SqlParameter("@ReturnMsg",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ReturnMsg) ,
                                    new SqlParameter("@ErrorCode",SqlDbType.Int, 8, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, bizResponseClass.ErrorCode)};

                            var res = (new DataCommon()).ExecuteSP("sp_AffiliateCron", ref param1);

                            bizResponseClass.ReturnCode = (enResponseCode)Convert.ToInt32(@param1[4].Value);
                            bizResponseClass.ReturnMsg = @param1[5].Value.ToString();
                            bizResponseClass.ErrorCode = (enErrorCode)Convert.ToInt32(@param1[6].Value);

                            WriteLogIntoFile(2, UTC_To_IST(), "Cron executed For:", dRow["Description"].ToString(), "#ReturnCode#" + bizResponseClass.ReturnCode + "#ReturnMsg#" + bizResponseClass.ReturnMsg + "#ErrorCode#" + bizResponseClass.ErrorCode);

                            #region Old code
                            //if (Convert.ToInt64(enAffiliateSchemeTypeMapping.MLMDeposit) == Convert.ToInt64(dRow["Id"]))
                            //{

                            //}
                            //else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.MLMBuyTrading) == Convert.ToInt64(dRow["Id"]))
                            //{

                            //}
                            //else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.MLMSellTrading) == Convert.ToInt64(dRow["Id"]))
                            //{

                            //}
                            //else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.SlabSignUp) == Convert.ToInt64(dRow["Id"]))
                            //{

                            //}
                            //else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.SlabDeposit) == Convert.ToInt64(dRow["Id"]))
                            //{

                            //}
                            //else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.SlabBuyTrading) == Convert.ToInt64(dRow["Id"]))
                            //{

                            //}
                            //else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.SlabSellTrading) == Convert.ToInt64(dRow["Id"]))
                            //{

                            //}
                            //else if (Convert.ToInt64(enAffiliateSchemeTypeMapping.FlatSignUp) == Convert.ToInt64(dRow["Id"]))
                            //{

                            //}
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Program", "RunCommissionCron");
            }
            finally
            {
                TransactionTick.Start();
            }
        }
        #endregion


        #region Logging
        public static void WriteLogIntoFile(byte CheckReq_Res, DateTime Date, string MethodName, string Controllername, string Req_Res = null, string accessToken = null)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                if (CheckReq_Res == 1)
                {
                    logger.Info("\nDate:" + Date + "\nMethodName:" + MethodName + ", Controllername: " + Controllername + "\nAccessToken: " + accessToken + "\nRequest: " + Req_Res + "\n===================================================================================================================");
                }
                else //if(CheckReq_Res==2)
                {
                    logger.Info("\nDate:" + Date + "\nMethodName:" + MethodName + ", Controllername: " + Controllername + "\nResponse: " + Req_Res + "\n===================================================================================================================");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public static void WriteErrorLog(Exception ex, string methodName, string Source, string OtherInfo = "")
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                logger.Error("\nDate:" + UTC_To_IST() + "\nMethodName:" + methodName + ",Controllername: " + Source + "\nError: " + ex.Message + "\nOtherInfo:" + OtherInfo + "===================================================================================================================");
            }
            catch (Exception ex1)
            {
                logger.Error(ex1);
            }
        }
        public static void WriteRequestLog(string inputString, string methodName, string APIName, int IsAllow = 1, int Action = 1)
        {
            string strFilePath = string.Empty;
            try
            {
                if (IsAllow == 1)
                {
                    string ip = "";
                    string dir = AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\";

                    strFilePath = (dir + APIName + "Log" + Convert.ToString("_")) + UTC_To_IST().ToString("yyyyMMdd") + ".txt";

                    dynamic maxRetry = 2;

                    //for Error: The process cannot access the file because it is being used by another process
                    for (int retry = 0; retry <= maxRetry - 1; retry++)
                    {
                        try
                        {
                            if (Action == 1) //komal 12-9-2018 multi line log
                            {
                                using (StreamWriter sw = new StreamWriter(strFilePath, true))
                                {
                                    sw.WriteLine("----------------------------------------{0}---------------------------------------------------------", ip);
                                    sw.WriteLine((Convert.ToString("-----> ") + methodName) + " " + " Time " + UTC_To_IST());
                                    sw.WriteLine(" Response : {0}", inputString);

                                    sw.Flush();
                                    sw.Close();
                                }
                                break;
                            }
                            else if (Action == 2)
                            {
                                using (StreamWriter sw = new StreamWriter(strFilePath, true))
                                {
                                    sw.WriteLine("----------------------------------------{0}---------------------------------------------------------", ip);
                                    sw.WriteLine((Convert.ToString("-----> ") + methodName) + ":" + " Time " + UTC_To_IST() + ": Response - {0}", inputString);
                                    sw.Flush();
                                    sw.Close();
                                }
                                break;
                            }
                        }
                        catch (IOException generatedExceptionName)
                        {
                            if (retry < maxRetry - 1)
                            {
                                WriteErrorLog(generatedExceptionName, "Affiliate Commission app", "WriteRequestLog -Retry:Sleep");
                                // Wait some time before retry (2 secs)
                                Thread.Sleep(2000);
                                // handle unsuccessfull write attempts or just ignore.
                            }
                            else
                            {
                                WriteErrorLog(generatedExceptionName, "Affiliate Commission app", "WriteRequestLog-Retry Complete");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Affiliate Commission app", System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + methodName);
                ex = null;
            }
        }
        #endregion

        #region Other Function
        public static DateTime UTC_To_IST()
        {
            DateTime myUTC = DateTime.UtcNow;
            // 'Dim utcdate As DateTime = DateTime.ParseExact(DateTime.UtcNow, "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
            // Dim utcdate As DateTime = DateTime.ParseExact(myUTC, "M/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
            // 'Dim utcdate As DateTime = DateTime.ParseExact("11/09/2016 6:31:00 PM", "M/dd/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
            DateTime istdate = TimeZoneInfo.ConvertTimeFromUtc(myUTC, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            // MsgBox(myUTC & " - " & utcdate & " - " & istdate)
            return istdate;

        }
        #endregion
    }
}

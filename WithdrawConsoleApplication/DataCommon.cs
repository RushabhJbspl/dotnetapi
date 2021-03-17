using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.VisualBasic.CompilerServices;

namespace WithdrawConsoleApplication
{
    public class dStructure
    {
        public static string ConnectionString = "";
        //public static string ConnectionString = @"Server=jbsltest.database.windows.net;Initial Catalog=MNTRDec16;User ID=jbsplsa;Password=biztech@123;";
        //public static string ConnectionString = @"Server=DESKTOP-GEUGUEU\SQLEXPRESS;Initial Catalog=MNTRFeb;User ID=sa;Password=test123$;";
        //public static string ConnectionString = "Server=202.143.96.59;Initial Catalog=MNTRFeb16Staging;User ID=sa;Password=C!y@z^$xuv;";
    }

    public class DataCommon
    {

        public SqlConnection Conn;

        private string _DatabaseName = string.Empty;
        #region "Connection Procedures"
        //public string DatabaseName
        //{

        //    get
        //    {
        //        string sConn = string.Empty;
        //        string sFileData = string.Empty;
        //        Conn = new System.Data.SqlClient.SqlConnection();
        //        try
        //        {
        //            // Modified because of encryption algorythm changed.
        //            // -Nishit Jani on 21/07/2014 12:33 PM
        //            //Dim obj As New GTechUtil.Utility
        //            //Dim obj As BizTechCommon.BizTechEncryption.EncryptionUtility = New BizTechCommon.BizTechEncryption.EncryptionUtility
        //            sFileData = System.IO.File.ReadAllText(System.Configuration.ConfigurationManager.AppSettings["DBINIPATH"] + "\\dbini.txt");
        //            //sConn = obj.GetDecryptedText(sFileData, "aSp.NeT")
        //            //sConn = obj.GetDecryptedText(sFileData, "b1z73kh4rd1k")
        //            Conn.ConnectionString = sConn;
        //            _DatabaseName = Conn.Database;
        //        }
        //        catch (Exception ex)
        //        {
        //            //Throw ex
        //            return "Exeption in dbini";
        //        }

        //        return _DatabaseName;
        //    }


        //    set { _DatabaseName = value; }
        //}

        public string ConnectionString
        {
            get
            {
                string sConn = string.Empty;
                string sFilePath = string.Empty;
                string sFileData = string.Empty;
                if (string.IsNullOrEmpty(dStructure.ConnectionString.Trim()))
                {
                    if ((Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["IsLive"])) == 1)
                    {
                        BizTechCommon.BizTechEncryption.EncryptionUtility obj = new BizTechCommon.BizTechEncryption.EncryptionUtility();
                        sFilePath = System.Configuration.ConfigurationManager.AppSettings["DBINIPATH"] + "\\DBINI.txt";
                        if (System.IO.File.Exists(sFilePath) == false)
                        {
                            throw new Exception("DB INI File Not Found");
                        }
                        sFileData = System.IO.File.ReadAllText(sFilePath);
                        sConn = obj.GetDecryptedText(sFileData, "b1z73kh4rd1k");

                        //    Dim databaseName As String = sConn("Database")

                        dStructure.ConnectionString = sConn;
                        return sConn;
                    }
                    else
                    {
                        sConn = "Server=" + ConfigurationManager.AppSettings["ServerName"] + ";Initial Catalog=" + ConfigurationManager.AppSettings["Database"] + ";User ID=" + ConfigurationManager.AppSettings["UserID"] + ";Password=" + ConfigurationManager.AppSettings["Password"] + ";";
                        dStructure.ConnectionString = sConn;
                        return sConn;
                    }
                }
                else
                {
                    return dStructure.ConnectionString;
                }
            }
        }

        public void cnOpen()
        {
            try
            {
                if (Conn == null)
                {
                    Conn = new System.Data.SqlClient.SqlConnection();
                }
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
                Conn.ConnectionString = ConnectionString;
                Conn.Open();

            }
            catch (SqlException e)
            {
                SqlConnection.ClearAllPools();
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void cnClose()
        {
            try
            {
                if ((Conn != null))
                {
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Conn = null;
            }
        }
        #endregion

        #region "Date Modifier"
        public string DateModifier(string SqlStr)
        {
            try
            {
                string[] strDate = null;
                string strMonth = string.Empty;
                string strDay = string.Empty;
                System.Text.RegularExpressions.Regex re = null;
                strDate = System.Text.RegularExpressions.Regex.Split(SqlStr, "([0-9]+[-][0-9]+[-][0-9]+)");
                //if (strDate.Length == 1)
                //{
                //    strDate = System.Text.RegularExpressions.Regex.Split(SqlStr, "([0-9]+[/][0-9]+[/][0-9]+)");
                //}
                //for (Int16 i = 0; i <= strDate.Length - 1; i++)
                //{
                //    if (Information.IsDate(strDate[i]) == true)
                //    {
                //        if (DateAndTime.Day(strDate[i]).ToString().Length == 1)
                //        {
                //            strDay = "0" + DateAndTime.Day(strDate[i]);
                //        }
                //        else
                //        {
                //            strDay = DateAndTime.Day(strDate[i]);
                //        }
                //        if (DateAndTime.Month(strDate[i]).ToString().Length == 1)
                //        {
                //            strMonth = "0" + DateAndTime.Month(strDate[i]);
                //        }
                //        else
                //        {
                //            strMonth = DateAndTime.Month(strDate[i]);
                //        }
                //        string st = DateAndTime.Year(strDate[i]) + "/" + strMonth + "/" + strDay;
                //        SqlStr = Strings.Replace(SqlStr, strDate[i], st);
                //    }
                //}
                return SqlStr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public string ExecuteSP(string spName, ref SqlParameter[] @params)
        {
            try
            {
                cnOpen();
                SqlCommand cmd = new SqlCommand(spName, Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;

                for (int i = 0; i <= @params.Length - 1; i++)
                {
                    cmd.Parameters.Add(@params[i]);
                }

                string retval = (cmd.ExecuteNonQuery()).ToString();

                return retval;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnClose();
            }
        }

        public DataSet OpenDataSet(string TableName, string strQuery, DataSet dSet = null, Int16 TimeOut = 30)
        {
            SqlDataAdapter dAdapter = null;
            DataTable dTable = null;
            try
            {
                if (dSet == null)
                {
                    dSet = new System.Data.DataSet();
                }

                // Check whether the table with same name exists
                if (!string.IsNullOrEmpty(TableName.Trim()))
                {
                    if (dSet.Tables.Count > 0)
                    {
                        foreach (DataTable dTable_loopVariable in dSet.Tables)
                        {
                            dTable = dTable_loopVariable;
                            if (dTable.TableName == TableName)
                            {
                                dSet.Tables.Remove(dTable.TableName);
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                }
                cnOpen();

                dAdapter = new SqlDataAdapter(DateModifier(strQuery), Conn);
                dAdapter.SelectCommand.CommandTimeout = TimeOut;
                if (!string.IsNullOrEmpty(TableName.Trim()))
                {
                    dAdapter.Fill(dSet, TableName);
                }
                else
                {
                    dAdapter.Fill(dSet);
                }

                return dSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dTable = null;
                dAdapter = null;
                cnClose();
            }
        }

        public DataSet UpdateDataSet(DataSet dSet, string strQuery = "")
        {
            SqlDataAdapter dAdapter = null;
            SqlCommandBuilder cBuilder = null;
            DataTable dTable = null;
            string strSQL = null;
            string TableName = null;
            try
            {
                cnOpen();
                if (strQuery.Trim() != string.Empty)
                {
                    strSQL = strQuery;
                    dAdapter = new SqlDataAdapter(strSQL, Conn);
                    cBuilder = new SqlCommandBuilder(dAdapter);
                    dAdapter.Update(dSet.Tables[0]);
                }
                else
                {
                    foreach (DataTable dTable_loopVariable in dSet.Tables)
                    {
                        dTable = dTable_loopVariable;
                        TableName = dTable.TableName;
                        strSQL = "Select * From " + TableName;
                        dAdapter = new SqlDataAdapter(strSQL, Conn);
                        cBuilder = new SqlCommandBuilder(dAdapter);
                        dAdapter.Update(dSet, TableName);
                    }
                }

                return dSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dTable = null;
                cBuilder = null;
                dAdapter = null;
                cnClose();
            }
        }

        public bool UpdateWithTrans(DataSet dSetFirst, DataSet dSetSecond)
        {
            SqlTransaction Transaction = null;
            bool blnOk = false;
            try
            {
                cnOpen();
                Transaction = Conn.BeginTransaction();

                blnOk = Update_DataSet(dSetFirst, Transaction, Conn);
                if (blnOk)
                    blnOk = Update_DataSet(dSetSecond, Transaction, Conn);

                if (blnOk)
                {
                    Transaction.Commit();
                }
                else
                {
                    Transaction.Rollback();
                }

                return blnOk;
            }
            catch (Exception ex)
            {
                if ((Transaction != null))
                    Transaction.Rollback();
                throw ex;
            }
            finally
            {
                cnClose();
            }
        }

        public bool UpdateMultiWithTrans(IEnumerable dSets, Int16 TotDataSets)
        {
            SqlTransaction Transaction = null;
            bool blnOk = false;
            DataSet dSetTr = null;
            try
            {
                cnOpen();
                Transaction = Conn.BeginTransaction();

                foreach (DataSet dSetTr_loopVariable in dSets)
                {
                    dSetTr = dSetTr_loopVariable;
                    blnOk = Update_DataSet(dSetTr, Transaction, Conn);
                    if (blnOk == false)
                        break; // TODO: might not be correct. Was : Exit For
                }

                if (blnOk)
                {
                    Transaction.Commit();
                }
                else
                {
                    Transaction.Rollback();
                }

                return blnOk;
            }
            catch (Exception ex)
            {
                if ((Transaction != null))
                    Transaction.Rollback();
                throw ex;
            }
            finally
            {
                cnClose();
            }
        }

        //Use For Transaction Maintain
        public bool Update_DataSet(DataSet dSet, SqlTransaction mTrans = null, System.Data.SqlClient.SqlConnection objConn = null, string strQuery = "")
        {
            SqlDataAdapter dAdapter = null;
            SqlCommandBuilder cBuilder = null;
            DataTable dTable = null;
            string strSQL = null;
            string TableName = null;
            try
            {
                if (objConn == null)
                {
                    cnOpen();
                }
                else
                {
                    Conn = objConn;
                }

                if (strQuery.Trim() != string.Empty)
                {
                    strSQL = strQuery;
                    dAdapter = new SqlDataAdapter(strSQL, Conn);
                    dAdapter.SelectCommand.Transaction = mTrans;
                    cBuilder = new SqlCommandBuilder(dAdapter);
                    dAdapter.Update(dSet.Tables[0]);
                }
                else
                {
                    foreach (DataTable dTable_loopVariable in dSet.Tables)
                    {
                        dTable = dTable_loopVariable;
                        TableName = dTable.TableName;
                        strSQL = "Select * From " + TableName;
                        dAdapter = new SqlDataAdapter(strSQL, Conn);
                        dAdapter.SelectCommand.Transaction = mTrans;
                        cBuilder = new SqlCommandBuilder(dAdapter);
                        dAdapter.Update(dSet, TableName);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dTable = null;
                cBuilder = null;
                dAdapter = null;
                if (objConn == null)
                {
                    cnClose();
                }
            }
        }

        public string ExecuteScalar(string SqlStr)
        {
            SqlCommand cmd = null;
            string sRetVal = string.Empty;
            try
            {
                cnOpen();
                cmd = new SqlCommand(DateModifier(SqlStr), Conn);
                sRetVal = (string)(cmd.ExecuteScalar());

                return sRetVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnClose();
            }
        }

        //Without Date Modifier
        public string ExecuteScalarWDM(string SqlStr)
        {
            SqlCommand cmd = null;
            string sRetVal = string.Empty;
            try
            {
                cnOpen();
                cmd = new SqlCommand(SqlStr, Conn);
                sRetVal = cmd.ExecuteScalar().ToString();

                return sRetVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnClose();
            }
        }

        public int ExecuteQuery(string strQuery)
        {
            int RecordsAffected = 0;
            SqlCommand cmd = null;
            try
            {
                cnOpen();
                cmd = new SqlCommand(DateModifier(strQuery), Conn);
                RecordsAffected = cmd.ExecuteNonQuery();

                return RecordsAffected;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnClose();
                cmd = null;
            }
        }

        public int ExecuteQueryWDM(string strQuery, Int16 TimeOut = 30)
        {
            int RecordsAffected = 0;
            SqlCommand cmd = default(SqlCommand);
            try
            {
                cnOpen();
                cmd = new SqlCommand(strQuery, Conn);
                cmd.CommandTimeout = TimeOut;
                RecordsAffected = cmd.ExecuteNonQuery();

                return RecordsAffected;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnClose();
                cmd = null;
            }
        }

        public SqlDataReader ExecuteReader(string strQuery)
        {
            SqlCommand cmd = null;
            SqlDataReader dReader = null;
            try
            {
                cnOpen();
                cmd = new SqlCommand(DateModifier(strQuery), Conn);
                dReader = cmd.ExecuteReader();

                return dReader;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnClose();
                cmd = null;
            }
        }

        public int GetNextAutoNo(string TableName, string FieldName, string WhereCondition = "")
        {
            string strQuery = string.Empty;
            DataSet dSet = new DataSet();
            try
            {
                if (string.IsNullOrEmpty(WhereCondition))
                {
                    strQuery = "Select Max(" + FieldName + ") As AutoNo From " + TableName + " WITH (NOLOCK)";
                }
                else
                {
                    strQuery = "Select Max(" + FieldName + ") As AutoNo From " + TableName + " WITH (NOLOCK) where " + WhereCondition;
                }

                dSet = OpenDataSet(TableName, DateModifier(strQuery));

                if (dSet.Tables[0].Rows[0].IsNull(0))
                {
                    return 1;
                }
                else
                {
                    return Convert.ToInt32(dSet.Tables[0].Rows[0]["AutoNo"]) + 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dSet = null;
            }
        }

        public DataSet GetDSWithSP(string spName, ref SqlParameter[] @params)
        {
            DataSet dSet = new DataSet();
            SqlDataAdapter dAdapter = null;
            try
            {
                cnOpen();
                SqlCommand cmd = new SqlCommand(spName, Conn);
                cmd.CommandType = CommandType.StoredProcedure;

                for (int i = 0; i <= @params.Length - 1; i++)
                {
                    cmd.Parameters.Add(@params[i]);
                }

                dAdapter = new SqlDataAdapter(cmd);
                dAdapter.Fill(dSet);

                return dSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnClose();
            }
        }

        public int RecordCount(string TableName, string FieldName, string FieldValue, string Condition)
        {
            string SqlStr = null;
            int iCnt = 0;
            try
            {
                SqlStr = "Select Count(*) From " + TableName + " Where " + FieldName + "='" + FieldValue + "' " + Condition;
                iCnt = Convert.ToInt32(ExecuteScalar(SqlStr));

                return iCnt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void GetDBName()
        //{
        //    Conn.ConnectionString = DatabaseName;
        //}

        public long ExecuteQuery_IC(string strQuery)
        {
            try
            {
                this.cnOpen();
                new SqlCommand(this.DateModifier(strQuery), this.Conn).ExecuteNonQuery();
                return Conversions.ToLong(new SqlCommand("SELECT @@IDENTITY", this.Conn).ExecuteScalar());
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                throw ex;
            }
            finally
            {
                this.cnClose();
            }
        }

        public long ExecuteQueryParameterize_IC(string key, string value, string strQuery, String KeysplitChar = ",", String valuesplitChar = ",")
        {
            int RecordsAffected;
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            SqlCommand cmd;
            string[] keyArr;
            string[] valueArr;
            int i = 0;
            try
            {
                keyArr = key.Split(KeysplitChar.ToArray());
                valueArr = value.Split(valuesplitChar.ToArray());
                cnOpen();
                cmd = new SqlCommand(DateModifier(strQuery), Conn);
                foreach (string temp in keyArr)
                {
                    if ((temp != String.Empty))
                    {
                        cmd.Parameters.AddWithValue(temp, valueArr[i]);
                    }

                    i = i + 1;
                }

                RecordsAffected = cmd.ExecuteNonQuery();
                return Conversions.ToLong(new SqlCommand("SELECT @@IDENTITY", this.Conn).ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnClose();
                cmd = null;
            }
        }

        public int ExecuteQueryParameterize(string key, string value, string strQuery, String KeysplitChar = ",", String valuesplitChar = ",")
        {
            int RecordsAffected;
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            SqlCommand cmd;
            string[] keyArr;
            string[] valueArr;
            int i = 0;
            try
            {
                keyArr = key.Split(KeysplitChar.ToArray());
                valueArr = value.Split(valuesplitChar.ToArray());
                cnOpen();
                cmd = new SqlCommand(DateModifier(strQuery), Conn);
                foreach (string temp in keyArr)
                {
                    if ((temp != String.Empty))
                    {
                        cmd.Parameters.AddWithValue(temp, valueArr[i]);
                    }

                    i = i + 1;
                }

                RecordsAffected = cmd.ExecuteNonQuery();
                return RecordsAffected;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cnClose();
                cmd = null;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSerializer
{
    public class DBBase
    {
        SqlConnection cnn = null;
        public DBBase()
        {

        }

        /// <summary>
        /// Get DB Connection 
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>SqlConnection</returns>
        public static SqlConnection GetConnection()
        {
            string connectionStr = ConfigurationManager.ConnectionStrings["DataAccess"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionStr);
            return conn;
        }

        //public void AddTest(byte[] test1, byte[] test2, byte[] test3)
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["DataAccess"].ConnectionString;
        //    cnn = new SqlConnection(connectionString);
        //    cnn.Open();
        //    SqlCommand cmdDB = new SqlCommand("[MyTest]", cnn);
        //    cmdDB.CommandType = CommandType.StoredProcedure;
        //    cmdDB.Parameters.Add("@TestValue1", SqlDbType.VarBinary).Value = test1;
        //    cmdDB.Parameters.Add("@TestValue2", SqlDbType.VarBinary).Value = test2;
        //    cmdDB.Parameters.Add("@TestValue3", SqlDbType.VarBinary).Value = test3;
        //    cmdDB.ExecuteNonQuery();
        //    cnn.Close();

        //}

        /// <summary>
        /// Get Error Log Excel File Data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetErrorLogExcelData()
        {

            string querystring;
            
            DataSet ds = null;


            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DataAccess"].ConnectionString;
                cnn = new SqlConnection(connectionString);
                querystring = "Select distinct el.Id,el.ErrorInformation,el.ErrorDate, el.ErrorCode, el.ErrorType, el.VersionId," +
"el.CatheterID,	el.SystemStatesID, ss.[Description] as SystemStates,	el.UserID,	el.IsUsingICB,	el.IsUsingRemote, el.CatheterContainer, " +
"v.Software,	v.ControlFirmware   ,	v.ControlFirmwareBootLoader ,	v.CPLDFirmware  ,	v.RemoteFirmware    ,	v.RemoteFirmwareBootLoader  ,	v.PatientFirmware   ,	v.PatientFirmwareBootLoader ,	v.RepeaterFirmware  ,	v.RepeaterFirmwareBootLoader    ,	v.ICBFirmware   ,	v.ICBFirmwareBootLoader ,	v.CatheterFirmware  ,	v.DataBaseVersion   ,	v.StartDate ," +
"c.SerialNumber,	c.FirmwareVersion   ,	c.CatheterExpirationDate    ,	c.LastUseDate   ,	c.NumberOfInjection ,	c.Lot   ,	c.IsUsedForEngineering  ,	c.OverloadedCatheterID  ,	c.CatheterTypeID," +
"em.[Message],	em.SolutionMessage  ,	em.CryterionMessage " +
"from   ErrorLog el inner join[dbo].[ConsoleVersion] v on el.VersionId=v.Id " +
"inner join ErrorMessages em on el.ErrorCode =em.ErrorCode " +
"inner join SystemStates ss on ss.StateID = el.[SystemStatesID]" +
"Left join CatheterInformations c on  el.CatheterID =c.Id " +
"where el.ErrorType = em.Type and em.LanguageId =1";

                SqlCommand cmd = new SqlCommand(querystring, cnn);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adpter = new SqlDataAdapter(cmd);
                ds = new DataSet();
                cnn.Close();
                adpter.Fill(ds);
                return ds;
            }

            catch (Exception e)
            {
                cnn.Close();
                return ds;
            }
        }

        /// <summary>
        /// Encrypt value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>Task</returns>

        public byte[] EncryptedValue(string value)
        {
            string query = "";
            SqlConnection cnn = null;
            byte[] encryptedValue = null;
            string connectionString = ConfigurationManager.ConnectionStrings["DataAccess"].ConnectionString;

            cnn = new SqlConnection(connectionString);
            query = "SELECT [dbo].[EncryptedValue] (N'" + value + "','Sorry, the key is not right!')";
            var cmd = new SqlCommand("query", cnn);
            try
            {
                cnn.Open();

                cmd.CommandText = query;
                encryptedValue = (byte[])(cmd.ExecuteScalar());
                cnn.Close();
            }
            catch (Exception ex)
            {
               // File.AppendAllText(@"C:\Program Files\BSC\Smart Ablation System\BugTracking_log.txt", ex.ToString() + DateTime.Now.ToString() + Environment.NewLine);
            }
            return encryptedValue;
        }

        /// <summary>
        /// Decrypt value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>Task</returns>
        public string DecryptedValue(byte[] value, int valueType)
        {

            string query = "";
            string decryptedvalue = "";
            SqlConnection cnn = null;

            if (valueType == 1)
                decryptedvalue = "In Decrypt Function";
            else if (valueType == 2)
                decryptedvalue = "1800-01-01";
            else if (valueType == 4)
                decryptedvalue = "-1";
            else
                decryptedvalue = "0";

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DataAccess"].ConnectionString;
                cnn = new SqlConnection(connectionString);
                query = "SELECT [dbo].[DecryptedValue] ( @EValue,'Sorry, the key is not right!')  ";
                SqlCommand cmd = new SqlCommand("query", cnn);
                cmd.Parameters.Add("@EValue", SqlDbType.VarBinary, value.Length).Value = value;

                cnn.Open();
                cmd.CommandText = query;
                decryptedvalue = (string)cmd.ExecuteScalar();

                cnn.Close();
            }
            catch (Exception ex)
            {
              //  File.AppendAllText(@"C:\Program Files\BSC\Smart Ablation System\BugTracking_log.txt", ex.ToString() + DateTime.Now.ToString() + Environment.NewLine);
            }
            return decryptedvalue;
        }



    }

}


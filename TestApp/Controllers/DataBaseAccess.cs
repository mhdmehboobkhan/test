using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TestApp.Controllers
{
    public class DataBaseAccess
    {
        public const string pDBError_Prefix = "The system is unable to process the request, please check error: ";
        public const int pSQL_CommandTimeOut = 6000000;
        //Data Source=10.183.240.69;Initial Catalog=BlockedCustomer;user=sa;password=systek_0808
        //Data Source=windevsrv-01;Initial Catalog=BlockedCustomer;user=sa;password=M1lkyB@r

        public static string m_ConnectionString = "Data Source=windevsrv-01;Initial Catalog=TestingPurpose;user=sa;password=M1lkyB@r";

        SqlConnection SConnection = new SqlConnection(m_ConnectionString);
        SqlCommand SCmd = new SqlCommand();

        //---Fill SqlDataReader By Stored Procedure New
        public List<dynamic> Get_DT_SP(string vSPName, CommandType comandtype,SqlParameter[] param = null)
        {
            SConnection.Open();
            List<dynamic> foo = new List<dynamic>();
            List<dynamic> foo2 = new List<dynamic>();

            try
            {
                if (SConnection.State != ConnectionState.Open)
                    SConnection.Open();
                SCmd = new SqlCommand();
                SCmd.Connection = SConnection;
                SCmd.CommandText = vSPName;
                SCmd.CommandTimeout = pSQL_CommandTimeOut;
                SCmd.CommandType = comandtype;
                if(param != null)
                    SCmd.Parameters.AddRange(param);
                using (var reader = SCmd.ExecuteReader())
                {
                    foo2 = reader.Cast<dynamic>().ToList();
                    if (reader.HasRows)
                    {
                        var names = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                        foreach (IDataRecord record in reader as IEnumerable)
                            foo.Add(names.ToDictionary(n => n, n => record[n]));

                        
                    }
                }


                return foo2;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                SConnection.Close();
            }
        }

        public DataSet GetDataSet_BySQLTransactions(SqlCommand pScmd, SqlParameter[] param)
        {
            SqlDataAdapter SDataAdapter = new SqlDataAdapter();
            DataSet dsData = new DataSet();
            try
            {
                pScmd.CommandType = CommandType.StoredProcedure;
                pScmd.Parameters.AddRange(param);
                pScmd.CommandTimeout = pSQL_CommandTimeOut;
                SDataAdapter.SelectCommand = pScmd;
                SDataAdapter.Fill(dsData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsData;
        }
    }
}
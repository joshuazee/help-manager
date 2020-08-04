using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace CMHL.Lawer
{
    public class DataBaseHelper
    {
        public static int ExecuteNonQuery(string sql, out string err)
        {
            int result = 0;
            err = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfCenter.Connection))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        result = cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (SqlException sqlEx)
            {
                err = sqlEx.ToString();
            }

            return result;
        }

        public static object ExecuteScalar(string sql,out string err)
        {
            object result = null;
            err = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfCenter.Connection))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        result = cmd.ExecuteScalar();
                    }
                    conn.Close();
                }
            }
            catch (SqlException sqlEx)
            {
                err = sqlEx.ToString();
            }

            return result;
        }

        public static DataTable ExecuteTable(string sql, out string err, int tableIndex = 0)
        {
            DataSet ds = new DataSet();
            DataTable result= new DataTable();
            err = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfCenter.Connection))
                {
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    da.Fill(ds);
                    result = ds.Tables[tableIndex];
                }
            }
            catch (SqlException sqlEx)
            {
                err = sqlEx.ToString();
            }

            return result;
        }
    }
}

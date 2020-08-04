using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CMHL.Lawer
{
    /// <summary>
    /// 日志编码
    /// 操作方法错误 1***
    /// 系统方法错误 2***
    /// 日志错误 3***
    /// </summary>
    public static class SystemLog
    {
        private static string ErrorLogTableName()
        {
            string tableName = "sys_error_log_" + DateTime.Now.ToString("yyyyMM");

            #region 建表
            //string sql = "", error = "";
            //try
            //{
            //    if (DateTime.Now.Day == 1)
            //    {
            //        sql = "SELECT count(*) FROM sysobjects where name = '" + tableName + "' and xtype = 'U'";
            //        int count = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
            //        if (error == "" && count == 0)
            //        {
            //            sql = "CREATE TABLE [dbo].[" + tableName + @"](
            //                       [error_id][int] IDENTITY(1, 1) NOT NULL,
            //                       [error_code] [varchar](20) NULL,
            //                    [error_message] [varchar](200) NULL,
            //                    [error_content] [varchar](max) NULL,
            //                    [error_stack] [varchar](max) NULL,
            //                    [error_time] [datetime] NULL,
            //                       CONSTRAINT[PK_" + tableName + @"] PRIMARY KEY CLUSTERED
            //                       (
            //                        [error_id] ASC
            //                       )WITH(PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON[PRIMARY]
            //                    ) ON[PRIMARY]";
            //            DataBaseHelper.ExecuteNonQuery(sql, out error);
            //            if(error != "")
            //            {
            //                throw new Exception(error);
            //            }
            //        }
            //        else if(error != "")
            //        {
            //            throw new Exception(error);
            //        }
            //    }
            //}
            //catch(Exception ex)
            //{
            //    WriteErrorLog2("日志表创建失败", "9999", ex.Message, ex.StackTrace);
            //}
            #endregion

            return tableName;
        }
        public static bool WriteErrorLog(string msg = "", string code = "0000", string error = "", string stackInfo = "")
        {
            string sql = "";
            try
            {
                ///语句需要修改
                sql = string.Format("insert into {5} values({0},'{1}','{2}','{3}','{4}')", code, msg, error.Replace("'", "''"), stackInfo.Replace("'", "''"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ErrorLogTableName());
                string logErr = "";
                int count  = DataBaseHelper.ExecuteNonQuery(sql, out logErr);
                if(logErr != "")
                {
                    throw new Exception(logErr);
                }
                else if(count > 0)
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                WriteErrorLog2("日志写入失败:" + sql, "3001", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public static bool WriteOperationLog(int userId, string title, string content)
        {
            try
            {
                ///语句需要修改
                string sql = string.Format("insert into sys_operation_log values({0},'{1}','{2}','{3}')", userId, title, content, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                string logErr = "";
                int count = DataBaseHelper.ExecuteNonQuery(sql, out logErr);
                if (logErr != "")
                {
                    throw new Exception(logErr);
                }
                else if (count > 0)
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                WriteErrorLog2("日志写入失败:" + ex.Message, "3001", ex.StackTrace);
            }
            return false;
        }

        private static FileStream GetLocalLogFile()
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            string directory = AppHome.Buffer + "\\logs\\";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string path = directory + "Log - " + date + ".log";
            FileStream fs = File.Open(path, FileMode.Append);
            return fs;
        }

        /// <summary>
        /// 本地文件错误日志(只在数据库错误时记录数据库错误，不做他用)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="code"></param>
        /// <param name="error"></param>
        /// <param name="stackInfo"></param>
        public static void WriteErrorLog2(string msg = "", string code = "0000", string error = "", string stackInfo = "")
        {
            string content = "";
            FileStream fs = GetLocalLogFile();

            content += "==============================\r\n";

            content += "time:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";

            content += "statusCode:" + code + "\r\n";

            content += "message:" + msg + "\r\n";

            content += "error:" + error + "\r\n";

            content += "stack:" + stackInfo + "\r\n";

            content += "==============================\r\n";

            byte[] bytes = Encoding.UTF8.GetBytes(content);
            fs.Write(bytes, 0, bytes.Length);
            fs.Dispose();
            fs.Close();
        }
    }
}

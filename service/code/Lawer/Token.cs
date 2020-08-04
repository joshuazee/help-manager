using CMHL.Lawer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CMHL.Lawer
{
    public class Token
    {
        private const int token_time = 60*60*1000;
        /// <summary>
        /// 成功登录后更新token，并退出原用户（仅限web后台系统使用）
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="client">客户端ID</param>
        /// 在此示范标准服务写法(参数验证，日志操作，数据库操作等)
        public static string UpdateUserToken(int id, string client)
        {
            if(id == 0)
            {
                return "请传入id参数";
            }
            if(string.IsNullOrWhiteSpace(client))
            {
                return "请传入client参数";
            }
            string token = "";
            try
            {
                string sql = string.Format("select top 1 * from sys_user_login where user_id = {0} order by create_time desc", id);
                string error = "";
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    if(dt.Rows.Count != 0)
                    {
                        DataRow dr = dt.Rows[0];
                        DateTime oldTime = DateTime.Parse(dr["create_time"].ToString());
                        if (DateTime.Now - oldTime < new TimeSpan(token_time))
                        {
                            //上一个token还未到期，需要将上一个用户踢下线
                            string oldClient = dr["client_id"].ToString();
                        }
                    }
                    string where = "client_id = '" + client + "' and token = '" + new Guid().ToString() + "'";
                    sql = string.Format("update sys_user_login set {0} where user_id = {1}", where, id);
                    token = DataBaseHelper.ExecuteScalar(sql, out error).ToString();
                    if (error != "")
                    {
                        throw new Exception(error);
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch(Exception ex)
            {
                SystemLog.WriteErrorLog("token更新失败", "2001", ex.Message, ex.StackTrace);
            }

            return token;
        }

        public static bool ValidToken(int id, string token)
        {
            try
            {
                return true;
            }
            catch(Exception ex)
            {

            }
            return false;
        }
    }
}

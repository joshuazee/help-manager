using CMHL.Entity;
using CMHL.Lawer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrugControlBusiness
{
    public partial class Rest : IRest
    {
        public bool AddSign(out string str_error, int user1, string location1, string address1, string time1, string photo, string type, string state, string remark, int user2, string location2, string address2, string time2, string appointment, string appointment_time)
        {
            str_error = "";
            string sql, error;
            try
            {
                sql = string.Format(@"insert into user_sign(user_id1, sign_location1, sign_address1, sign_time1, sign_photo, sign_type, sign_state, sign_remark, user_id2, sign_location2, sign_address2, sign_time2, sign_appointment, sign_appointment_time, sign_distance, create_time, delete_mark) 
                                                     values({0}, '{1}', '{2}'", user1, location1, address1);
                if (string.IsNullOrWhiteSpace(time1))
                {
                    sql += ", null";
                }
                else
                {
                    sql += ", '" + time1 + "'";
                }
                sql += string.Format(", '{0}', '{1}', '{2}', '{3}', {4}, '{5}', '{6}'", photo, type, state, remark, user2, location2, address2);
                if (string.IsNullOrWhiteSpace(time2))
                {
                    sql += ", null";
                }
                else
                {
                    sql += ", '" + time2 + "'";
                }
                sql += string.Format(", '{0}'", appointment);
                if (string.IsNullOrWhiteSpace(appointment_time))
                {
                    sql += ", null";
                }
                else
                {
                    sql += ", '" + appointment_time + "'";
                }
                sql += string.Format(", 0, getdate(), 0)");
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if (count > 0 && error == "")
                {
                    if (state != "已完成")
                    {
                        sql = "select user_id, user_mobile,user_name from sys_user where user_id = " + user1 + 
                                      " union select user_id, user_mobile,user_name from sys_user where user_id = " + user2;
                        DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                        string mobile = dt.Rows[0][1].ToString();
                        string name = dt.Rows[1][2].ToString();
                        if(error == "")
                        {
                            Utilities.SendMailRY(mobile, 2, ConfCenter.MailTemplateID1, new string[] { name });
                        }
                        else
                        {
                            throw new Exception(error);
                        }
                    }
                    return true;
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("新增签到信息失败", "1104", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public bool UpdateSign(out string str_error, int id, int user1, string location1, string address1, string time1, string photo, string type, string state, string remark, int user2, string location2, string address2, string time2, string appointment)
        {
            str_error = "";
            string sql, error;
            string where = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(location1))
                {
                    if(location1 == "4.9E-324,4.9E-324" || location1== "0,0")
                    {
                        throw new Exception("手机定位失败");
                    }
                    where += string.Format(", sign_location1 = '{0}'", location1);
                }
                if (!string.IsNullOrWhiteSpace(address1))
                {
                    where += string.Format(", sign_address1 = '{0}'", address1);
                }
                if (!string.IsNullOrWhiteSpace(time1))
                {
                    where += string.Format(", sign_time1 = '{0}'", time1);
                }
                if (!string.IsNullOrWhiteSpace(photo))
                {
                    where += string.Format(", sign_photo = '{0}'", photo);
                }
                if (!string.IsNullOrWhiteSpace(state))
                {
                    where += string.Format(", sign_state = '{0}'", state);
                }
                if (!string.IsNullOrWhiteSpace(location2))
                {
                    where += string.Format(", sign_location2 = '{0}'", location2);
                }
                if (!string.IsNullOrWhiteSpace(address2))
                {
                    where += string.Format(", sign_address2 = '{0}'", address2);
                }
                if (!string.IsNullOrWhiteSpace(time2))
                {
                    where += string.Format(", sign_time2 = '{0}'", time2);
                }
                if (!string.IsNullOrWhiteSpace(appointment))
                {
                    where += string.Format(", sign_appointment = '{0}'", appointment);
                }
                if (where != "")
                {
                    where = where.Substring(1);
                    where += ", update_time = getdate()";
                }
                else
                {
                    throw new Exception("请传入更新参数");
                }
                sql = string.Format("update user_sign set {0} where sign_id = {1} and delete_mark = 0", where, id);
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if (count > 0 && error == "")
                {
                    //写入操作日志
                    return true;
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("更新签到信息失败", "1104", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public bool DeleteSign(out string str_error, int id)
        {
            str_error = "";
            string sql;
            try
            {
                sql = string.Format("update user_sign set delete_mark = 1, update_time = getdate() where sign_id = {0}", id);
                string error = "";
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if (count > 0 && error == "")
                {
                    //写入操作日志
                    return true;
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("删除签到信息失败", "1104", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public SignInfo[] QuerySign(out string str_error, int leader, int user, int year, int month, string state, int dep)
        {
            List<SignInfo> result = new List<SignInfo>();
            str_error = "";
            string sql, error;
            DataTable dt;
            string where = "s.delete_mark = 0";
            try
            {
                if(user != 0)
                {
                    where += string.Format(" and user_id1 = {0}", user);
                }
                else if (dep != 0)
                {
                    sql = string.Format(@"select stuff((select ',' + convert(varchar(100),u.user_id )
                                                        from sys_user u, sys_dep d, sys_user_dep_relationship t, sys_role r, sys_user_role_relationship t2
                                                        where u.user_id = t.user_id and t.dep_id = d.dep_id and u.user_id = t2.user_id and t2.role_id = r.role_id
                                                        and u.delete_mark = 0 and d.dep_id = {0} and r.role_level = {1}
                                                        for xml path('')),1,1,'')", dep, ConfCenter.ImportantUserRoleLevel);
                    string userid = DataBaseHelper.ExecuteScalar(sql, out error).ToString();
                    if (userid != "" && error == "")
                    {
                        where += string.Format(" and user_id1 in ({0})", userid);
                    }
                }
                else if (leader != 0)
                {
                    List<int> ids = new List<int>();
                    sql = string.Format("exec QuerySubUserByUserID {0}, {1}", leader, ConfCenter.ImportantUserRoleLevel);
                    dt = DataBaseHelper.ExecuteTable(sql, out error);
                    if (error == "")
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            ids.Add(Convert.ToInt32(dr[0]));
                        }

                        if(ids.Count > 0)
                        {
                            where += string.Format(" and user_id1 in ({0})", string.Join(",", ids));
                        }
                        else
                        {
                            where += " and user_id1 = 0";
                        }
                    }
                }
                else
                {
                    throw new Exception("人员相关查询参数错误");
                }
                if (year != 0 && month != 0)
                {
                    int nextMonth = 0;
                    int nextYear = 0;
                    if (month == 12)
                    {
                        nextMonth = 1;
                        nextYear = year + 1;
                    }
                    else
                    {
                        nextMonth = month + 1;
                        nextYear = year;
                    }

                    where += string.Format(" and s.create_time >= '{0}-{1}-1 0:0:0' and s.create_time < '{2}-{3}-1 0:0:0'", year, month, nextYear, nextMonth);
                }
                if (!string.IsNullOrWhiteSpace(state))
                {
                    where += string.Format(" and sign_state = '{0}'", state);
                }
                //if (!string.IsNullOrWhiteSpace(mobile))
                //{
                //    where += string.Format(" and u1.user_mobile = '{0}", mobile);
                //}
                //if (!string.IsNullOrWhiteSpace(identity))
                //{
                //    where += string.Format(" and u1.user_identity = '{0}'", identity);
                //}


                sql = string.Format(@"select s.sign_id, user_id1, sign_location1, sign_address1, sign_time1, sign_photo, sign_type, sign_state, sign_remark, user_id2, sign_location2, sign_address2, sign_time2, sign_appointment, sign_appointment_time, sign_distance, s.update_time, u1.user_name, u2.user_name from user_sign s 
                                                     left join sys_user u1 on u1.user_id = s.user_id1 and u1.delete_mark = 0
                                                     left join sys_user u2 on u2.user_id = s.user_id2 and u2.delete_mark = 0
                                                     where {0} order by s.create_time desc", where);

                dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error != "")
                {
                    throw new Exception(error);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    SignInfo info = new SignInfo();
                    info.id = Convert.ToInt32(dr[0]);
                    info.user1 = Convert.ToInt32(dr[1]);
                    info.location1 = dr[2].ToString();
                    info.address1 = dr[3].ToString();
                    string tmp = dr[4].ToString();
                    if (tmp != "")
                    {
                        info.time1 = Convert.ToDateTime(tmp).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    info.photo = dr[5].ToString();
                    info.type = dr[6].ToString();
                    info.state = dr[7].ToString();
                    info.remark = dr[8].ToString();
                    info.user2 = Convert.ToInt32(dr[9]);
                    info.location2 = dr[10].ToString();
                    info.address2 = dr[11].ToString();
                    tmp = dr[12].ToString();
                    if (tmp != "")
                    {
                        info.time2 = Convert.ToDateTime(tmp).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    info.appointment = dr[13].ToString();
                    tmp = dr[14].ToString();
                    if (tmp != "")
                    {
                        info.appointment_time = Convert.ToDateTime(tmp).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    tmp = dr[15].ToString();
                    if (tmp != "")
                    {
                        info.distance = Convert.ToDouble(tmp);
                    }
                    tmp = dr[16].ToString();
                    if (tmp != "")
                    {
                        info.update_time = Convert.ToDateTime(tmp).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    info.user1_name = dr[17].ToString();
                    info.user2_name = dr[18].ToString();
                    result.Add(info);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询签到信息失败", "1104", ex.Message, ex.StackTrace);
            }

            return result.ToArray();
        }

        public SignInfo[] QuerySignForApp(out string str_error, int user1, int user2, string type, string state, string time)
        {
            List<SignInfo> result = new List<SignInfo>();
            str_error = "";
            string sql, error;
            DataTable dt;
            string where = "s.delete_mark = 0";
            try
            {
                if(user1 != 0)
                {
                    where += string.Format(" and user_id1 = {0}", user1);
                }
                else if (user2 != 0)
                {
                    List<int> ids = new List<int>();
                    sql = string.Format("exec QuerySubUserByUserID {0}, {1}", user2, ConfCenter.ImportantUserRoleLevel);
                    dt = DataBaseHelper.ExecuteTable(sql, out error);
                    if (error == "")
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            ids.Add(Convert.ToInt32(dr[0]));
                        }

                        if(ids.Count > 0)
                        {
                            where += string.Format(" and user_id1 in ({0})", string.Join(",", ids));
                        }
                    }
                    //where += string.Format(" and user_id2 = {0}", user2);
                }
                if (!string.IsNullOrWhiteSpace(type))
                {
                    where += string.Format(" and sign_type = '{0}'", type);
                }
                if (!string.IsNullOrWhiteSpace(state))
                {
                    if(state.IndexOf(',') >= 0)
                    {
                        string[] temp = state.Split(',');
                        string w_temp = "";
                        for (int i = 0; i < temp.Length; i++)
                        {
                            if(w_temp != "")
                            {
                                w_temp += " or ";
                            }
                            w_temp += string.Format("sign_state = '{0}'", temp[i]);
                        }
                        where += " and (" + w_temp + ")";
                    }
                    else
                    {
                        where += string.Format(" and sign_state = '{0}'", state);
                    }
                }
                if(!string.IsNullOrWhiteSpace(time))
                {
                    //if(time.IndexOf(' ') >= 0)
                    //{
                    //    time = time.Split(' ')[0];
                    //}
                    //where += string.Format(" and s.sign_time1 >= '{0} 0:0:0' and s.sign_time1 <= '{0} 23:59:59'", time);
                    where += string.Format(" and s.create_time >= '{0}'", time);
                }
                //else
                //{
                //    where += string.Format(" and s.create_time >= '{0}' and s.create_time <= '{1}'", DateTime.Now.Year + "-" + DateTime.Now.Month + "-1 0:0:0",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //}
                sql = string.Format(@"select s.sign_id, user_id1, sign_location1, sign_address1, sign_time1, sign_photo, sign_type, sign_state, sign_remark, user_id2, sign_location2, sign_address2, sign_time2, sign_appointment, sign_appointment_time, sign_distance, s.update_time, u1.user_name, u2.user_name from user_sign s 
                                                     left join sys_user u1 on u1.user_id = s.user_id1 and u1.delete_mark = 0
                                                     left join sys_user u2 on u2.user_id = s.user_id2 and u2.delete_mark = 0
                                                     where {0} order by sign_time1 desc, s.create_time desc", where);
                dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error != "")
                {
                    throw new Exception(error);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    SignInfo info = new SignInfo();
                    info.id = Convert.ToInt32(dr[0]);
                    info.user1 = Convert.ToInt32(dr[1]);
                    info.location1 = dr[2].ToString();
                    info.address1 = dr[3].ToString();
                    string tmp = dr[4].ToString();
                    if (tmp != "")
                    {
                        info.time1 = Convert.ToDateTime(tmp).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    info.photo = dr[5].ToString();
                    info.type = dr[6].ToString();
                    info.state = dr[7].ToString();
                    info.remark = dr[8].ToString();
                    info.user2 = Convert.ToInt32(dr[9]);
                    info.location2 = dr[10].ToString();
                    info.address2 = dr[11].ToString();
                    tmp = dr[12].ToString();
                    if (tmp != "")
                    {
                        info.time2 = Convert.ToDateTime(tmp).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    info.appointment = dr[13].ToString();
                    tmp = dr[14].ToString();
                    if (tmp != "")
                    {
                        info.appointment_time = Convert.ToDateTime(tmp).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    tmp = dr[15].ToString();
                    if (tmp != "")
                    {
                        info.distance = Convert.ToDouble(tmp);
                    }
                    tmp = dr[16].ToString();
                    if (tmp != "")
                    {
                        info.update_time = Convert.ToDateTime(tmp).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    info.user1_name = dr[17].ToString();
                    info.user2_name = dr[18].ToString();
                    result.Add(info);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询签到信息失败", "1104", ex.Message, ex.StackTrace);
            }
            return result.ToArray();
        }
    }
}

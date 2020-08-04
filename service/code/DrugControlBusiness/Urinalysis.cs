using CMHL.Entity;
using CMHL.Lawer;
using System;
using System.Collections.Generic;
using System.Data;

namespace DrugControlBusiness
{
    public partial class Rest: IRest
    {
        private bool AddUrinalysisRecord(out string str_error, int user1, string result, string remark, string photo)
        {
            string sql = "";
            string error = "";
            int count = 0;
            str_error = "";
            try
            {
                sql = string.Format(@"insert into 
                                                         urinalysis_records(user_id1, urinalysis_result, urinalysis_remark, urinalysis_photo, urinalysis_state, create_time, delete_mark) 
                                                         values({0}, '{1}', '{2}', '{3}', '待审核', getdate(), 0)", user1, result, remark, photo);
                count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if(error == "" && count > 0)
                {
                    return true;
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("添加尿检结果失败", "2901", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool UpdateUrinalysisRecord(out string str_error, int id, int user2, string result, string remark, string photo, string state)
        {
            string sql = "";
            string error = "";
            int count = 0;
            string where = "";
            str_error = "";
            try
            {
                if(!string.IsNullOrWhiteSpace(result))
                {
                    where += ",urinalysis_result = '" + result + "'";
                }
                if(user2 != 0)
                {
                    where += string.Format(",user_id2 = {0}", user2);
                }
                if(!string.IsNullOrWhiteSpace(remark))
                {
                    where += string.Format(",urinalysis_remark = '{0}'", remark);
                }
                if(!string.IsNullOrWhiteSpace(photo))
                {
                    where += string.Format(",urinalysis_photo = '{0}'", photo);
                }
                if(!string.IsNullOrWhiteSpace(state))
                {
                    where += ",urinalysis_state = '"+ state + "'";
                }
                if(where == "" || id == 0)
                {
                    throw new Exception("参数传入错误");
                }
                where = "update_time = getdate()" + where;
               sql = string.Format("update urinalysis_records set {0} where delete_mark = 0 and urinalysis_id = {1}", where, id);
                count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if (error == "" && count > 0)
                {
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
                SystemLog.WriteErrorLog("更新尿检结果失败", "2902", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool DeleteUrinalysisRecord(out string str_error, int id = 0)
        {
            string sql = "";
            string error = "";
            int count = 0;
            str_error = "";
            try
            {
                if(id == 0)
                {
                    throw new Exception("参数传入错误");
                }
                sql = string.Format("update urinalysis_records set delete_mark = 1 where urinalysis_id = {0} and delete_mark = 0", id);
                count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if (error == "" && count > 0)
                {
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
                SystemLog.WriteErrorLog("删除尿检结果失败", "2903", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private UrinalysisRecord[] QueryUrinalysisRecord(out string str_error, int user, int year, int month, int leader, string result, string state, int dep)
        {
            List<UrinalysisRecord> records = new List<UrinalysisRecord>();
            string sql = "";
            string error = "";
            DataTable dt;
            str_error = "";
            int intTemp = 0;
            string temp = "";
            string where = "";
            try
            {
                if (dep != 0)
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
                else if (leader != 0 && user == 0)
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

                        if (ids.Count > 0)
                        {
                            where += string.Format(" and user_id1 in ({0})", string.Join(",", ids));
                        }
                        else
                        {
                            where += " and user_id1 = 0";
                        }
                    }
                }
                else if (user != 0)
                {
                    where += string.Format(" and user_id1 = {0}", user);
                }
                else
                {
                    throw new Exception("查询参数错误");
                }
                //if(user != 0)
                //{
                //    where += string.Format(" and user_id1 = {0}", user);
                //}
                //else if(dep != 0)
                //{
                //    sql = string.Format(@"select stuff((select ',' + convert(varchar(100),u.user_id )
                //                                        from sys_user u, sys_dep d, sys_user_dep_relationship t, sys_role r, sys_user_role_relationship t2
                //                                        where u.user_id = t.user_id and t.dep_id = d.dep_id and u.user_id = t2.user_id and t2.role_id = r.role_id
                //                                        and u.delete_mark = 0 and d.dep_id = {0} and r.role_level = {1}
                //                                        for xml path('')),1,1,'')", dep, ConfCenter.ImportantUserRoleLevel);
                //    string userid = DataBaseHelper.ExecuteScalar(sql, out error).ToString();
                //    if(userid != "" && error == "")
                //    {
                //        where += string.Format(" and user_id1 in ({0})", userid);
                //    }
                //}
                if (year != 0 && month != 0)
                {
                    int nextMonth = 0;
                    int nextYear = 0;
                    if(month == 12)
                    {
                        nextMonth = 1;
                        nextYear = year + 1;
                    }
                    else
                    {
                        nextMonth = month + 1;
                        nextYear = year;
                    }

                    where += string.Format(" and ur.create_time >= '{0}-{1}-1 0:0:0' and ur.create_time < '{2}-{3}-1 0:0:0'", year, month, nextYear, nextMonth);
                }
                //if(leader != 0 && user == 0)
                //{
                //    List<int> ids = new List<int>();
                //    sql = string.Format("exec QuerySubUserByUserID {0}, {1}", leader, ConfCenter.ImportantUserRoleLevel);
                //    dt = DataBaseHelper.ExecuteTable(sql, out error);
                //    if(error == "")
                //    {
                //        foreach (DataRow dr in dt.Rows)
                //        {
                //            ids.Add(Convert.ToInt32(dr[0]));
                //        }
                //        if(ids.Count > 0)
                //        {
                //            where += string.Format(" and user_id1 in ({0})", string.Join(",", ids));
                //        }
                //        else
                //        {
                //            where += " and user_id1 = 0";
                //        }
                //    }
                //}
                if(!string.IsNullOrWhiteSpace(result))
                {
                    where += string.Format(" and urinalysis_result = '{0}'", result);
                }
                if(!string.IsNullOrWhiteSpace(state))
                {
                    where += string.Format(" and urinalysis_state = '{0}'", state);
                }

                if(where == "")
                {
                    throw new Exception("查询参数错误");
                }
                where = "ur.delete_mark = 0" + where;
                sql = string.Format(@"select ur.*,u1.user_name, u2.user_name from urinalysis_records ur 
                                                     left join sys_user u1 on ur.user_id1 = u1.user_id and u1.delete_mark = 0
                                                     left join sys_user u2 on ur.user_id2 = u2.user_id and u2.delete_mark = 0
                                                     where {0} order by ur.create_time desc", where);
                dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        UrinalysisRecord record = new UrinalysisRecord();
                        record.id = Convert.ToInt32(dr[0]);
                        record.user1 = Convert.ToInt32(dr[1]);
                        temp = dr[2].ToString();
                        if(int.TryParse(temp, out intTemp))
                        {
                            record.user2 = intTemp;
                        }
                        record.result = dr[3].ToString();
                        record.remark = dr[4].ToString();
                        record.photo = dr[5].ToString();
                        record.state = dr[6].ToString();
                        temp = dr[7].ToString();
                        if(temp != "")
                        {
                            record.createTime = Convert.ToDateTime(temp).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        temp = dr[8].ToString();
                        if (temp != "")
                        {
                            record.updateTime = Convert.ToDateTime(temp).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        record.name1 = dr[10].ToString();
                        record.name2 = dr[11].ToString();
                        records.Add(record);
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询尿检结果失败", "2904", ex.Message, ex.StackTrace);
            }

            return records.ToArray();
        }
    }
}
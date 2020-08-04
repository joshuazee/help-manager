using CMHL.Entity;
using CMHL.Lawer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Base
{
    public partial class Rest : IRest
    {
        private bool AddUser(out string str_error, string name, string identity, string mobile, int age, string sex, string photo, int role, int dep, int admin)
        {
            try
            {
                string sql = "";
                str_error = "";
                string error = "";
                if (photo == "")
                {
                    photo = "/upload/default_via.png";
                }
                sql = string.Format("select count(*) from sys_user where (user_mobile = '{0}' or user_identity = '{1}') and delete_mark = 0", mobile, identity);
                int count = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                if(count > 0)
                {
                    throw new Exception("该用户已注册(已存在的手机号/身份证号)");
                }
                if(admin == 0)
                {
                    sql = string.Format(@"insert into sys_user(user_name, user_identity, user_mobile, user_password, user_age, user_sex, user_photo, create_time, delete_mark) 
                                                                    values('{0}', '{1}', '{2}', '123456',{3}, '{4}', '{5}', getdate(), 0)select @@IDENTITY", name, identity, mobile, age, sex, photo);
                }
                else
                {
                    sql = string.Format(@"insert into sys_user(user_name, user_identity, user_mobile, user_age, user_sex, user_photo, user_parent, create_time, delete_mark) 
                                                                    values('{0}', '{1}', '{2}', {3}, '{4}', '{5}', {6}, getdate(), 0)select @@IDENTITY", name, identity, mobile, age, sex, photo, admin);
                }
                int id = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                if(id > 0 && error == "")
                {
                    //写入操作日志
                    if(role != 0)
                    {
                        bool flag = UpdateUserRoleRelationship(out error, id, role.ToString());
                        if(flag && error == "")
                        {
                            flag = UpdateUserDepRelationship(out error, id, dep.ToString());
                            if(flag && error == "")
                            {
                                return true;
                            }
                            else {
                                throw new Exception(error);
                            }
                        }
                        else
                        {
                            throw new Exception(error);
                        }
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("添加用户失败", "1001", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool UpdateUser(out string str_error, int id, string name, string identity, string mobile, string password, int age, string sex, string photo, string pin, int parent, string authCode)
        {
            str_error = "";
            try
            {
                string where = "";
                string sql = "";
                string error = "";
                int count = 0;
                if (!string.IsNullOrWhiteSpace(pin))
                {
                    sql = string.Format("select user_pin, user_pin_update_time from sys_user where delete_mark = 0 and user_id = {0}", id);
                    DataTable dt0 = DataBaseHelper.ExecuteTable(sql, out error);
                    if(dt0.Rows[0][0].ToString() != pin)
                    {
                        string d = dt0.Rows[0][1].ToString();
                        if (string.IsNullOrEmpty(d))
                        {
                            where += string.Format(",user_pin = '{0}',user_pin_update_time = getdate()", pin);
                        }
                        else
                        {
                            DateTime date = DateTime.Parse(d);
                            if ((DateTime.Now - date).TotalDays <= 30)
                            {
                                throw new Exception("绑定手机30天内无法重复修改");
                            }
                            else
                            {
                                where += string.Format(",user_pin = '{0}',user_pin_update_time = getdate()", pin);
                            }
                        }
                    }
                }
                if(!string.IsNullOrWhiteSpace(name))
                {
                    where += string.Format(",user_name = '{0}'", name);
                }
                if(!string.IsNullOrWhiteSpace(password))
                {
                    string old = password.Split('#')[0];
                    string new1 = password.Split('#')[1];
                    sql = string.Format("select count(*) from sys_user where user_id = {0} and user_password = '{1}'", id, old);
                    count = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                    if(count == 1 && error == "")
                    {
                        where += string.Format(",user_password = '{0}'", new1);
                    }
                    else
                    {
                        if(error != "")
                        {
                            throw new Exception(error);
                        }
                        else
                        {
                            throw new Exception("原密码不正确");
                        }
                    }
                }
                if(!string.IsNullOrWhiteSpace(mobile))
                {
                    sql = string.Format("select count(*) from sys_user where delete_mark = 0 and user_mobile = '{0}' and user_id != {1}", mobile, id);
                    count = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                    if(count > 0)
                    {
                        throw new Exception("该手机号已被注册");
                    }
                    where += string.Format(",user_mobile = '{0}'", mobile);
                }
                if (!string.IsNullOrWhiteSpace(identity))
                {
                    sql = string.Format("select count(*) from sys_user where delete_mark = 0 and user_identity = '{0}' and user_id != {1}", identity, id);
                    count = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                    if (count > 0)
                    {
                        throw new Exception("该身份证号已被注册");
                    }
                    where += string.Format(",user_identity = '{0}'", identity);
                }
                if (age != 0)
                {
                    where += string.Format(",user_age = {0}", age);
                }
                if (!string.IsNullOrWhiteSpace(sex))
                {
                    where += string.Format(",user_sex = '{0}'", sex);
                }
                if (!string.IsNullOrWhiteSpace(photo))
                {
                    where += string.Format(",user_photo = '{0}'", photo);
                }
                if(parent != 0)
                {
                    where += string.Format(",user_parent = {0}", parent);
                }
                if(!string.IsNullOrWhiteSpace(authCode))
                {
                    where += string.Format(",user_auth_code = '{0}'", authCode);
                }
                if (string.IsNullOrWhiteSpace(where) || id == 0)
                {
                    throw new Exception("编辑参数提交错误");
                }
                where = where.Substring(1);
                sql = string.Format("update sys_user set {0} where delete_mark = 0 and user_id = {1}", where, id);
                
                count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if (count > 0 && error == "")
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
                SystemLog.WriteErrorLog("编辑用户失败", "1002", ex.Message, ex.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// 用户手机绑定
        /// </summary>
        /// <param name="str_error"></param>
        /// <param name="mobile"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        private bool UpdateUserPin(out string str_error, string mobile, string pin)
        {
            str_error = "";

            try
            {
                string where = "";
                string error = "";
                string sql = string.Format("select user_pin_update_time from sys_user where delete_mark = 0 and user_mobile = '{0}'", mobile);
                string d = DataBaseHelper.ExecuteScalar(sql, out error).ToString();
                if (string.IsNullOrEmpty(d))
                {
                    where += string.Format("user_pin = '{0}',user_pin_update_time = getdate()", pin);
                }
                else
                {
                    DateTime date = DateTime.Parse(d);
                    if ((DateTime.Now - date).TotalDays <= 30)
                    {
                        throw new Exception("绑定手机30天内无法重复修改");
                    }
                    else
                    {
                        where += string.Format("user_pin = '{0}',user_pin_update_time = getdate()", pin);
                    }
                }
                sql = string.Format("update sys_user set {0} where delete_mark = 0 and user_mobile = '{1}'", where, mobile);

                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if (error == "")
                {
                    if(count == 0)
                    {
                        throw new Exception("该用户不存在");
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("更新用户设备码失败", "1002", ex.Message, ex.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// 清除用户手机绑定
        /// </summary>
        /// <returns></returns>
        private bool UpdateUserPin2(out string str_error, int id)
        {
            str_error = "";
            try
            {
                string sql = string.Format("update sys_user set user_pin='', user_pin_update_time = null where user_id = {0}", id);
                string error = "";
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if (error == "")
                {
                    if (count == 0)
                    {
                        throw new Exception("该用户不存在");
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("用户手机解绑失败", "1002", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool UpdateUserRoleRelationship(out string str_error, int userId, string roleIds)
        {
            str_error = "";
            string sql = "";
            string error = "";
            int count = 0;
            DataTable dt;
            try
            {
                sql = "select role_id from sys_user_role_relationship where delete_mark = 0 and user_id = " + userId;
                dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error != "")
                {
                    throw new Exception(error);
                }
                List<int> dels = new List<int>();
                foreach (DataRow dr in dt.Rows)
                {
                    dels.Add(Convert.ToInt32(dr[0]));
                }
                List<int> news = new List<int>();
                string[] tmp = roleIds.Split(',');
                for (int i = 0; i < tmp.Length; i++)
                {
                    int id = Convert.ToInt32(tmp[i]);
                    int j;
                    for (j = 0; j < dels.Count; j++)
                    {
                        if (id == dels[j])
                        {
                            dels.Remove(j);
                            j--;
                            break;
                        }
                    }

                    if (j >= dels.Count)
                    {
                        news.Add(id);
                    }
                }

                for (int i = 0; i < dels.Count; i++)
                {
                    sql = string.Format("update sys_user_role_relationship set delete_mark = 1 where delete_mark = 0 and user_id = {0} and role_id = {1}", userId, dels[i]);
                    count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                    if (count == 0 || error != "")
                    {
                        throw new Exception("清除多余人员角色关系失败：" + error);
                    }
                }

                for (int i = 0; i < news.Count; i++)
                {
                    sql = string.Format("insert into sys_user_role_relationship(user_id, role_id, create_time, delete_mark) values({0}, {1}, getdate(), 0)", userId, news[i]);
                    count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                    if (count == 0 || error != "")
                    {
                        throw new Exception("新增人员角色关系失败：" + error);
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("用户关联角色失败", "1005", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool UpdateUserDepRelationship(out string str_error, int userId, string depIds)
        {
            str_error = "";
            string sql = "";
            string error = "";
            int count = 0;
            DataTable dt;
            try
            {
                sql = "select dep_id from sys_user_dep_relationship where delete_mark = 0 and user_id = " + userId;
                dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error != "")
                {
                    throw new Exception(error);
                }
                List<int> dels = new List<int>();
                foreach (DataRow dr in dt.Rows)
                {
                    dels.Add(Convert.ToInt32(dr[0]));
                }
                List<int> news = new List<int>();
                string[] tmp = depIds.Split(',');
                for (int i = 0; i < tmp.Length; i++)
                {
                    int id = Convert.ToInt32(tmp[i]);
                    int j;
                    for (j = 0; j < dels.Count; j++)
                    {
                        if(id == dels[j])
                        {
                            dels.Remove(j);
                            j--;
                            break;
                        }
                    }

                    if(j >= dels.Count)
                    {
                        news.Add(id);
                    }
                }

                for (int i = 0; i < dels.Count; i++)
                {
                    sql = string.Format("update sys_user_dep_relationship set delete_mark = 1 where delete_mark = 0 and user_id = {0} and dep_id = {1}", userId, dels[i]);
                    count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                    if( count == 0 || error != "")
                    {
                        throw new Exception("清除多余人员部门关系失败：" + error);
                    }
                }

                for (int i = 0; i < news.Count; i++)
                {
                    sql = string.Format("insert into sys_user_dep_relationship(user_id, dep_id, create_time, delete_mark) values({0}, {1}, getdate(), 0)", userId, news[i]);
                    count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                    if (count == 0 || error != "")
                    {
                        throw new Exception("新增人员部门关系失败：" + error);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("用户关联部门失败", "1006", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool UpdateUserDep(out string str_error, int id, int dep, int parent)
        {
            str_error = "";
            try
            {
                string sql = "", error = "";
                sql = string.Format("TranslateUser {0}, {1}, {2}", id, dep, parent);
                string result = DataBaseHelper.ExecuteScalar(sql, out error).ToString();
                if(result == "" && error == "")
                {
                    return true;
                }
                else
                {
                    if(error != "")
                    {
                        throw new Exception(error);
                    }
                    else
                    {
                        throw new Exception(result);
                    }
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("用户调动部门失败", "1010", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool DeleteUser(out string str_error, int id)
        {
            str_error = "";
            try
            {
                if (id == 0)
                {
                    throw new Exception("缺少用户ID参数");
                }
                string sql = string.Format("DeleteUser {0}", id);
                string error = "";
                string message = DataBaseHelper.ExecuteScalar(sql, out error).ToString();
                if (error == "")
                {
                    if(message == "")
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception(message);
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
                SystemLog.WriteErrorLog("删除用户失败", "1003", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private User[] QueryUser(out string str_error, int id = 0, string name = "", string code= "", string sex = "", int age = 0, string mobile = "", int dep = 0, int role = 0, string level = "")
        {
            SystemLog.WriteErrorLog2("测试日志", "0000", "2");
            str_error = "";
            List<User> users = new List<User>();
            string sql = "";
            string error = "";
            string where = "u.delete_mark = 0";
            try
            {
                if(id != 0)
                {
                    where += " and u.user_ id=" + id;
                }
                if(!string.IsNullOrWhiteSpace(name))
                {
                    where += string.Format(" and u.user_name like '%{0}%'", name);
                }
                if (!string.IsNullOrWhiteSpace(code))
                {
                    where += string.Format(" and user_code = '{0}'", code);
                }
                if (!string.IsNullOrWhiteSpace(sex))
                {
                    where += string.Format(" and user_sex = '{0}'", name);
                }
                if (age != 0)
                {
                    where += string.Format(" and user_age = '{0}'", age);
                }
                if (!string.IsNullOrWhiteSpace(mobile))
                {
                    where += string.Format(" and user_mobile = '{0}'", mobile);
                }
                if (dep != 0)
                {
                    where += string.Format(" and d.dep_id = {0}", dep);
                }
                if (role != 0)
                {
                    where += string.Format(" and r.role_id = {0}", role);
                }
                if (!string.IsNullOrEmpty(level))
                {
                    string[] temp = level.Split('#');
                    string t = "";
                    switch (temp[0])
                    {
                        case "lt":
                            t += "<";
                            break;
                        case "gt":
                            t += ">";
                            break;
                        case "eq":
                            t += "=";
                            break;
                        case "ltoet":
                            t += "<=";
                            break;
                        case "gtoet":
                            t += ">=";
                            break;
                    }
                    where += string.Format(" and role_level {0} {1}", t, temp[1]);
                }
                sql = string.Format(@"select u.user_id, u.user_name, u.user_identity, u.user_photo, u.user_age, u.user_sex, u.user_pin, u.user_mobile,
                                                     d.dep_id, d.dep_name, r.role_id, r.role_name, u2.user_id, u2.user_name, r.role_code, r.role_level
                                                     from sys_user u 
                                                     left join sys_user_role_relationship t1 on u.user_id = t1.user_id
                                                     left join sys_role r on t1.role_id = r.role_id and r.delete_mark = 0
                                                     left join sys_user_dep_relationship t2 on u.user_id = t2.user_id
                                                     left join sys_dep d on t2.dep_id = d.dep_id and d.delete_mark = 0
                                                     left join sys_user u2 on u2.user_id = u.user_parent
                                                     where {0} order by r.role_level desc, u.user_id", where);
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error != "")
                {
                    throw new Exception(error);
                }
                
                foreach(DataRow dr in dt.Rows)
                {
                    User info = new User();
                    info.id = Convert.ToInt32(dr[0]);
                    info.name = dr[1].ToString();
                    info.identity = dr[2].ToString();
                    info.photo = dr[3].ToString();
                    info.age = Convert.ToInt32(dr[4]);
                    info.sex = dr[5].ToString();
                    info.pin = dr[6].ToString();
                    info.mobile = dr[7].ToString();
                    
                    List<Department> deps = new List<Department>();
                    List<Role> roles = new List<Role>();
                    string tmp;
                    int numTmp;
                    tmp = dr[8].ToString();
                    if (int.TryParse(tmp, out numTmp))
                    {
                        Department dInfo = new Department();
                        dInfo.id = numTmp;
                        dInfo.name = dr[9].ToString();
                        deps.Add(dInfo);
                    }
                    tmp = dr[10].ToString();
                    if (int.TryParse(tmp, out numTmp))
                    {
                        Role rInfo = new Role();
                        rInfo.id = numTmp;
                        rInfo.code = dr[14].ToString();
                        rInfo.level = Convert.ToInt32(dr[15]);
                        rInfo.name = dr[11].ToString();
                        roles.Add(rInfo);
                    }
                    tmp = dr[12].ToString();
                    if(int.TryParse(tmp, out numTmp))
                    {
                        info.parent = numTmp;
                        info.parentName = dr[13].ToString();
                    }
                    info.roles = roles.ToArray();
                    if(info.roles.Length > 0)
                    {
                        info.roleName = info.roles[0].name;
                    }
                    info.deps = deps.ToArray();
                    if(info.deps.Length > 0)
                    {
                        info.depName = info.deps[0].name;
                    }
                    users.Add(info);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询用户信息失败", "1004", ex.Message, ex.StackTrace);
            }
            return users.ToArray();
        }

        private User[] QueryAdminUserByDep(out string str_error, int dep)
        {
            return QueryUser(out str_error, 0, "", "", "", 0, "", dep, 0, ConfCenter.AdministratorUserRoleLevel.ToString());
        }

        //private User[] QuerySubUserById(out string str_error, int userId)
        //{
        //    List<User> result = new List<User>();
            
        //    str_error = "";
        //    string sql = "";
        //    string error = "";
        //    string where = "";
        //    DataTable dt;
        //    try
        //    {
        //        int roleId = ConfCenter.ImportantUserRoleID;
        //        if (roleId == 0)
        //        {
        //            throw new Exception("请检查重点人员角色ID配置");
        //        }
        //        sql = string.Format("exec QuerySubUserByUserID {0}, {1}", userId, roleId);
        //        dt = DataBaseHelper.ExecuteTable(sql, out error);
        //        if(error == "")
        //        {
        //            foreach(DataRow dr in dt.Rows)
        //            {
        //                User info = new User();
        //                info.id = Convert.ToInt32(dr[0]);
        //                info.name = dr[1].ToString();
        //                info.mobile = dr[3].ToString();
        //                info.age = Convert.ToInt32(dr[6]);
        //                info.sex = dr[7].ToString();
        //                info.photo = dr[8].ToString();
        //                info.pin = dr[9].ToString();
        //                result.Add(info);
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception(error);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        str_error = ex.Message;
        //        SystemLog.WriteErrorLog("查询下属人员失败", "1004", ex.Message, ex.StackTrace);
        //    }

        //    return result.ToArray();
        //}

        private User[] QuerySubImportantUserById(out string str_error, int id)
        {
            List<User> users = new List<User>();
            str_error = "";
            string error = "";
            try
            {
                string sql = string.Format(@"select r.role_level from sys_user u 
                                                               left join sys_user_role_relationship t1 on t1.delete_mark = 0 and u.user_id = t1.user_id
                                                               left join sys_role r on t1.role_id = r.role_id and r.delete_mark = 0
                                                               where u.delete_mark = 0 and u.user_id = {0}", id);
                int level = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                if(level == ConfCenter.AdministratorUserRoleLevel)
                {
                    sql = "select * from sys_user where delete_mark = 0 and user_parent = " + id;
                }
                else
                {
                    sql = string.Format("exec QuerySubUserByUserID {0}, {1}", id, ConfCenter.ImportantUserRoleLevel);
                }
                
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        User info = new User();
                        info.id = Convert.ToInt32(dr[0]);
                        info.name = dr[1].ToString();
                        info.mobile = dr[3].ToString();
                        info.age = Convert.ToInt32(dr[6]);
                        info.sex = dr[7].ToString();
                        info.photo = dr[8].ToString();
                        info.pin = dr[9].ToString();
                        users.Add(info);
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询用户信息失败", "1004", ex.Message, ex.StackTrace);
            }
            return users.ToArray();
        }

        private User[] QueryContactsByDep(out string str_error, int dep, int role, int user)
        {
            List<User> users = new List<User>();
            str_error = "";
            DataTable dt;
            string error = "";
            try
            {
                string sql;
                if(role == 0)
                {
                    throw new Exception("请求参数错误");
                }
                else
                {
                    sql = string.Format(@"select role_level from sys_role where role_id = {0}", role);
                    int level = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                    if(error != "")
                    {
                        throw new Exception(error);
                    }
                    if(level == ConfCenter.ImportantUserRoleLevel)
                    {
                        sql = string.Format(@"select u.* from sys_user u
                                                             left join sys_user_role_relationship t on t.user_id = u.user_id and t.delete_mark = 0
                                                             left join sys_role r on r.role_id = t.role_id and r.delete_mark = 0
                                                             left join sys_user_dep_relationship t2 on t2.user_id = u.user_id and t.delete_mark = 0
                                                             left join sys_dep d on d.dep_id = t2.dep_id and d.delete_mark = 0
                                                             where u.delete_mark = 0 and d.dep_id = {0} and r.role_level >= {1}
                                                             union
                                                             select * from sys_user 
                                                             where delete_mark = 0 and user_id in (select user_parent from sys_user where delete_mark = 0 and user_id = {2})", dep, ConfCenter.LoginAdminRoleLevel, user);
                    }
                    else if(level == ConfCenter.AdministratorUserRoleLevel)
                    {
                        sql = string.Format(@"select u.* from sys_user u
                                                             left join sys_user_role_relationship t on t.user_id = u.user_id and t.delete_mark = 0
                                                             left join sys_role r on r.role_id = t.role_id and r.delete_mark = 0
                                                             left join sys_user_dep_relationship t2 on t2.user_id = u.user_id and t.delete_mark = 0
                                                             left join sys_dep d on d.dep_id = t2.dep_id and d.delete_mark = 0
                                                             where u.delete_mark = 0 and d.dep_id = {0} and r.role_level >= {1}
                                                             union
                                                             select * from sys_user where delete_mark = 0 and user_parent = {2}", dep, ConfCenter.LoginAdminRoleLevel, user);

                    }
                    else
                    {
                        sql = string.Format(@"select u.* from sys_user u
                                                             left join sys_user_role_relationship t on t.user_id = u.user_id and t.delete_mark = 0
                                                             left join sys_role r on r.role_id = t.role_id and r.delete_mark = 0
                                                             left join sys_user_dep_relationship t2 on t2.user_id = u.user_id and t.delete_mark = 0
                                                             left join sys_dep d on d.dep_id = t2.dep_id and d.delete_mark = 0
                                                             where u.delete_mark = 0 and d.dep_id = {0}", dep);
                    }
                    dt = DataBaseHelper.ExecuteTable(sql, out error);
                    if (error == "")
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            User info = new User();
                            info.id = Convert.ToInt32(dr[0]);
                            info.name = dr[1].ToString();
                            info.mobile = dr[3].ToString();
                            info.age = Convert.ToInt32(dr[6]);
                            info.sex = dr[7].ToString();
                            info.photo = dr[8].ToString();
                            info.pin = dr[9].ToString();
                            users.Add(info);
                        }
                    }
                    else
                    {
                        throw new Exception(error);
                    }
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询联系人列表失败", "1004", ex.Message, ex.StackTrace);
            }

            return users.ToArray();
        }
    }
}

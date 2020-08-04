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
        private bool AddRole(out string str_error, string name, int level)
        {
            str_error = "";
            try
            {
                string sql = string.Format("insert into sys_role(role_name, role_level, role_order, create_time, delete_mark) values('{0}', {1}, 0, getdate(), 0)", name, level);
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
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("添加角色失败", "1011", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool UpdateRole(int id, string name, int level)
        {
            try
            {
                string where = "";
                if(!string.IsNullOrWhiteSpace(name))
                {
                    where += ",role_name = '" + name + "'";
                }
                if(level != 0)
                {
                    where += ",role_level = " + level;
                }
                if(where == "" || id == 0)
                {
                    throw new Exception("修改角色参数传入不正确");
                }
                where = where.Substring(1);
                string sql = string.Format("update sys_role set {0} where delete_mark = 0 and role_id = {1}", where, id);
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
                SystemLog.WriteErrorLog("编辑角色失败", "1012", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool UpdateRoleDepRelationship(out string str_error, int role, string deps)
        {
            str_error = "";

            string sql = "", error = "";
            int count = 0;
            try
            {
                sql = string.Format(@"UpdateRoleDepRelationship {0}, '{1}'", role, deps);
                count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if(count > 0 && error == "")
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
                SystemLog.WriteErrorLog("角色关联部门失败", "1008", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool UpdateRoleMenuRelationship(out string str_error, int roleId, string menuIds)
        {
            str_error = "";
            string sql = "";
            string error = "";
            int count = 0;
            DataTable dt;
            try
            {
                sql = "select menu_id from sys_role_menu_relationship where delete_mark = 0 and role_id = " + roleId;
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
                string[] tmp = menuIds.Split(',');
                for (int i = 0; i < tmp.Length; i++)
                {
                    int id = Convert.ToInt32(tmp[i]);
                    int j;
                    for (j = 0; j < dels.Count; j++)
                    {
                        if (id == dels[j])
                        {
                            dels.RemoveAt(j);
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
                    sql = string.Format("delete from sys_role_menu_relationship where delete_mark = 0 and role_id = {0} and menu_id = {1}", roleId, dels[i]);
                    count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                    if (count == 0 || error != "")
                    {
                        throw new Exception("清除多余角色菜单关系失败：" + error);
                    }
                }

                for (int i = 0; i < news.Count; i++)
                {
                    sql = string.Format("insert into sys_role_menu_relationship(role_id, menu_id, create_time, delete_mark) values({0}, {1}, getdate(), 0)", roleId, news[i]);
                    count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                    if (count == 0 || error != "")
                    {
                        throw new Exception("新增角色菜单关系失败：" + error);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("角色关联菜单失败", "1005", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool DeleteRole(out string str_error, int id)
        {
            str_error = "";
            try
            {
                if (id == 0)
                {
                    throw new Exception("删除角色传入参数不正确");
                }
                string sql = string.Format("DeleteRole {0}", id);
                string error = "";
                string message = DataBaseHelper.ExecuteScalar(sql, out error).ToString();
                if(error == "")
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
                SystemLog.WriteErrorLog("删除角色失败", "1013", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private Role[] QueryRoles(out string str_error, string name, string level)
        {
            str_error = "";
            List<Role> roles = new List<Role>();
            try
            {
                string where = "r.delete_mark = 0";
                if (!string.IsNullOrWhiteSpace(name))
                {
                    where += string.Format(" and role_name like '%{0}%'", name);
                }
                if(!string.IsNullOrEmpty(level))
                {
                    string[] temp = level.Split('#');
                    string t = "";
                    switch(temp[0])
                    {
                        case "lt":
                            t += "<";
                            break;
                        case "gt":
                            t += ">";
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
                string sql = string.Format("select r.role_id, role_code, role_name, role_level, role_order from sys_role r where {0} order by role_order", where);
                string error = "";
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Role info = new Role();
                        info.id = Convert.ToInt32(dr[0]);
                        info.code = dr[1].ToString();
                        info.name = dr[2].ToString();
                        info.level = Convert.ToInt32(dr[3]);
                        info.order = Convert.ToInt32(dr[4]);
                        roles.Add(info);
                    }

                    //操作日志
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询角色信息失败", "1014", ex.Message, ex.StackTrace);
            }
            return roles.ToArray();
        }

        private Role[] QueryRole2(out string str_error, int dep, string level)
        {
            str_error = "";
            List<Role> roles = new List<Role>();
            try
            {
                string where = "t.role_id = r.role_id and r.delete_mark = 0";
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
                        case "ltoet":
                            t += "<=";
                            break;
                        case "gtoet":
                            t += ">=";
                            break;
                    }
                    where += string.Format(" and role_level {0} {1}", t, temp[1]);
                }
                if(dep != 0)
                {
                    where += string.Format(" and t.dep_id = {0}", dep);
                }
                string sql = string.Format("select distinct r.role_id, role_code, role_name, role_level, role_order from sys_role r, sys_role_dep_relationship t where {0} order by role_order", where);
                string error = "";
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Role info = new Role();
                        info.id = Convert.ToInt32(dr[0]);
                        info.code = dr[1].ToString();
                        info.name = dr[2].ToString();
                        info.level = Convert.ToInt32(dr[3]);
                        info.order = Convert.ToInt32(dr[4]);
                        roles.Add(info);
                    }

                    //操作日志
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询角色信息失败", "1014", ex.Message, ex.StackTrace);
            }
            return roles.ToArray();
        }
    }
}

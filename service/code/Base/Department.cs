using CMHL.Entity;
using CMHL.Lawer;
using System;
using System.Collections.Generic;
using System.Data;

namespace CMHL.Base
{
    public partial class Rest : IRest
    {
        private Department AddDepartment(out string str_error, string name, int order, int parent, int level, string alias, string code)
        {
            str_error = "";
            Department dep = new Department();
            try
            {
                string sql = string.Format("select count(*) from sys_dep where delete_mark = 0 and dep_name = '{0}' and dep_parent = {1}", name, parent);
                string error = "";
                int count = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                if(error == "")
                {
                    if(count > 0)
                    {
                        throw new Exception("不能在同一部门下增加相同名称的子级部门");
                    }
                    else
                    {
                        sql = string.Format("insert into sys_dep(dep_name, dep_order, dep_parent, dep_level, dep_alias, dep_code, create_time, delete_mark) values('{0}', {1}, {2}, {3}, '{4}', '{5}', getdate(), 0)select @@IDENTITY", name, order, parent, level, alias, code);
                        count = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                        if (count > 0 && error == "")
                        {
                            dep.id = count;
                            dep.name = name;
                            dep.parent = parent;
                            dep.order = order;
                            dep.level = level;
                            dep.alias = alias;
                            dep.code = code;
                            //写入操作日志
                        }
                        else
                        {
                            throw new Exception(error);
                        }
                    }
                }

                
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("添加部门失败", "1021", ex.Message, ex.StackTrace);
            }
            return dep;
        }

        private Department UpdateDepartment(out string str_error, int id, string name, string alias, string code)
        {
            string where = "";
            str_error = "";
            Department dep = new Department();
            try
            {
                if(!string.IsNullOrWhiteSpace(name))
                {
                    where += string.Format(",dep_name = '{0}'", name);
                }
                if(!string.IsNullOrWhiteSpace(alias))
                {
                    where += string.Format(",dep_alias = '{0}'", alias);
                }
                if (!string.IsNullOrWhiteSpace(code))
                {
                    where += string.Format(",dep_code = '{0}'", code);
                }
                if (where == "" || id == 0)
                {
                    throw new Exception("修改部门传入参数不正确");
                }
                where = where.Substring(1);
                string sql = string.Format("update sys_dep set {0} where delete_mark = 0 and dep_id = {1}", where, id);
                string error = "";
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if (count > 0 && error == "")
                {
                    dep.id = id;
                    dep.name = name;
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("修改部门失败", "1022", ex.Message, ex.StackTrace);
            }
            return dep;
        }

        private bool DeleteDepartment(out string str_error, int id)
        {
            str_error = "";
            try
            {
                if(id == 0)
                {
                    throw new Exception("删除部门传入参数不正确");
                }
                string sql = string.Format(@"DeleteDep {0}", id);
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
                SystemLog.WriteErrorLog("删除部门失败", "1023", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private Department[] QueryDepartments(out string str_error, int id = 0, string name = "", int parent = 0)
        {
            str_error = "";
            List<Department> deps = new List<Department>();
            List<int> parents = new List<int>();
            string where = "delete_mark = 0";
            try
            {
                if(id != 0)
                {
                    where += " and dep_id = " + id;
                }
                if(!string.IsNullOrWhiteSpace(name))
                {
                    where += string.Format(" and dep_name like '%{0}%'", name);
                }
                if (parent != 0)
                {
                    parents.Add(parent);
                }
                string sql = string.Format("select dep_id, dep_name, dep_order, dep_parent, dep_level, dep_alias, dep_code from sys_dep where {0} order by dep_level desc", where);
                string error = "";
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Department dep = new Department();
                        dep.id = Convert.ToInt32(dr[0]);
                        dep.name = dr[1].ToString();
                        dep.order = Convert.ToInt32(dr[2]);
                        dep.parent = Convert.ToInt32(dr[3]);
                        dep.level = Convert.ToInt32(dr[4]);
                        dep.alias = dr[5].ToString();
                        dep.code = dr[6].ToString();
                        if (parents.Count != 0)
                        {
                            if (parents.IndexOf(dep.parent) >= 0 || parents.IndexOf(dep.id) >= 0)
                            {
                                if (parents.IndexOf(dep.id) < 0)
                                {
                                    parents.Add(dep.id);
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        deps.Add(dep);
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
                SystemLog.WriteErrorLog("查询部门信息失败", "1024", ex.Message, ex.StackTrace);
            }
            return deps.ToArray();
        }

        private DepartmentVue[] SerializeDepTree(int parent, Department[] deps)
        {
            List<DepartmentVue> list = new List<DepartmentVue>();

            for (int i = 0; i < deps.Length; i++)
            {
                if(deps[i].parent == parent)
                {
                    DepartmentVue dep = new DepartmentVue(deps[i]);
                    ///属性还原
                    dep.children = SerializeDepTree(dep.id, deps);
                    list.Add(dep);
                }
            }

            return list.ToArray();
        }

        private Department[] QuerySubDepartment(out string str_error, int parent)
        {
            str_error = "";
            List<Department> deps = new List<Department>();
            try
            {
                string sql = "", error = "";
                sql = string.Format("QueryCommunities {0}", parent);
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                foreach (DataRow dr in dt.Rows)
                {
                    Department dep = new Department();
                    dep.id = Convert.ToInt32(dr[0]);
                    dep.name = dr[1].ToString();
                    dep.parent = Convert.ToInt32(dr[2]);
                    dep.level = Convert.ToInt32(dr[3]);
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询部门信息失败", "1024", ex.Message, ex.StackTrace);
            }

            return deps.ToArray();
        }

        private Department QueryRegionByDepID(out string str_error, int id)
        {
            Department dep = new Department();
            str_error = "";

            string sql = "exec QueryRegionByDepID " + id;
            string error = "";
            try
            {
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    if(dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        dep.id = Convert.ToInt32(dr[0]);
                        dep.code = dr[1].ToString();
                        dep.name = dr[2].ToString();
                        dep.parent = Convert.ToInt32(dr[3]);
                    }
                    else
                    {
                        throw new Exception("没有查询到相应行政区数据");
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
                SystemLog.WriteErrorLog("查询所属行政区划失败", "1024", ex.Message, ex.StackTrace);
            }
            return dep;
        }

        private Department[] QuerySubRegionByDepID(out string str_error, int id)
        {
            List<Department> deps = new List<Department>();
            str_error = "";

            string sql = "exec QuerySubRegionByDepID " + id;
            string error = "";
            try
            {
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    if(dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            Department dep = new Department();
                            dep.name = dr[0].ToString();
                            dep.code = dr[1].ToString();
                            dep.alias = dr[2].ToString();
                            deps.Add(dep);
                        }
                    }
                    else
                    {
                        throw new Exception("已无下属行政区数据");
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
                SystemLog.WriteErrorLog("查询下属行政区划失败", "1025", ex.Message, ex.StackTrace);
            }

            return deps.ToArray();
        }

        private Department[] QueryRoleDepRelationship(out string str_error, int role)
        {
            str_error = "";
            List<Department> deps = new List<Department>();
            try
            {
                string sql = string.Format(@"select d.dep_id, dep_name, dep_order, dep_parent, dep_level, role_id
                                                                from sys_dep d left join sys_role_dep_relationship t on t.dep_id = d.dep_id and t.role_id = {0}
                                                                where d.delete_mark = 0  order by dep_level desc", role);
                string error = "";
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Department dep = new Department();
                        dep.id = Convert.ToInt32(dr[0]);
                        dep.name = dr[1].ToString();
                        dep.order = Convert.ToInt32(dr[2]);
                        dep.parent = Convert.ToInt32(dr[3]);
                        dep.level = Convert.ToInt32(dr[4]);
                        dep.isChecked = !string.IsNullOrEmpty(dr[5].ToString());
                        deps.Add(dep);
                    }
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询角色部门关系失败", "1026", ex.Message, ex.StackTrace);
            }

            return deps.ToArray();
        }
    }
}

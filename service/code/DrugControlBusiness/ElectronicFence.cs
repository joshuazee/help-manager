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
        public bool AddFence(out string str_error, string name, string type, string extent)
        {
            str_error = "";
            string sql = string.Format("insert into electronic_fence  values('{0}', '{1}', '{2}', getdate(), 0)", name, type, extent);
            string error = "";

            try
            {
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if(error == "")
                {
                    if(count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("语句执行成功但添加失败");
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
                SystemLog.WriteErrorLog("围栏添加失败", "1501", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public bool UpdateFence(out string str_error, int id, string name, string extent)
        {
            str_error = "";
            try
            {
                string where = "";
                if(name != "")
                {
                    where += string.Format(" and fence_name = '{0}'", name);
                }
                if(extent != "")
                {
                    where += string.Format(" and fence_extent = '{1}'", extent);
                }
                string sql = string.Format("update electronic_fence set {1} where fence_id = {0}", id, where);
                string error = "";
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if(error == "")
                {
                    if(count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("语句执行成功但编辑失败");
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
                SystemLog.WriteErrorLog("围栏编辑失败", "1502", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public bool DeleteFence(out string str_error, int id)
        {
            str_error = "";
            try
            {
                string sql = "update electronic_fence set delete_mark = 1 where fence_id = " + id;
                string error = "";
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if(error == "")
                {
                    if (count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("语句执行成功但删除失败");
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
                SystemLog.WriteErrorLog("围栏删除失败", "1503", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public bool FenceRelateDep(out string str_error, int id, string deps)
        {
            str_error = "";
            string sql = "", error = "";
            try
            {
                sql = "delete from dep_fence_relationship where fence_id = " + id;
               
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if(error == "")
                {
                    string where = "";
                    string[] temp = deps.Split(',');
                    for (int i = 0; i < temp.Length; i++)
                    {
                        int dep = 0;
                        if(int.TryParse(temp[i], out dep))
                        {
                            if(dep != 0)
                            {
                                if(where != "")
                                {
                                    where += ",";
                                }
                                where += "("+ dep + "," + id + ",getdate())";
                            }
                        }
                    }
                    if(where != "")
                    {
                        sql = "insert into dep_fence_relationship values" + where;
                        count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                        if (error == "")
                        {
                            if (count > 0)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        throw new Exception(error);
                    }
                }
                else
                {
                    throw new Exception(error);
                }
                //string[] temp = deps.Split(',');
                //string userIds = "";
                //for (int i = 0; i < temp.Length; i++)
                //{
                //    int depId = Convert.ToInt32(temp[i]);
                //    sql = string.Format("select user_id from sys_user", depId, ConfCenter.ImportantUserRoleLevel);
                //    if(userIds != "")
                //    {
                //        userIds += ",";
                //    }
                //    userIds += DataBaseHelper.ExecuteScalar(sql, out error).ToString();
                //}
                //string values = "";
                //for (int i = 0; i < userIds.Length; i++)
                //{
                //    if(values != "")
                //    {
                //        values += ",";
                //    }
                //    values += "(" + id + ", " + userIds[i] + ")";
                //}
                //sql = string.Format("insert into  values {0}", values);
                //return true;
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("围栏关联用户失败", "1504", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public FenceRecord[] QueryFence(out string str_error)
        {
            str_error = "";
            List<FenceRecord> records = new List<FenceRecord>();
            string sql, error = "";
            try
            {
                sql = string.Format("select fence_id, fence_name, fence_type, fence_extent, create_time from electronic_fence where delete_mark = 0");
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        FenceRecord fr = new FenceRecord();
                        fr.id = Convert.ToInt32(dr[0]);
                        fr.name = dr[1].ToString();
                        fr.type = dr[2].ToString();
                        fr.extent = dr[3].ToString();
                        fr.createTime = dr[4].ToString();
                        records.Add(fr);
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
                SystemLog.WriteErrorLog("围栏查询失败", "1504", ex.Message, ex.StackTrace);
            }
            return records.ToArray();
        }

        public ZTreeNode[] QueryRelatedDep(out string str_error, int id)
        {
            List<ZTreeNode> list = new List<ZTreeNode>();
            str_error = "";

            try
            {
                string sql = string.Format(@"select d.dep_id, d.dep_name, d.dep_parent, e.fence_id from 
                                        sys_dep d left join dep_fence_relationship t on d.dep_id = t.dep_id
                                        left join electronic_fence e on t.fence_id = e.fence_id and e.fence_id = {0} where d.delete_mark = 0", id);
                string error = "";
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ZTreeNode node = new ZTreeNode();
                        node.id = Convert.ToInt32(dr[0]);
                        node.name = dr[1].ToString();
                        node.parent = Convert.ToInt32(dr[2]);
                        node.isChecked = dr[3].ToString() == "" ? false : true;
                        list.Add(node);
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
                SystemLog.WriteErrorLog("围栏关联部分信息查询失败", "1505", ex.Message, ex.StackTrace);
            }
            return list.ToArray();
        }
    }
}
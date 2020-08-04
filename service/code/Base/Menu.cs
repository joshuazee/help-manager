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
        private bool AddMenu(string name, string config, int order)
        {
            try
            {
                string sql = string.Format("insert into sys_menu values()");
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
                SystemLog.WriteErrorLog("添加菜单失败", "1001", ex.Message, ex.StackTrace);
            }
            return false;
        }
        private bool UpdateMenu()
        {
            try
            {
                string sql = string.Format("update sys_menu set {0} where menu_id = {1}");
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
                SystemLog.WriteErrorLog("更新菜单信息失败", "1001", ex.Message, ex.StackTrace);
            }
            return false;
        }
        private bool DeleteMenu()
        {
            try
            {
                string sql = string.Format("delete from sys_menu where menu_id = {0}");
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
                SystemLog.WriteErrorLog("删除菜单失败", "1001", ex.Message, ex.StackTrace);
            }
            return false;
        }
        private Menu[] QueryMenus(out string str_error)
        {
            List<Menu> menus = new List<Menu>();
            str_error = "";
            try
            {
                string sql = string.Format(@"select * from sys_menu order by menu_parent, menu_order");
                string error = "";
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        int parent = Convert.ToInt32(dr[6]);
                        if(parent == 0)
                        {
                            Menu menu = new Menu();
                            menu.id = Convert.ToInt32(dr[0]);
                            menu.name = dr[1].ToString();
                            menu.code = dr[2].ToString();
                            menu.url = dr[3].ToString();
                            menu.icon = dr[4].ToString();
                            menu.type = dr[5].ToString();
                            menu.parent = parent;
                            menu.system = dr[7].ToString();
                            menu.order = Convert.ToInt32(dr[8]);
                            menu.config = dr[9].ToString();
                            menu.path = dr[10].ToString();
                            menu.title = dr[11].ToString();
                            if (menu.type == "root")
                            {
                                menu.children = GetChildrenMenu(menu.id, dt.Rows);
                            }
                            menus.Add(menu);
                        }
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
                SystemLog.WriteErrorLog("查询菜单失败", "1001", ex.Message, ex.StackTrace);
            }
            return menus.ToArray();
        }

        private string[] QueryRoleMenuRelationship(out string str_error, string roleIds)
        {
            str_error = "";
            List<string> result = new List<string>();
            try
            {
                string sql = string.Format(@"select menu_code 
                                                               from sys_menu m, sys_role_menu_relationship t 
                                                               where m.menu_type = 'menu' and m.menu_id = t.menu_id and t.role_id in ({0})", roleIds);
                string error = "";
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result.Add(dr[0].ToString());
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
                SystemLog.WriteErrorLog("获取角色对应菜单失败", "1001", ex.Message, ex.StackTrace);
            }

            return result.ToArray();
        }
    }
}
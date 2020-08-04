using CMHL.Entity;
using CMHL.Lawer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DrugControlBusiness
{
    public partial class Rest : IRest
    {
        public bool AddNews(out string str_error, string title, string content, int user, string type, string photo, int order, int top, int dep)
        {
            str_error = "";
            string sql;
            try
            {
                string temp = "";
                for (int i = 0; i < content.Length && temp.Length < 50; i++)
                {
                    if (content[i] == '<')
                    {
                        i = content.IndexOf('>', i);
                    }
                    else
                    {
                        temp += content[i];
                    }
                }
                
                sql = string.Format(@"insert into 
                                                     activity_news(news_title, news_content, news_user, news_dep, news_type, news_photo, news_order, news_top, create_time, delete_mark, news_html)
                                                     values('{0}', '{1}', {2}, {7},'{3}', '{4}', {5}, {6}, getdate(), 0, '{8}')", title, temp, user, type, photo, order, top, dep, content);

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
                SystemLog.WriteErrorLog("新增新闻信息失败", "1104", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public bool UpdateNews(out string str_error, int id, string title, string content, int user, string type, string photo, int order, int top)
        {
            str_error = "";
            string sql;
            string where = "";
            try
            {
                if(!string.IsNullOrWhiteSpace(title))
                {
                    where += string.Format(", news_title = '{0}'", title);
                }
                if (!string.IsNullOrWhiteSpace(content))
                {
                    string temp = "";
                    for (int i = 0; i < content.Length && temp.Length < 50; i++)
                    {
                        if (content[i] == '<')
                        {
                            i = content.IndexOf('>', i);
                        }
                        else
                        {
                            temp += content[i];
                        }
                    }

                    where += string.Format(", news_html = '{0}', news_content = '{1}'", content, temp);
                }
                //if (user != 0)
                //{
                //    where += string.Format(", news_user = {0}", user);
                //}
                //if (!string.IsNullOrWhiteSpace(type))
                //{
                //    where += string.Format(", news_type = '{0}'", type);
                //}
                //if (!string.IsNullOrWhiteSpace(photo))
                //{
                //    where += string.Format(", news_photo = '{0}'", photo);
                //}
                //if (order != 0)
                //{
                //    where += string.Format(", news_order = {0}", order);
                //}
                where += string.Format(", news_top = {0}", top);
                if(where != "")
                {
                    where = where.Substring(1);
                    sql = string.Format("update activity_news set {0} where news_id = {1} and delete_mark = 0", where, id);
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
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("更新新闻信息失败", "1104", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public bool DeleteNews(out string str_error, int id)
        {
            str_error = "";
            string sql;
            try
            {
                sql = string.Format("update activity_news set delete_mark = 1, update_time = getdate() where news_id = {0}", id);
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
                SystemLog.WriteErrorLog("删除新闻信息失败", "1104", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public News[] QueryNews(out string str_error, string type, int user = 0, string title = "", int top = 0)
        {
            List<News> news = new List<News>();
            str_error = "";
            string where = "news.delete_mark = 0";
            string sql, error = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(type))
                {
                    where += string.Format(" and news_type = '{0}'", type);
                }
                if (user != 0)
                {
                    //where += " and news_user = " + user;
                    sql = string.Format(@"select d.dep_id, d.dep_parent
                                                         from sys_user u, sys_dep d, sys_user_dep_relationship t
                                                         where u.user_id = t.user_id and d.dep_id = t.dep_id and u.user_id = {0}", user);
                    DataTable dt0 = DataBaseHelper.ExecuteTable(sql, out error);
                    if(error == "" && dt0.Rows.Count > 0)
                    {
                        int dep = Convert.ToInt32(dt0.Rows[0][0]);
                        int dParent = Convert.ToInt32(dt0.Rows[0][1]);
                        sql = string.Format(@"QueryNewsPermission {0}", dep);
                        List<int> deps = new List<int>();
                        DataTable dt1 = DataBaseHelper.ExecuteTable(sql, out error);
                        if(error == "" && dt1.Rows.Count > 0)
                        {
                            foreach(DataRow dr in dt1.Rows)
                            {
                                deps.Add(Convert.ToInt32(dr[0]));
                            }
                            where += string.Format(" and news_dep in ({0})", string.Join(",", deps));
                        }
                        else
                        {
                            throw new Exception("未知参数错误，请检查");
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(title))
                {
                    where += string.Format(" and news_title like '%{0}%'", title);
                }

                string order = "";
                if(top == 0)
                {
                    order = "create_time desc";
                }
                else
                {
                    order = "news_top desc, create_time desc";
                }

                sql = string.Format(@"select news_id, news_title, news_content, news_user, u.user_name, news_type, news_order, news.create_time, news_photo, news_top, news_html
                                                     from activity_news news
                                                     left join sys_user u on news.news_user = u.user_id and u.delete_mark = 0
                                                     where {0} order by {1}", where, order);

                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error != "")
                {
                    throw new Exception(error);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    News info = new News();
                    info.id = Convert.ToInt32(dr[0]);
                    info.title = dr[1].ToString();
                    info.user = dr[4].ToString();
                    info.content = dr[2].ToString();
                    info.user_id = Convert.ToInt32(dr[3]);
                    info.publish_time = Convert.ToDateTime(dr[7]).ToString("yyyy-MM-dd HH:mm:ss");
                    info.photo = dr[8].ToString();
                    info.order = Convert.ToInt32(dr[6]);
                    info.type = dr[5].ToString();
                    info.top = Convert.ToInt32(dr[9]);
                    info.html = dr[10].ToString();
                    news.Add(info);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询新闻信息失败", "1104", ex.Message, ex.StackTrace);
            }

            return news.ToArray();
        }
    }
}

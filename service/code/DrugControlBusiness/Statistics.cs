using CMHL.Entity;
using CMHL.Lawer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrugControlBusiness
{
    public partial class Rest : IRest
    {
        private Dictionary<string, string> QueryIndexContent(out string str_error, int id, int dep)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            News[] news = QueryNews(out str_error, "新闻快报", id, "", 1);
            if (str_error == "" && news.Length > 0)
            {
                result.Add("news", JsonConvert.SerializeObject(news));
            }
            news = QueryNews(out str_error, "社区新鲜事", id, "", 1);
            if (str_error == "" && news.Length > 0)
            {
                result.Add("communities", JsonConvert.SerializeObject(news));
            }
            news = QueryNews(out str_error, "帮扶记录", id, "", 1);
            if (str_error == "" && news.Length > 0)
            {
                result.Add("helprecords", JsonConvert.SerializeObject(news));
            }
            VideoRecord[] videos = QueryVideo(out str_error);
            if (str_error == "" && videos.Length > 0)
            {
                result.Add("video", videos[0].url);
            }

            //result.Add("statInfo", IndexStatistics(out str_error, dep));
            //Dictionary<string, int> stat = new Dictionary<string, int> {
            //    { "A社区", 45 },
            //    { "B社区", 32 },
            //    { "C社区", 19},
            //    { "D社区", 22},
            //    { "E社区", 38},
            //    { "F社区", 44},
            //    { "G社区", 27},
            //    { "H社区", 39},
            //    { "I社区", 33},
            //    { "J社区", 35},
            //    { "K社区", 41},
            //    { "L社区", 40},
            //    { "M社区", 27},
            //    { "N社区", 30},
            //    { "O社区", 42},
            //    { "P社区", 29},
            //    { "Q社区", 33},
            //    { "R社区", 28},
            //    { "S社区", 19},
            //    { "T社区", 22},
            //    { "U社区", 41},
            //    { "V社区", 29},
            //    { "W社区", 35},
            //    { "X社区", 36},
            //    { "Y社区", 18},
            //    { "Z社区", 29}
            //};
            //result.Add("statInfo", JsonConvert.SerializeObject(stat));
            return result;
        }
        private string CountSignByDep(out string str_error, string regionCode, string startTime, string endTime)
        {
            string result = "";
            str_error = "";

            try
            {
                if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                {
                    startTime = DateTime.Now.Year + "-" + DateTime.Now.Month + "-1 0:0:0";
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                string sql = "", error = "";

                sql = string.Format("select count(*) from sys_dep a, sys_dep b where a.dep_parent = b.dep_id and b.dep_code = '{0}' and a.delete_mark = 0 and b.delete_mark = 0", regionCode);
                int childrenCount = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                if(error == "")
                {
                    if (childrenCount == 0)
                    {
                        result = CountSignCommunity(regionCode, startTime, endTime);
                    }
                    else if(childrenCount > 0)
                    {
                        result = CountSignParent(regionCode, startTime, endTime);
                    }
                    else
                    {
                        throw new Exception("部门数据有误，无法统计");
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
                SystemLog.WriteErrorLog("统计签到信息失败", "11001", ex.Message, ex.StackTrace);
            }

            return result;
        }

        private string CountSignParent(string regionCode, string startTime, string endTime)
        {
            string result = "";

            string sql = "", error = "";

            if (!string.IsNullOrWhiteSpace(regionCode))
            {
                sql = string.Format("exec StatParentDepSignInfo '{0}', '{1}', '{2}'", regionCode, startTime, endTime);
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    List<StaticticsForSign> list = new List<StaticticsForSign>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        StaticticsForSign info = new StaticticsForSign();
                        info.name = dr[0].ToString();
                        info.code = dr[1].ToString();
                        info.total = Convert.ToInt32(dr[2]);
                        info.signed = Convert.ToInt32(dr[3]);
                        info.unsign = Convert.ToInt32(dr[4]);
                        info.check = Convert.ToInt32(dr[5]);
                        info.uncheck = Convert.ToInt32(dr[6]);
                        list.Add(info);
                    }
                    result = JsonConvert.SerializeObject(list);
                }
                else
                {
                    throw new Exception(error);
                }
            }
            else
            {
                throw new Exception("参数错误");
            }

            return result;
        }

        private string CountSignCommunity(string regionCode, string startTime, string endTime)
        {
            string result = "";

            string sql = "", error = "";

            if (!string.IsNullOrWhiteSpace(regionCode))
            {
                sql = string.Format("exec StatLevel1DepSignInfo '{0}', '{1}', '{2}'", regionCode, startTime, endTime);
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    List<StaticticsForSign> list = new List<StaticticsForSign>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        StaticticsForSign info = new StaticticsForSign();
                        info.name = dr[0].ToString();
                        info.total = Convert.ToInt32(dr[1]);
                        info.signed = Convert.ToInt32(dr[2]);
                        info.unsign = Convert.ToInt32(dr[3]);
                        info.check = Convert.ToInt32(dr[4]);
                        info.uncheck = Convert.ToInt32(dr[5]);
                        list.Add(info);
                    }
                    result = JsonConvert.SerializeObject(list);
                }
                else
                {
                    throw new Exception(error);
                }
            }
            else
            {
                throw new Exception("参数错误");
            }

            return result;
        }

        private string CountUrinalysisByDep(out string str_error, string regionCode, string startTime, string endTime)
        {
            string result = "";
            str_error = "";
            try
            {
                if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                {
                    startTime = DateTime.Now.Year + "-" + DateTime.Now.Month + "-1 0:0:0";
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                string sql = "", error = "";

                sql = string.Format("select count(*) from sys_dep a, sys_dep b where a.dep_parent = b.dep_id and b.dep_code = '{0}' and a.delete_mark = 0 and b.delete_mark = 0", regionCode);
                int childrenCount = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                if (error == "")
                {
                    if (childrenCount == 0)
                    {
                        result = CountUrinalysisCommunity(regionCode, startTime, endTime);
                    }
                    else if (childrenCount > 0)
                    {
                        result = CountUrinalysisParent(regionCode, startTime, endTime);
                    }
                    else
                    {
                        throw new Exception("部门数据有误，无法统计");
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
                SystemLog.WriteErrorLog("统计尿检信息失败", "11002", ex.Message, ex.StackTrace);
            }
            return result;
        }
        private string CountUrinalysisParent(string regionCode, string startTime, string endTime)
        {
            string result = "";

            string sql = "", error = "";

            if(!string.IsNullOrWhiteSpace(regionCode))
            {
                sql = string.Format("exec CountParentDepUrinalysis '{0}', '{1}', '{2}'", regionCode, startTime, endTime);
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    List<StatisticForUrinalysis> list = new List<StatisticForUrinalysis>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        StatisticForUrinalysis info = new StatisticForUrinalysis();
                        info.name = dr[0].ToString();
                        info.code = dr[1].ToString();
                        info.total = Convert.ToInt32(dr[2]);
                        info.check = Convert.ToInt32(dr[3]);
                        info.uncheck = Convert.ToInt32(dr[4]);
                        list.Add(info);
                    }
                    result = JsonConvert.SerializeObject(list);
                }
                else
                {
                    throw new Exception(error);
                }
            }
            else
            {
                throw new Exception("参数错误");
            }

            return result;
        }

        private string CountUrinalysisCommunity(string regionCode, string startTime, string endTime)
        {
            string result = "";

            string sql = "", error = "";

            if (!string.IsNullOrWhiteSpace(regionCode))
            {
                sql = string.Format("exec CountLevel1DepUrinalysis '{0}', '{1}', '{2}'", regionCode, startTime, endTime);
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    List<StatisticForUrinalysis> list = new List<StatisticForUrinalysis>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        StatisticForUrinalysis info = new StatisticForUrinalysis();
                        info.name = dr[0].ToString();
                        info.total = Convert.ToInt32(dr[1]);
                        info.check = Convert.ToInt32(dr[2]);
                        info.uncheck = Convert.ToInt32(dr[3]);
                        list.Add(info);
                    }
                    result = JsonConvert.SerializeObject(list);
                }
                else
                {
                    throw new Exception(error);
                }
            }
            else
            {
                throw new Exception("参数错误 ");
            }

            return result;
        }

        private string QueryRegionMapData(string regionCode)
        {
            string result = "";

            if(!string.IsNullOrWhiteSpace(regionCode))
            {
                string filename = AppHome.MapJson + regionCode + ".json";
                if(File.Exists(filename))
                {
                    FileStream fs = File.Open(filename, FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    result = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                    fs.Close();
                    fs.Dispose();
                }
            }

            return result;
        }

        private string CountActiveUser(out string str_error, string regionCode, string startTime, string endTime)
        {
            string result = "";
            str_error = "";
            try
            {
                if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                {
                    startTime = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                string sql = "", error = "";

                sql = string.Format("exec CountActiveUser '{0}', '{1}', '{2}', {3}", regionCode, startTime, endTime, ConfCenter.ImportantUserRoleLevel);
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    List<StatisticForActiveUser> list = new List<StatisticForActiveUser>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        StatisticForActiveUser info = new StatisticForActiveUser();
                        info.name = dr[0].ToString();
                        info.code = dr[1].ToString();
                        info.communityCount = Convert.ToInt32(dr[2]);
                        info.totalUserCount = Convert.ToInt32(dr[3]);
                        info.activeUserCount = Convert.ToInt32(dr[4]);
                        list.Add(info);
                    }
                    result = JsonConvert.SerializeObject(list);
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("统计装机信息失败", "11003", ex.Message, ex.StackTrace);
            }
            
            return result;
        }
    }
}

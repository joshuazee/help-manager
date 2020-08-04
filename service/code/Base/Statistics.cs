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
        private StatisticResult[] StatRegionData(out string str_error, int id)
        {
            str_error = "";
            List<StatisticResult> results = new List<StatisticResult>();

            string sql = "exec StatSubRegionData " + id + "," + ConfCenter.ImportantUserRoleLevel + "," + ConfCenter.AdministratorUserRoleLevel + "," + ConfCenter.LoginAdminRoleLevel;
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
                            StatisticResult result = new StatisticResult();
                            string name = dr[0].ToString();
                            string alias = dr[1].ToString();
                            int dCount = Convert.ToInt32(dr[2]);
                            int uCount = Convert.ToInt32(dr[3]);
                            int uCountL1 = Convert.ToInt32(dr[4]);
                            int uCountL2 = Convert.ToInt32(dr[5]);
                            int uCountL3 = Convert.ToInt32(dr[6]);
                            result.name = alias == "" ? name : alias;
                            result.data.Add("社区数目", dCount);
                            result.data.Add("用户总数", uCount);
                            result.data.Add("戒毒人员", uCountL1);
                            result.data.Add("社工", uCountL2);
                            result.data.Add("民警", uCountL3);
                            if (uCountL1 + uCountL2 + uCountL3 < uCount)
                            {
                                result.data.Add("其他", uCount - uCountL1 - uCountL2 - uCountL3);
                            }
                            results.Add(result);
                        }
                    }
                    else
                    {
                        throw new Exception("没有查到该行政区的数据");
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
                SystemLog.WriteErrorLog("统计行政区基本信息失败", "1031", ex.Message, ex.StackTrace);
            }

            return results.ToArray();
        }
    }
}

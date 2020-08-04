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
   public  partial class Rest : IRest
    {
        private bool AddVideo(out string str_error, string title, string uploader, string url, int dep = 0)
        {
            str_error = "";
            try
            {
                string sql = string.Format("insert into video_records values('{0}', '{1}', {2}, '{3}', getdate(), 0)", title, uploader, dep, url);
                string error = "";
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if(error == "" && count > 0)
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
                SystemLog.WriteErrorLog("添加视频失败", "1041", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private bool DeleteVideo(out string str_error, int id)
        {
            str_error = "";
            try
            {
                string sql = string.Format("update video_records set delete_mark = 1 where video_id = {0}", id);
                string error = "";
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
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
                SystemLog.WriteErrorLog("删除视频失败", "1043", ex.Message, ex.StackTrace);
            }
            return false;
        }

        private VideoRecord[] QueryVideo(out string str_error)
        {
            List<VideoRecord> records = new List<VideoRecord>();
            str_error = "";
            try
            {
                string sql = string.Format("select * from video_records where delete_mark = 0 order by create_time desc");
                string error = "";
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        VideoRecord vr = new VideoRecord();
                        vr.id = Convert.ToInt32(dr[0]);
                        vr.title = dr[1].ToString();
                        vr.uploader = dr[2].ToString();
                        vr.dep = dr[3].ToString();
                        vr.url = dr[4].ToString();
                        vr.createTime = dr[5].ToString();
                        records.Add(vr);
                    }
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询视频失败", "1043", ex.Message, ex.StackTrace);
            }
            return records.ToArray();
        }
    }
}

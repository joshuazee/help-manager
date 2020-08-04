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
        public bool AddTracePoint(out string str_error, double x, double y, int id)
        {
            str_error = "";
            string sql = "";
            string error = "";
            try
            {
                string tableName = GetTraceTableName();
                if (x == 4.9E-324 || y == 4.9E-324 || x == 0 || y == 0)
                {
                    throw new Exception("手机定位失败");
                }
                sql = string.Format("insert into {3}(user_id, point_x, point_y, create_time, delete_mark) values({0}, {1}, {2}, getdate(), 0)", id, x, y, tableName);
                int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
                if(count > 0 && error == "")
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("上传轨迹点失败", "1104", ex.Message, ex.StackTrace);
            }
            return false;
        }

        public Trace QueryTrace(out string str_error, int user, string time)
        {
            Trace t = new Trace();
            str_error = "";
            string sql = "";
            string error = "";
            string where = "delete_mark = 0";
            DataTable dt;
            try
            {
                if(user != 0)
                {
                    t.userID = user;
                    where += " and user_id = " + user;
                }
                if(!string.IsNullOrWhiteSpace(time))
                {
                    if(time.IndexOf(' ') >= 0)
                    {
                        time = time.Split(' ')[0];
                    }
                    where += " and create_time >= '" + time + " 0:0:0' and create_time <= '" + time + " 23:59:59'";
                    t.date = time;
                }
                string tableName = GetTraceTableName(time);
                sql = string.Format("select point_x, point_y, create_time from {1} where {0} order by create_time", where, tableName);

                dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    double lastX = 0, lastY = 0;
                    List<Point> points = new List<Point>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        double x = Convert.ToDouble(dr[0]);
                        double y = Convert.ToDouble(dr[1]);
                        if((lastX == 0 && lastY == 0) || Distance(lastX, lastY, x, y) < 1000)
                        {
                            Point p = new Point();
                            p.x = x;
                            p.y = y;
                            p.time = Convert.ToDateTime(dr[2]).ToString("yyyy-MM-dd HH:mm:ss");
                            points.Add(p);
                            lastX = x;
                            lastY = y;
                            if(points.Count > 10)
                            {
                                TraceFilter(ref points);
                            }
                        }
                    }
                    t.points = points.ToArray();
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询轨迹失败", "1104", ex.Message, ex.StackTrace);
            }

            return t;
        }

        public static double HaverSin(double theta)
        {
            var v = Math.Sin(theta / 2);
            return v * v;
        }


        static double EARTH_RADIUS = 6371.0;//km 地球半径 平均值，千米

        /// <summary>
        /// 给定的经度1，纬度1；经度2，纬度2. 计算2个经纬度之间的距离。
        /// </summary>
        /// <param name="lat1">经度1</param>
        /// <param name="lon1">纬度1</param>
        /// <param name="lat2">经度2</param>
        /// <param name="lon2">纬度2</param>
        /// <returns>距离（公里、千米）</returns>
        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            //用haversine公式计算球面两点间的距离。
            //经纬度转换成弧度
            lat1 = ConvertDegreesToRadians(lat1);
            lon1 = ConvertDegreesToRadians(lon1);
            lat2 = ConvertDegreesToRadians(lat2);
            lon2 = ConvertDegreesToRadians(lon2);

            //差值
            var vLon = Math.Abs(lon1 - lon2);
            var vLat = Math.Abs(lat1 - lat2);

            //h is the great circle distance in radians, great circle就是一个球体上的切面，它的圆心即是球心的一个周长最大的圆。
            var h = HaverSin(vLat) + Math.Cos(lat1) * Math.Cos(lat2) * HaverSin(vLon);

            var distance = 2 * EARTH_RADIUS * Math.Asin(Math.Sqrt(h));

            return distance;
        }

        /// <summary>
        /// 将角度换算为弧度。
        /// </summary>
        /// <param name="degrees">角度</param>
        /// <returns>弧度</returns>
        public static double ConvertDegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static double ConvertRadiansToDegrees(double radian)
        {
            return radian * 180.0 / Math.PI;
        }

        private void TraceFilter(ref List<Point> opoints)
        {
            Point[] points;
            Point midPoint = new Point();
            if (opoints.Count >= 10)
            {
                 points = new Point[10];
            }
            else
            {
                points = new Point[opoints.Count];
            }
            for (int i = opoints.Count - 1, count = 0; i >= 0 && count < 10; i--, count++)
            {
                points[count] = opoints[i];
                midPoint.x += opoints[i].x;
                midPoint.y += opoints[i].y;
                if(count == 0)
                {
                    midPoint.time = opoints[i].time;
                }
            }
            midPoint.x = midPoint.x / points.Length;
            midPoint.y = midPoint.y / points.Length;

            int j;
            for (j = 0; j < points.Length; j++)
            {
                if(Distance(midPoint.x, midPoint.y, points[j].x, points[j].y) > 50)
                {
                    break;
                }
            }

            if(j >= points.Length)
            {
                opoints.RemoveRange(opoints.Count - points.Length, points.Length);
                opoints.Add(midPoint);
            }
        }

        private string GetTraceTableName(string time = "")
        {
            string tableName = "";
            tableName = time == "" ? "user_trace_point_" + DateTime.Now.ToString("yyyyMM") : "user_trace_point_" + DateTime.Parse(time).ToString("yyyyMM");

            #region 建表
            //try
            //{
            //    string sql = "select COUNT(*) from sysobjects where name = '" + tableName + "' and type = 'U'";
            //    string error = "";
            //    int count = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
            //    if(error != "" || count < 1)
            //    {
            //        sql = "CREATE TABLE [dbo].[" + tableName +@"](
            //                     [point_id] [int] IDENTITY(1,1) NOT NULL,
            //                     [user_id] [int] NULL,
            //                     [point_x] [float] NULL,
            //                     [point_y] [float] NULL,
            //                     [create_time] [datetime] NULL,
            //                     [delete_mark] [int] NULL,
            //                        CONSTRAINT [PK_" + tableName + @"] PRIMARY KEY CLUSTERED 
            //                    (
            //                     [point_id] ASC
            //                    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
            //                    ) ON [PRIMARY]";
            //        count = DataBaseHelper.ExecuteNonQuery(sql, out error);
            //    }
            //}
            //catch(Exception ex)
            //{
            //    SystemLog.WriteErrorLog("获取轨迹点表名失败", "8888", ex.Message, ex.StackTrace);
            //}
            #endregion 

            return tableName;
        }
    }
}

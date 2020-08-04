using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMHL.TimerTask
{
    public partial class TimerTaskService : ServiceBase
    {
        public TimerTaskService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 月初执行的事件计时器
        /// </summary>
        private Timer timer1;
        /// <summary>
        /// 轨迹判断
        /// </summary>
        private Timer timer2;

        private static string ip = "127.0.0.1";
        //荆州端口
        private static int port1 = 8002;
        //远安端口
        private static int port2 = 8084;
        //新版
        private static int port3 = 8088;
        private static string basepath = "/service/rest/Base.svc/action/";
        private static string businesspath = "/service/rest/DrugControlBusiness.svc/action/";

        /// <summary>
        /// 定位判定时间间隔
        /// </summary>
        private int locationRefreshTimeSpan = 2 * 60 * 1000;

        protected override void OnStart(string[] args)
        {
            timer1 = new Timer(new TimerCallback(timer1Callback), null, 20000, 24 * 60 * 60 * 1000);

            timer2 = new Timer(new TimerCallback(timer2Callback), null, 0, locationRefreshTimeSpan);
        }

        protected override void OnStop()
        {
        }

        private void timer2Callback(object obj)
        {

        }

        /// <summary>
        /// 月初执行表创建
        /// </summary>
        /// <param name="obj"></param>
        private void timer1Callback(object obj)
        {
            if (DateTime.Now.Day == 1)
            {
                Dictionary<string, string> p = new Dictionary<string, string>();
                p.Add("t", Guid.NewGuid().ToString());
                string temp = HttpServices.SendRequest("http://" + ip + ":" + port1 + basepath + "create_partial_table", p);
                temp = HttpServices.SendRequest("http://" + ip + ":" + port2 + basepath + "create_partial_table", p);
                temp = HttpServices.SendRequest("http://" + ip + ":" + port3 + basepath + "create_partial_table", p);
            }
            timer1.Change(GetCurrentDaySpan(), 24 * 60 * 60 * 1000);
        }

        private static int GetCurrentMonthSpan()
        {
            DateTime d1 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01 00:00:00"));
            DateTime d2 = DateTime.Parse(DateTime.Now.AddMonths(1).ToString("yyyy-MM-01 00:00:00"));
            return (int)Math.Ceiling((d2 - d1).TotalMilliseconds);
        }

        private static int GetNextMonthSpan()
        {
            DateTime d1 = DateTime.Parse(DateTime.Now.AddMonths(1).ToString("yyyy-MM-01 00:00:00"));
            DateTime d2 = DateTime.Parse(DateTime.Now.AddMonths(2).ToString("yyyy-MM-01 00:00:00"));
            return (int)Math.Ceiling((d2 - d1).TotalMilliseconds);
        }

        private static int GetCurrentDaySpan()
        {
            DateTime d1 = DateTime.Now;
            DateTime d2 = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            return (int)Math.Ceiling((d2 - d1).TotalMilliseconds);
        }
    }
}

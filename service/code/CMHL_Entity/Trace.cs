using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class Trace
    {
        public Trace()
        {
            userID = 0;
            userName = "";
            points = new Point[0];
            date = "";
        }

        [DataMember(Name = "user_id")]
        public int userID;
        [DataMember(Name = "user_name")]
        public string userName;
        [DataMember(Name = "points")]
        public Point[] points;
        [DataMember(Name = "date")]
        public string date;
    }

    [DataContract]
    public class Point
    {
        [DataMember(Name = "x")]
        public double x;
        [DataMember(Name = "y")]
        public double y;
        [DataMember(Name = "time")]
        public string time;
    }
}

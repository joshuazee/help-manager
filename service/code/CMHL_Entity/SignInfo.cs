using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class SignInfo
    {
        public SignInfo()
        {
            id = 0;
            user1 = 0;
            user1_name = "";
            location1 = "";
            address1 = "";
            photo = "";
            type = "";
            state = "";
            remark = "";
            user2 = 0;
            user2_name = "";
            location2 = "";
            address2 = "";
            appointment = "";
            appointment_time = "";
            update_time = "";
            distance = 0.0;
        }

        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "user1")]
        public int user1;
        [DataMember(Name = "name1")]
        public string user1_name;
        [DataMember(Name = "location1")]
        public string location1;
        [DataMember(Name = "address1")]
        public string address1;
        [DataMember(Name = "time1")]
        public string time1;
        [DataMember(Name = "photo")]
        public string photo;
        [DataMember(Name = "type")]
        public string type;
        [DataMember(Name = "state")]
        public string state;
        [DataMember(Name = "remark")]
        public string remark;
        [DataMember(Name = "user2")]
        public int user2;
        [DataMember(Name = "name2")]
        public string user2_name;
        [DataMember(Name = "location2")]
        public string location2;
        [DataMember(Name = "address2")]
        public string address2;
        [DataMember(Name = "time2")]
        public string time2;
        [DataMember(Name = "appointment")]
        public string appointment;
        [DataMember(Name = "appointment_time")]
        public string appointment_time;
        [DataMember(Name = "distance")]
        public double distance;
        [DataMember(Name = "update_time")]
        public string update_time;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class UrinalysisRecord
    {
        public UrinalysisRecord()
        {
            id = 0;
            user1 = 0;
            user2 = 0;
            name1 = "";
            name2 = "";
            result = "";
            remark = "";
            photo = "";
            state = "";
            createTime = "";
            updateTime = "";
        }
        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "user1")]
        public int user1;
        [DataMember(Name = "user2")]
        public int user2;
        [DataMember(Name = "name1")]
        public string name1;
        [DataMember(Name = "name2")]
        public string name2;
        [DataMember(Name = "result")]
        public string result;
        [DataMember(Name = "remark")]
        public string remark;
        [DataMember(Name = "photo")]
        public string photo;
        [DataMember(Name = "state")]
        public string state;
        [DataMember(Name = "time")]
        public string createTime;
        [DataMember(Name = "update_time")]
        public string updateTime;
    }
}

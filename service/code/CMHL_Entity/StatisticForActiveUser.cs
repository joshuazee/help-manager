using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class StatisticForActiveUser
    {
        [DataMember(Name = "name")]
        public string name;
        [DataMember(Name = "code")]
        public string code;
        [DataMember(Name = "community_count")]
        public int communityCount;
        [DataMember(Name = "total_user_count")]
        public int totalUserCount;
        [DataMember(Name = "active_user_count")]
        public int activeUserCount;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class FenceRecord
    {
        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "name")]
        public string name;
        [DataMember(Name = "type")]
        public string type;
        [DataMember(Name = "extent")]
        public string extent;
        [DataMember(Name = "create_time")]
        public string createTime;
    }
}

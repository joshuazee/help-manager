using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class StatisticResult
    {
        public StatisticResult()
        {
            name = "";
            data = new Dictionary<string, object>();
        }

        [DataMember(Name = "name")]
        public string name;
        [DataMember(Name = "value")]
        public Dictionary<string, object> data;
    }
}

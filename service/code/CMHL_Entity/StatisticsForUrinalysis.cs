using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class StatisticForUrinalysis
    {
        [DataMember(Name = "name")]
        public string name;
        [DataMember(Name = "code")]
        public string code;
        [DataMember(Name = "total")]
        public int total;
        [DataMember(Name = "checked")]
        public int check;
        [DataMember(Name = "uncheck")]
        public int uncheck;
    }
}

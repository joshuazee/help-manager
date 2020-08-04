using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class StaticticsForSign
    {
        [DataMember(Name = "name")]
        public string name;
        [DataMember(Name = "code")]
        public string code;
        [DataMember(Name = "total")]
        public int total;
        [DataMember(Name = "signed")]
        public int signed;
        [DataMember(Name = "unsign")]
        public int unsign;
        [DataMember(Name = "checked")]
        public int check;
        [DataMember(Name = "uncheck")]
        public int uncheck;
    }
}

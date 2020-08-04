using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class Role
    {
        public Role()
        {
            id = 0;
            code = "";
            name = "";
            level = 0;
            order = 0;
        }

        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "code")]
        public string code;
        [DataMember(Name = "name")]
        public string name;
        [DataMember(Name = "level")]
        public int level;
        [DataMember(Name = "order")]
        public int order;
    }
}

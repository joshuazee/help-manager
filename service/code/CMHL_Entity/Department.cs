using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class Department
    {
        public Department()
        {
            id = 0;
            name = "";
            code = "";
            parent = 0;
            open = true;
            order = 0;
            isChecked = false;
            level = 0;
        }

        [DataMember(Name = "id", Order = 1)]
        public int id;
        [DataMember(Name = "name", Order = 2)]
        public string name;
        [DataMember(Name = "code", Order = 3)]
        public string code;
        [DataMember(Name = "pId", Order = 4)]
        public int parent;
        [DataMember(Name = "open", Order = 5)]
        public bool open;
        [DataMember(Name = "alias", Order = 6)]
        public string alias;
        [DataMember(Name = "checked", Order = 7)]
        public bool isChecked;
        [DataMember(Name = "level", Order = 8)]
        public int level;
        public int order;
    }
}

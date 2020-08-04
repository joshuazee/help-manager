using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class DepartmentVue
    {
        public DepartmentVue()
        {
            id = 0;
            name = "";
            code = "";
            children = null;
            open = true;
            isChecked = false;
        }

        public DepartmentVue(Department dep)
        {
            id = dep.id;
            name = string.IsNullOrEmpty(dep.alias) ? dep.name : dep.alias;
            code = dep.code;
            open = dep.open;
            isChecked = dep.isChecked;
        }

        [DataMember(Name = "id", Order = 1)]
        public int id;
        [DataMember(Name = "label", Order = 2)]
        public string name;
        [DataMember(Name = "code", Order = 3)]
        public string code;
        [DataMember(Name = "children", Order = 4)]
        public DepartmentVue[] children;
        [DataMember(Name = "expand", Order = 5)]
        public bool open;
        [DataMember(Name = "alias", Order = 6)]
        public string alias;
        [DataMember(Name = "checked", Order = 7)]
        public bool isChecked;
    }
}

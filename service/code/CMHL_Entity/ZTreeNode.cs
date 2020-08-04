using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CMHL.Entity
{
    [DataContract]
    public class ZTreeNode
    {
        public ZTreeNode()
        {
            id = 0;
            attributes = "";
            name = "";
            open = true;
            parent = 0;
        }
        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "attributes")]
        public string attributes;
        [DataMember(Name = "name")]
        public string name;
        [DataMember(Name = "open")]
        public bool open;
        [DataMember(Name = "pId")]
        public int parent;
        [DataMember(Name = "checked")]
        public bool isChecked;
    }
}
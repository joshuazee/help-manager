using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class Menu
    {
        public Menu()
        {
            id = 0;
            name = "";
            code = "";
            icon = "";
            config = "";
            children = null;
            type = "";
            parent = -1;
            system = "";
            order = 0;
            check = false;
            url = "";
            path = "";
            title = "";
        }

        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "name")]
        public string name;
        [DataMember(Name = "code")]
        public string code;
        [DataMember(Name = "img")]
        public string icon;
        [DataMember(Name = "config")]
        public string config;
        [DataMember(Name = "children")]
        public Menu[] children;
        [DataMember(Name = "type")]
        public string type;
        [DataMember(Name = "check")]
        public bool check;
        [DataMember(Name = "url")]
        public string url;
        [DataMember(Name = "path")]
        public string path;
        [DataMember(Name = "title")]
        public string title;
        public int parent;
        public string system;
        public int order;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class User
    {
        public User()
        {
            id = 0;
            name = "";
            mobile = "";
            photo = "";
            sex = "";
            age = 0;
            pin = "";
            parent = 0;
            parentName = "";
            deps = new Department[0];
            roles = new Role[0];
            password = "";
        }


        
        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "name")]
        public string name;
        [DataMember(Name = "mobile")]
        public string mobile;
        [DataMember(Name = "identity")]
        public string identity;
        [DataMember(Name = "photo")]
        public string photo;
        [DataMember(Name = "sex")]
        public string sex;
        [DataMember(Name = "age")]
        public int age;
        [DataMember(Name = "pin")]
        public string pin;
        [DataMember(Name = "parent")]
        public int parent;
        [DataMember(Name = "parent_name")]
        public string parentName;
        [DataMember(Name = "dep_name")]
        public string depName;
        [DataMember(Name = "deps")]
        public Department[] deps;
        [DataMember(Name = "role_name")]
        public string roleName;
        [DataMember(Name = "roles")]
        public Role[] roles;
        public string password;
    }
}

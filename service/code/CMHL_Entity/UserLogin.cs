using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class UserLogin
    {
        public UserLogin()
        {
            id = 0;
            name = "";
            mobile = "";
            sex = "";
            age = 0;
            token = "";
            photo = "";
            menus = new Menu[0];
            deps = new Department[0];
            roles = new Role[0];
        }

        [DataMember(Name = "userid")]
        public int id;
        [DataMember(Name = "username")]
        public string name;
        [DataMember(Name = "mobile")]
        public string mobile;
        [DataMember(Name = "sex")]
        public string sex;
        [DataMember(Name = "age")]
        public int age;
        [DataMember(Name = "token")]
        public string token;
        [DataMember(Name = "via")]
        public string photo;
        [DataMember(Name = "menus")]
        public Menu[] menus;
        [DataMember(Name = "deps")]
        public Department[] deps;
        [DataMember(Name = "roles")]
        public Role[] roles;
    }
}

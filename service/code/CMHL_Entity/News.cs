using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class News
    {
        public News()
        {
            id = 0;
            title = "";
            user = "";
            content = "";
            order = 0;
            user_id = 0;
            type = "";
            photo = "";
            top = 0;
        }

        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "title")]
        public string title;
        [DataMember(Name = "publish_user")]
        public string user;
        [DataMember(Name = "content")]
        public string content;
        [DataMember(Name = "html")]
        public string html;
        [DataMember(Name = "publish_time")]
        public string publish_time;
        [DataMember(Name = "photo")]
        public string photo;
        [DataMember(Name = "top")]
        public int top;
        public string type;
        public int order;
        public int user_id;
    }
}

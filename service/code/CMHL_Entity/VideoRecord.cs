using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Entity
{
    [DataContract]
    public class VideoRecord
    {
        [DataMember(Name = "id")]
        public int id;
        [DataMember(Name = "uploader")]
        public string uploader;
        [DataMember(Name = "title")]
        public string title;
        [DataMember(Name = "url")]
        public string url;
        [DataMember(Name = "time")]
        public string createTime;
        [DataMember(Name = "dep")]
        public string dep;
    }
}

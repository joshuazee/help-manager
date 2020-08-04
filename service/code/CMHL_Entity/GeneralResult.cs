using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CMHL.Lawer
{
    [DataContract]
    public class GeneralResult<T>
    {

        public GeneralResult()
        {
            success = false;
            message = "";
            data = default(T);

        }

        public GeneralResult(string msg)
        {
            success = false;
            message = msg;
            data = default(T);
        }
        [DataMember(Name = "success")]
        public bool success;
        [DataMember(Name = "message")]
        public string message;
        [DataMember(Name = "data")]
        public T data;
    }
}

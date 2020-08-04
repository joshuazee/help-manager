using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMHL.Lawer
{
    public class GeneralResults<T>
    {
        public GeneralResults()
        {
            isSuccess = false;
            errMsg = "";
            list = new List<T>();
            total = 0;
        }

        public GeneralResults(string errMsg)
        {
            isSuccess = false;
            this.errMsg = errMsg;
            list = new List<T>();
            total = 0;
        }
        public bool isSuccess;
        public string errMsg;
        public int total;
        public string exportPath;
        public List<T> list;
    }
}

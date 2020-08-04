using CMHL.Lawer;
using System;
using System.Collections.Generic;
using System.IO;

namespace DrugControlBusiness
{
    public partial class Rest : IRest
    {
        public string[] QueryHomepagePicture(out string str_error, string type)
        {
            List<string> result = new List<string>();
            str_error = "";
            try
            {
                string dir_path = AppHome.Download + "homepage";
                if(!string.IsNullOrEmpty(type))
                {
                    dir_path += "-" + type;
                }
                
                if(Directory.Exists(dir_path))
                {
                    string[] file_path = Directory.GetFileSystemEntries(dir_path);
                    for (int i = 0; i < file_path.Length; i++)
                    {
                        string t = file_path[i];
                        if(t.IndexOf(".jpg") >= 0 || t.IndexOf(".png") >= 0)
                        {
                            t = t.Substring(AppHome.BaseDirectory.Length - 1);
                            t = t.Replace("\\", "/");
                            result.Add(t);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询轮播图失败", "2914", ex.Message, ex.StackTrace);
            }

            return result.ToArray();
        }
    }
}

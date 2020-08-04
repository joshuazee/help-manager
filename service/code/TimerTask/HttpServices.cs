using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.TimerTask
{
    public static class HttpServices
    {
        public static string SendRequest(string uri, Dictionary<string, string> data = null)
        {
            string result = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "Post";
                request.ContentType = "application/x-www-form-urlencoded";
                if (data != null)
                {
                    Stream stream;
                    stream = request.GetRequestStream();
                    string temp = "";
                    foreach (string key in data.Keys)
                    {
                        string value = data[key];
                        if (temp != "")
                        {
                            temp += "&";
                        }
                        temp += key + "=" + value;
                    }
                    byte[] content = Encoding.UTF8.GetBytes(temp);
                    request.ContentLength = content.Length;
                    stream.Write(content, 0, content.Length);
                    stream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = reader.ReadToEnd();
                reader.Close();
            }
            catch(Exception ex)
            {
                
            }
            
            return result;
        }
    }
}

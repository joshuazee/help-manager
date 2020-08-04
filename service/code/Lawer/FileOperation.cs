using CMHL.Lawer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CMHL.Lawer
{
    public static class FileOperation
    {
        public static string upload(out string str_error, Stream file_stream)
        {
            str_error = "";
            try
            {
                string result = "";

                byte[] data = Misc.ToByteArray(file_stream);

                Encoding encoding = Encoding.UTF8;

                string content = encoding.GetString(data);

                string cloneContent = content;

                string delimiter = content.Substring(0, content.IndexOf("\r\n"));

                while (cloneContent.IndexOf("---------") != 0)
                {
                    cloneContent = cloneContent.Substring(delimiter.Length + 2);
                    int index = cloneContent.IndexOf("\r\n\r\n");
                    string head = cloneContent.Substring(0, index);

                    Match nameMatch = new Regex(@"(?<=name\=\"")(.*?)(?=\"")").Match(head);
                    string file_name = nameMatch.Value.Trim().ToLower();

                    int startIndex = Misc.IndexOf(data, encoding.GetBytes(head), 0) +
                                             encoding.GetBytes(head).Length +
                                             encoding.GetBytes("\r\n\r\n").Length;
                    byte[] delimiterBytes = encoding.GetBytes("\r\n" + delimiter);
                    int endIndex = Misc.IndexOf(data, delimiterBytes, startIndex);

                    int contentLength = endIndex - startIndex;

                    byte[] fileData = new byte[contentLength];

                    Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength);

                    string directory = AppHome.Upload + Guid.NewGuid() + "\\";

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    string file = directory + file_name;

                    using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(fileData, 0, contentLength);
                        fs.Flush();
                    }

                    result += file.Substring(AppHome.BaseDirectory.Length - 1) + "*";

                    cloneContent = cloneContent.Substring(cloneContent.IndexOf(delimiter) + 9);
                }

                if (result.Length > 0)
                {
                    result = result.Substring(0, result.Length - 1);
                    result = result.Replace("\\", "/");
                }

                return result;
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询用户信息失败", "1004", ex.Message, ex.StackTrace);
            }
            return "";
        }

        public static string SaveFile(out string str_error, string file_name, byte[] file_data)
        {
            str_error = "";
            string result = "";
            try
            {
                if (string.IsNullOrWhiteSpace(file_name))
                {
                    throw new Exception("请传入文件名");
                }
                
                if(file_data.Length <= 0)
                {
                    throw new Exception("请传入文件内容");
                }

                string directory = AppHome.Upload + DateTime.Now.ToString("yyyy_MM_dd") + "\\";

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string file_path = directory + file_name;

                using (FileStream fs = new FileStream(file_path, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(file_data, 0, file_data.Length);
                    fs.Flush();
                }
                result = file_path;
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("查询用户信息失败", "1004", ex.Message, ex.StackTrace);
            }
            return result;
        }

        public static string upload2(Stream stream)
        {
            List<string> paths = new List<string>();
            return string.Join("*", paths);
        }
    }
}

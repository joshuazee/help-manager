using CMHL.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace CMHL.Lawer
{
    public static class Utilities
    {
        public static Dictionary<string, string> GetQueryParam(Stream stream)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                StreamReader sr = new StreamReader(stream);
                string s = sr.ReadToEnd();
                sr.Dispose();
                NameValueCollection nvc = System.Web.HttpUtility.ParseQueryString(s);

                for (int i = 0; i < nvc.AllKeys.Length; i++)
                {
                    string key = nvc.AllKeys[i];
                    if (!string.IsNullOrWhiteSpace(nvc[key]))
                    {
                        result.Add(key, nvc[key]);
                    }
                }
            }
            catch (Exception ex)
            {
                SystemLog.WriteErrorLog("获取请求参数失败", "2001", ex.Message, ex.StackTrace);
            }

            return result;
        }

        public static bool SendMailRY(string mobile, int type, string templateId, string[] param = null)
        {
            string uri = "";
            if(type == 1)
            {
                uri = "http://api.sms.ronghub.com/sendCode.json";
            }
            else if(type == 2)
            {
                uri = "http://api.sms.ronghub.com/sendNotify.json";
            }
            int nonce = new Random().Next(9999);

            string timestamp = ((DateTime.Now.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000).ToString();

            string signature = SHA1_Encrypt(ConfCenter.MailAppSecret + nonce + timestamp);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "Post";
                request.Host = "api.sms.ronghub.com";
                request.Headers.Add("App-Key", ConfCenter.MailAppKey);
                request.Headers.Add("Nonce", nonce.ToString());
                request.Headers.Add("Timestamp", timestamp);
                request.Headers.Add("Signature", signature);
                request.ContentType = "application/x-www-form-urlencoded";
                Stream stream;
                stream = request.GetRequestStream();
                string p = "mobile=" + mobile + "&region=86&templateId=" + templateId;
                if(param != null)
                {
                    for (int i = 1; i <= param.Length; i++)
                    {
                        p += "&p" + i + "=" + param[i - 1];
                    }
                }
                byte[] content = Encoding.UTF8.GetBytes(p);
                stream.Write(content, 0, content.Length);
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string strRtnHtml = reader.ReadToEnd();
                reader.Close();
                RYResult result = JsonConvert.DeserializeObject<RYResult>(strRtnHtml);
                if (result.code == 200 && result.sessionId!= "")
                {
                    return true;
                }
                else
                {
                    throw new Exception(strRtnHtml);
                }
            }
            catch (Exception ex)
            {
                SystemLog.WriteErrorLog("短信发送失败", "1010", ex.Message, ex.StackTrace);
            }

            return false;
        }

        //public static bool FileUploadToCloudQiniu(string title, string filePath)
        //{
        //    try
        //    {
        //        if (File.Exists(filePath))
        //        {
        //            Mac mac = new Mac(ConfCenter.VideoAppKey, ConfCenter.VideoAppSecret);

        //            // 存储空间名
        //            string Bucket = ConfCenter.VideoBucket;

        //            PutPolicy putPolicy = new PutPolicy();
        //            putPolicy.Scope = Bucket;
        //            putPolicy.SetExpires(7200);
        //            string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());

        //            Config config = new Config();
        //            // 设置上传区域
        //            config.Zone = Zone.ZONE_CN_South;
        //            // 设置 http 或者 https 上传
        //            config.UseHttps = true;
        //            config.UseCdnDomains = true;
        //            config.ChunkSize = ChunkUnit.U512K;
        //            // 表单上传
        //            FormUploader target = new FormUploader(config);
        //            HttpResult result = target.UploadFile(filePath, title, token, null);
        //            //Console.WriteLine("form upload result: " + result.ToString());
        //        }
        //        else
        //        {
        //            throw new Exception("文件不存在");
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        SystemLog.WriteErrorLog("文件上传失败", "1011", ex.Message, ex.StackTrace);
        //    }
        //    return false;
        //}

        private static string SHA1_Encrypt(string Source_String)
        {
            byte[] StrRes = Encoding.UTF8.GetBytes(Source_String);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString().ToUpper();
        }
    }
}

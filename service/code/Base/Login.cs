using CMHL.Entity;
using CMHL.Lawer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CMHL.Base
{
    public partial class Rest: IRest
    {
        private int Login1(out string str_error,string mobile, string pin)
        {
            str_error = "";
            string sql = "";
            string error = "";
            int id = 0;
            try
            {
                sql = string.Format("ValidUser '{0}', '{1}', {2}", mobile, pin, ConfCenter.ImportantUserRoleLevel);
                DataTable dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    DataRow dr = dt.Rows[0];
                    error = dr[0].ToString();
                    id = Convert.ToInt32(dr[1]);
                    if (error == "" && id != 0)
                    {
                        SendMail(id, mobile);
                    }
                    else
                    {
                        throw new Exception(error);
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("登录失败_" + str_error + "_" + mobile + "_" + pin, "1009", ex.Message, ex.StackTrace);
            }
            return id;
        }

        public async void SendMail(int id, string mobile)
        {
            try
            {
                int nonce = new Random().Next(9999);

                string timestamp = ((DateTime.Now.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000).ToString();

                string signature = SHA1_Encrypt(ConfCenter.MailAppSecret + nonce + timestamp);

                string uri = "http://api.sms.ronghub.com/sendCode.json";

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("App-Key", ConfCenter.MailAppKey);
                client.DefaultRequestHeaders.Add("Nonce", nonce.ToString());
                client.DefaultRequestHeaders.Add("Timestamp", timestamp);
                client.DefaultRequestHeaders.Add("Signature", signature);
                //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"mobile", mobile},
                {"templateId", "0W3eX9zqABL9w7iFSM8yN_"},
                {"region", "86"}
            });
                var response = await client.PostAsync(uri, content);
                string responseString = await response.Content.ReadAsStringAsync();
                RYResult result = JsonConvert.DeserializeObject<RYResult>(responseString);
                if (result.code == 200)
                {
                    string error = "";
                    UpdateUser(out error, id, "", "", "", "", 0, "", "", "", 0, result.sessionId);
                    if (error != "")
                    {
                        throw new Exception(error + "_" + mobile);
                    }
                }
                else
                {
                    throw new Exception(responseString + "  __" + mobile);
                }
            }
            catch(Exception ex)
            {
                SystemLog.WriteErrorLog("发送短信失败", "1009", ex.Message, ex.StackTrace);
            }
        }

        public UserLogin Login2(out string str_error, string mobile, string code)
        {
            UserLogin result = new UserLogin();
            str_error = "";
            string sql = "";
            string error = "";
            string authCode = "";
            DataTable dt;
            try
            {
                sql = string.Format("select user_id, user_name, user_age, user_sex, user_photo, user_auth_code from sys_user where delete_mark = 0 and user_mobile = '{0}'", mobile);
               dt = DataBaseHelper.ExecuteTable(sql, out error);

                if (error == "")
                {
                    
                    DataRow dr = dt.Rows[0];
                    result.id = Convert.ToInt32(dr[0]);
                    result.name = dr[1].ToString();
                    result.mobile = mobile;
                    result.age = Convert.ToInt32(dr[2]);
                    result.sex = dr[3].ToString();
                    result.photo = dr[4].ToString();
                    authCode = dr[5].ToString();
                    //CheckUserPermission(ref result, out error);
                    //token = GetToken(result.id, result.name, result.photo);
                    //result.token = token;
                    if (CheckIdentity(code, authCode, mobile))
                    {
                        CheckUserPermission(ref result, out error);
                        if (error != "")
                        {
                            throw new Exception(error);
                        }
                        result.token = GetToken(result.id, result.name, result.photo);
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("登录失败_" + mobile, "1009", ex.Message, ex.StackTrace);
            }
            return result;
        }

        private bool CheckIdentity(string code, string sessionId, string mobile)
        {
            int nonce = new Random().Next(9999);

            string timestamp = ((DateTime.Now.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000).ToString();

            string signature = SHA1_Encrypt(ConfCenter.MailAppSecret + nonce + timestamp);

            string uri = "http://api.sms.ronghub.com/verifyCode.json";
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
                byte[] content = Encoding.UTF8.GetBytes("sessionId=" + sessionId + "&code=" + code);
                stream.Write(content, 0, content.Length);
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string strRtnHtml = reader.ReadToEnd();
                reader.Close();
                RYResult result = JsonConvert.DeserializeObject<RYResult>(strRtnHtml);
                if (result.code == 200 && result.success == true)
                {
                    return true;
                }
                else
                {
                    throw new Exception(strRtnHtml);
                }
            }
            catch(Exception ex)
            {
                SystemLog.WriteErrorLog("短信验证失败_" + mobile, "1009", ex.Message, ex.StackTrace);
                //throw new Exception(ex.Message);
            }
                
            return false;
        }

        public string GetToken(int id, string name, string photo)
        {
            string token = "";

            int nonce = new Random().Next(9999);

            string timestamp = ((DateTime.Now.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000).ToString();

            string signature = SHA1_Encrypt(ConfCenter.MailAppSecret + nonce + timestamp);

            string uri = "http://api-cn.ronghub.com/user/getToken.json";

            string str_content = "userId=" + id + " &name=" + name + "&portraitUri=" + photo;

            byte[] content = Encoding.UTF8.GetBytes(str_content);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "Post";
                request.Host = "api-cn.ronghub.com";
                request.Headers.Add("App-Key", ConfCenter.MailAppKey);
                request.Headers.Add("Nonce", nonce.ToString());
                request.Headers.Add("Timestamp", timestamp);
                request.Headers.Add("Signature", signature);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = content.Length;
                Stream stream;
                stream = request.GetRequestStream();
                stream.Write(content, 0, content.Length);
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string strRtnHtml = reader.ReadToEnd();
                reader.Close();
                RYResult result = JsonConvert.DeserializeObject<RYResult>(strRtnHtml);
                if (result.code == 200)
                {
                    token = result.token;
                }
            }
            catch (Exception ex)
            {
                SystemLog.WriteErrorLog("获取token失败", "1009", ex.Message, ex.StackTrace);
            }

            return token;
        }

        public UserLogin Login3(out string str_error, string mobile, string password)
        {
            UserLogin result = new UserLogin();
            str_error = "";
            string sql = "", error = "", temp = "";
            int intTemp = 0;
            DataTable dt;
            try
            {
                sql = string.Format(@"select u.user_id, user_name, user_mobile, user_age, user_sex, user_photo from sys_user u 
                                                     left join sys_user_role_relationship t1 on u.user_id = t1.user_id and t1.delete_mark = 0
                                                     left join sys_role r on t1.role_id = r.role_id and r.delete_mark = 0
                                                     where u.delete_mark = 0 and user_mobile = '{0}' and user_password = '{1}' and r.role_level >= {2}", mobile, password, ConfCenter.LoginAdminRoleLevel);

                dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    if(dt.Rows.Count == 1)
                    {
                        DataRow dr = dt.Rows[0];
                        result.id = Convert.ToInt32(dr[0]);
                        result.name = dr[1].ToString();
                        result.mobile = dr[2].ToString();
                        temp = dr[3].ToString();
                        if (int.TryParse(temp, out intTemp))
                        {
                            result.age = Convert.ToInt32(intTemp);
                        }
                        result.sex = dr[4].ToString();
                        result.photo = dr[5].ToString();

                        CheckUserPermission(ref result, out error);
                        if (error != "")
                        {
                            throw new Exception(error);
                        }
                    }
                    else
                    {
                        throw new Exception("用户/密码错误");
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch(Exception ex)
            {
                str_error = ex.Message;
                SystemLog.WriteErrorLog("登录失败_" + mobile, "1009", ex.Message, ex.StackTrace);
            }

            return result;
        }

        private void CheckUserPermission(ref UserLogin user, out string error)
        {
            error = "";
            user.roles = CheckUserRole(user.id, out error);
            if(error == "")
            {
                List<int> roleIds = new List<int>();
                for (int i = 0; i < user.roles.Length; i++)
                {
                    roleIds.Add(user.roles[i].id);
                }

                user.menus = CheckRoleMenu(roleIds.ToArray(), out error);

                if(error == "")
                {
                    user.deps = CheckUserDep(user.id, out error);
                }
            }
        }

        private Role[] CheckUserRole(int userId, out string str_error)
        {
            str_error = "";
            List<Role> roles = new List<Role>();
            string sql = "";
            string error = "";
            DataTable dt;
            try
            {
                sql = string.Format(@"select r.role_id, r.role_code, r.role_name, r.role_level, r.role_order
                                                     from sys_role r 
                                                     left join sys_user_role_relationship t on r.role_id = t.role_id and t.delete_mark = 0
                                                     left join  sys_user u on u.user_id = t.user_id and u.delete_mark = 0
                                                     where r.delete_mark = 0 and u.user_id = {0} order by role_order", userId);
                dt = DataBaseHelper.ExecuteTable(sql, out error);
                if(error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Role role = new Role();
                        role.id = Convert.ToInt32(dr[0]);
                        role.code = dr[1].ToString();
                        role.name = dr[2].ToString();
                        role.level = Convert.ToInt32(dr[3]);
                        role.order = Convert.ToInt32(dr[4]);
                        roles.Add(role);
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch(Exception ex)
            {
                str_error = "验证用户角色信息失败";
                SystemLog.WriteErrorLog("验证用户角色信息失败", "1008", ex.Message, ex.StackTrace);
            }
            
            return roles.ToArray();
        }

        private Department[] CheckUserDep(int userId, out string str_error)
        {
            str_error = "";
            List<Department> deps = new List<Department>();
            string sql = "";
            string error = "";
            DataTable dt;
            try
            {
                sql = string.Format(@"select d.dep_id, d.dep_name, d.dep_parent, d.dep_order, d.dep_level, d.dep_code
                                                     from sys_dep d
                                                     left join sys_user_dep_relationship t on t.dep_id = d.dep_id and t.delete_mark = 0
                                                     left join sys_user u on u.user_id = t.user_id and u.delete_mark = 0
                                                     where d.delete_mark = 0 and u.user_id = {0} order by dep_order", userId);
                dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Department dep = new Department();
                        dep.id = Convert.ToInt32(dr[0]);
                        dep.name = dr[1].ToString();
                        dep.parent = Convert.ToInt32(dr[2]);
                        dep.order = Convert.ToInt32(dr[3]);
                        dep.level = Convert.ToInt32(dr[4]);
                        dep.code = dr[5].ToString();
                        deps.Add(dep);
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                str_error = "验证用户部门信息失败";
                SystemLog.WriteErrorLog("验证用户部门信息失败", "1007", ex.Message, ex.StackTrace);
            }

            return deps.ToArray();
        }

        private Menu[] CheckRoleMenu(int[] roleIds, out string str_error)
        {
            str_error = "";
            List<Menu> menus = new List<Menu>();
            string sql = "";
            string error = "";
            DataTable dt;
            try
            {
                sql = string.Format(@"select distinct m.* from sys_menu m, sys_role_menu_relationship t, sys_role r
                                                     where m.menu_id = t.menu_id and m.delete_mark = 0 and t.role_id = r.role_id 
                                                     and t.delete_mark = 0 and r.delete_mark = 0 and r.role_id in({0}) and m.menu_system = 'web'
                                                     order by menu_parent,menu_order", string.Join(",", roleIds));
                dt = DataBaseHelper.ExecuteTable(sql, out error);
                if (error == "")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        int parent = Convert.ToInt32(dr[6]);
                        if(parent == 0)
                        {
                            Menu menu = new Menu();
                            menu.id = Convert.ToInt32(dr[0]);
                            menu.name = dr[1].ToString();
                            menu.code = dr[2].ToString();
                            menu.url = dr[3].ToString();
                            menu.icon = dr[4].ToString();
                            menu.type = dr[5].ToString();
                            menu.parent = parent;
                            menu.system = dr[7].ToString();
                            menu.order = Convert.ToInt32(dr[8]);
                            menu.config = dr[9].ToString();
                            menu.path = dr[10].ToString();
                            menu.title = dr[11].ToString();
                            if (menu.type == "root")
                            {
                                menu.children = GetChildrenMenu(menu.id, dt.Rows);
                            }
                            menus.Add(menu);
                        }
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                str_error = "验证用户菜单信息失败";
                SystemLog.WriteErrorLog("验证用户菜单信息失败", "1006", ex.Message, ex.StackTrace);
            }

            return menus.ToArray();
        }

        private Menu[] GetChildrenMenu(int id, DataRowCollection drc)
        {
            List<Menu> menus = new List<Menu>();
            foreach(DataRow dr in drc)
            {
                int parent = Convert.ToInt32(dr[6]);
                if (parent == id)
                {
                    Menu menu = new Menu();
                    menu.id = Convert.ToInt32(dr[0]);
                    menu.name = dr[1].ToString();
                    menu.code = dr[2].ToString();
                    menu.url = dr[3].ToString();
                    menu.icon = dr[4].ToString();
                    menu.type = dr[5].ToString();
                    menu.parent = parent;
                    menu.system = dr[7].ToString();
                    menu.order = Convert.ToInt32(dr[8]);
                    menu.config = dr[9].ToString();
                    menu.path = dr[10].ToString();
                    menu.title = dr[11].ToString();
                    if (menu.type == "root")
                    {
                        menu.children = GetChildrenMenu(menu.id, drc);
                    }
                    menus.Add(menu);
                }
            }
            return menus.ToArray();
        }

        private string SHA1_Encrypt(string Source_String)
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

        private int NoLogin(out string str_error, string mobile)
        {
            str_error = "";
            string sql = "", error = "";
            try
            {
                sql = string.Format("select user_id from sys_user where user_mobile = '{0}' and delete_mark = 0", mobile);
                int id = Convert.ToInt32(DataBaseHelper.ExecuteScalar(sql, out error));
                if(error == "")
                {
                    return id;
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch(Exception ex)
            {
                SystemLog.WriteErrorLog("获取用户id失败_" + mobile, "1010", ex.Message, ex.StackTrace);
            }
            return 0;
        }
    }
}

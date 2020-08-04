using CMHL.Entity;
using CMHL.Lawer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace CMHL.Base
{
    public partial class Rest : IRest
    {
        /// <summary>
        /// 分发器
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Message Action(string method, Stream stream)
        {
            try
            {
                int operation_user;
                string token;
                Dictionary<string, string> param = Utilities.GetQueryParam(stream);
                operation_user = param.ContainsKey("operation_user") ? Convert.ToInt32(param["operation_user"]) : 0;
                token = param.ContainsKey("token") ? param["token"] : "";
                string str_result = "";

                if (!Token.ValidToken(operation_user, token))
                {
                    GeneralResult<string> result = new GeneralResult<string>("token过期，请重新登录");
                    str_result = JsonConvert.SerializeObject(result);
                }
                else
                {
                    switch (method)
                    {
                        case "login1":
                            {
                                string mobile = param.ContainsKey("mobile") ? param["mobile"] : "";
                                string pin = param.ContainsKey("pin") ? param["pin"] : "";
                                string error = "";
                                int id = Login1(out error, mobile, pin);
                                GeneralResult<int> result = new GeneralResult<int>();
                                result.data = id;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "login2":
                            {
                                string mobile = param.ContainsKey("mobile") ? param["mobile"] : "";
                                string code = param.ContainsKey("code") ? param["code"] : "";
                                string error = "";
                                UserLogin ul = Login2(out error, mobile, code);
                                GeneralResult<UserLogin> result = new GeneralResult<UserLogin>();
                                result.data = ul;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "login3":
                            {
                                string mobile = param.ContainsKey("mobile") ? param["mobile"] : "";
                                string password = param.ContainsKey("password") ? param["password"] : "";
                                string error = "";
                                UserLogin ul = Login3(out error, mobile, password);
                                GeneralResult<UserLogin> result = new GeneralResult<UserLogin>();
                                result.data = ul;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "no_login":
                            {
                                string mobile = param.ContainsKey("mobile") ? param["mobile"] : "";
                                string error = "";
                                GeneralResult<int> result = new GeneralResult<int>();
                                result.data = NoLogin(out error, mobile);
                                if(error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "register":
                            {
                                Register();
                                break;
                            }
                        case "add_user":
                            {
                                string mobile = param.ContainsKey("mobile") ? param["mobile"] : "";
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                string identity = param.ContainsKey("identity") ? param["identity"] : "";
                                string sex = param.ContainsKey("sex") ? param["sex"] : "";
                                int age = param.ContainsKey("age") ? Convert.ToInt32(param["age"]) : 0;
                                string photo = param.ContainsKey("photo") ? param["photo"] : "";
                                int role = param.ContainsKey("role") ? Convert.ToInt32(param["role"]) : 0;
                                int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                                int admin = param.ContainsKey("admin") ? Convert.ToInt32(param["admin"]) : 0;
                                string error = "";
                                bool flag = AddUser(out error, name, identity, mobile, age, sex, photo, role, dep, admin);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if(error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "update_user":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string mobile = param.ContainsKey("mobile") ? param["mobile"] : "";
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                string identity = param.ContainsKey("identity") ? param["identity"] : "";
                                string sex = param.ContainsKey("sex") ? param["sex"] : "";
                                int age = param.ContainsKey("age") ? Convert.ToInt32(param["age"]) : 0;
                                string photo = param.ContainsKey("photo") ? param["photo"] : "";
                                int parent = param.ContainsKey("admin") ? Convert.ToInt32(param["admin"]) : 0;
                                string pin = param.ContainsKey("pin") ? param["pin"] : "";
                                string password = param.ContainsKey("password") ? param["password"] : "";
                                string authCode = param.ContainsKey("authCode") ? param["authCode"] : "";
                                string error = "";
                                bool flag = UpdateUser(out error, id, name, identity, mobile, password, age, sex, photo, pin, parent, authCode);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "update_user_pin":
                            {
                                string mobile = param.ContainsKey("mobile") ? param["mobile"] : "";
                                string pin = param.ContainsKey("pin") ? param["pin"] : "";
                                string error = "";
                                bool flag = UpdateUserPin(out error, mobile, pin);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "clear_user_pin":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string error = "";
                                bool flag = UpdateUserPin2(out error, id);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "update_user_dep_relationship":
                            {
                                int user = param.ContainsKey("user") ? Convert.ToInt32(param["user"]) : 0;
                                string dep = param.ContainsKey("dep") ? param["dep"].ToString() : "";
                                string error = "";
                                bool flag = UpdateUserDepRelationship(out error, user, dep);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "update_user_role_relationship":
                            {
                                int user = param.ContainsKey("user") ? Convert.ToInt32(param["user"]) : 0;
                                string role = param.ContainsKey("role") ? param["role"].ToString() : "";
                                string error = "";
                                bool flag = UpdateUserRoleRelationship(out error, user, role);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "update_user_dep":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                                int parent = param.ContainsKey("parent") ? Convert.ToInt32(param["parent"]) : 0;
                                string error = "";
                                bool flag = UpdateUserDep(out error, id, dep, parent);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "delete_user":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string error = "";
                                bool flag = DeleteUser(out error, id);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "query_user":
                            {
                                SystemLog.WriteErrorLog2("测试日志", "0000", "1");
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                string code = param.ContainsKey("code") ? param["code"] : "";
                                string sex = param.ContainsKey("sex") ? param["sex"] : "";
                                string mobile = param.ContainsKey("mobile") ? param["mobile"] : "";
                                int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                                int role = param.ContainsKey("role") ? Convert.ToInt32(param["role"]) : 0;
                                int age = param.ContainsKey("age") ? Convert.ToInt32(param["age"]) : 0;
                                string level = param.ContainsKey("level") ? param["level"] : "";
                                string error = "";
                                User[] users = QueryUser(out error, id, name, code, sex, age, mobile, dep, role, level);
                                GeneralResult<User[]> result = new GeneralResult<User[]>();
                                result.data = users;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "query_adminuser_by_dep":
                            {
                                int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                                string error = "";
                                User[] users = QueryAdminUserByDep(out error, dep);
                                GeneralResult<User[]> result = new GeneralResult<User[]>();
                                result.data = users;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "query_subuser_by_id":
                            {
                                int userId = param.ContainsKey("user_id") ? Convert.ToInt32(param["user_id"]) : 0;
                                string error = "";
                                User[] users = QuerySubImportantUserById(out error, userId);
                                GeneralResult<User[]> result = new GeneralResult<User[]>();
                                result.data = users;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "query_contacts_by_dep":
                            {
                                int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                                int role = param.ContainsKey("role") ? Convert.ToInt32(param["role"]) : 0;
                                int user = param.ContainsKey("user") ? Convert.ToInt32(param["user"]) : 0;
                                string error = "";
                                User[] users = QueryContactsByDep(out error, dep, role, user);
                                GeneralResult<User[]> result = new GeneralResult<User[]>();
                                result.data = users;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "add_role":
                            {
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                int level = param.ContainsKey("level") ? Convert.ToInt32(param["level"]) : 0;
                                string error = "";
                                bool flag = AddRole(out error, name, level);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "update_role":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                int level = param.ContainsKey("level") ? Convert.ToInt32(param["level"]) : 0;
                                string error = "";
                                bool flag = UpdateRole(id, name, level);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "update_role_menu_relationship":
                            {
                                int roleId = param.ContainsKey("roleId") ? Convert.ToInt32(param["roleId"]) : 0;
                                string menuId = param.ContainsKey("menuId") ? param["menuId"] : "";
                                string error = "";
                                bool flag = UpdateRoleMenuRelationship(out error, roleId, menuId);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "update_role_dep_relationship":
                            {
                                int role = param.ContainsKey("role") ? Convert.ToInt32(param["role"]) : 0;
                                string deps = param.ContainsKey("deps") ? param["deps"] : "";
                                string error = "";
                                bool flag = UpdateRoleDepRelationship(out error, role, deps);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "delete_role":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string error = "";
                                bool flag = DeleteRole(out error, id);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "query_role":
                            {
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                string level = param.ContainsKey("level") ? param["level"] : "";
                                string error = "";
                                Role[] roles = QueryRoles(out error, name, level);
                                GeneralResult<Role[]> result = new GeneralResult<Role[]>();
                                result.data = roles;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "query_role2":
                            {
                                string level = param.ContainsKey("level") ? param["level"] : "";
                                int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                                string error = "";
                                Role[] roles = QueryRole2(out error, dep, level);
                                GeneralResult<Role[]> result = new GeneralResult<Role[]>();
                                result.data = roles;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "add_department":
                            {
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                int order = param.ContainsKey("order") ? Convert.ToInt32(param["order"]) : 0;
                                int parent = param.ContainsKey("parent") ? Convert.ToInt32(param["parent"]) : 0;
                                int level = param.ContainsKey("level") ? Convert.ToInt32(param["level"]) : 0;
                                string alias = param.ContainsKey("alias") ? param["alias"] : "";
                                string code = param.ContainsKey("code") ? param["code"] : "";
                                string error = "";
                                Department dep = AddDepartment(out error, name, order, parent, level, alias, code);
                                GeneralResult<Department> result = new GeneralResult<Department>();
                                result.data = dep;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "update_department":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                string alias = param.ContainsKey("alias") ? param["alias"] : "";
                                string code = param.ContainsKey("code") ? param["code"] : "";
                                string error = "";
                                Department dep = UpdateDepartment(out error, id, name, alias, code);
                                GeneralResult<Department> result = new GeneralResult<Department>();
                                result.data = dep;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "delete_department":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string error = "";
                                bool flag = DeleteDepartment(out error, id);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "query_department":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                int parent = param.ContainsKey("parent") ? Convert.ToInt32(param["parent"]) : 0;
                                string error = "";
                                Department[] deps = QueryDepartments(out error, id, name, parent);
                                GeneralResult<Department[]> result = new GeneralResult<Department[]>();
                                result.data = deps;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "query_department2":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                int parent = param.ContainsKey("parent") ? Convert.ToInt32(param["parent"]) : 0;
                                string error = "";
                                Department[] deps = QueryDepartments(out error, id, name, parent);
                                DepartmentVue[] deps2 = SerializeDepTree(parent, deps);
                                GeneralResult<DepartmentVue[]> result = new GeneralResult<DepartmentVue[]>();
                                result.data = deps2;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "query_role_dep_relationship":
                            {
                                int role = param.ContainsKey("role") ? Convert.ToInt32(param["role"]) : 0;
                                string error = "";
                                Department[] deps = QueryRoleDepRelationship(out error, role);
                                GeneralResult<Department[]> result = new GeneralResult<Department[]>();
                                result.data = deps;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        //case "query_sub_dep":
                        //    {
                        //        int parent = param.ContainsKey("parent") ? Convert.ToInt32(param["parent"]) : 0;
                        //        string error = "";
                        //        Department[] deps = QuerySubDepartment(out error, parent);
                        //        GeneralResult<Department[]> result = new GeneralResult<Department[]>();
                        //        result.data = deps;
                        //        if (error == "")
                        //        {
                        //            result.success = true;
                        //        }
                        //        else
                        //        {
                        //            result.message = error;
                        //        }
                        //        str_result = JsonConvert.SerializeObject(result);
                        //        break;
                        //    }
                        //case "query_deparentment_contacts":
                        //    {
                        //        int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                        //        string name = param.ContainsKey("name") ? param["name"] : "";
                        //        int parent = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : -1;
                        //        string error = "";
                        //        Department[] deps = QueryDepartments(out error, id, name, parent);
                        //        GeneralResult<Department[]> result = new GeneralResult<Department[]>();
                        //        result.data = deps;
                        //        if (error == "")
                        //        {
                        //            result.success = true;
                        //        }
                        //        str_result = JsonConvert.SerializeObject(result);
                        //        break;
                        //    }
                        //case "addMenu":
                        //    {
                        //        AddMenu();
                        //        break;
                        //    }
                        //case "updateMenu":
                        //    {
                        //        int id = 0;
                        //        UpdateMenu();
                        //        break;
                        //    }
                        //case "deleteMenu":
                        //    {
                        //        int id = 0;
                        //        DeleteMenu();
                        //        break;
                        //    }
                        case "query_menu":
                            {
                                string error = "";
                                Menu[] menus = QueryMenus(out error);
                                GeneralResult<Menu[]> result = new GeneralResult<Menu[]>();
                                result.data = menus;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "query_role_menu_relationship":
                            {
                                string roleIds = param.ContainsKey("roles") ? param["roles"] : "";
                                string error = "";
                                string[] codes = QueryRoleMenuRelationship(out error, roleIds);
                                GeneralResult<string[]> result = new GeneralResult<string[]>();
                                result.data = codes;
                                if (error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "service_config":
                            {
                                str_result = "{";
                                str_result += string.Format("\"level1\":{0},", ConfCenter.ImportantUserRoleLevel);
                                str_result += string.Format("\"level2\":{0},", ConfCenter.AdministratorUserRoleLevel);
                                str_result += string.Format("\"level3\":{0}", ConfCenter.LoginAdminRoleLevel);
                                str_result += "}";
                                break;
                            }
                        case "get_map_data":
                            {
                                string error = "";
                                string base_path = "config/mapjson/";
                                GeneralResult<string> result = new GeneralResult<string>();
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                Department dep = QueryRegionByDepID(out error, id);
                                if(dep.code != "")
                                {
                                    using (FileStream fs = File.Open(AppHome.BaseDirectory + base_path + dep.code + ".json", FileMode.Open))
                                    {
                                        using (StreamReader sr = new StreamReader(fs))
                                        {
                                            result.data = sr.ReadToEnd();
                                            result.success = true;
                                        }
                                    }
                                }
                                if(error != "")
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "stat_region_data":
                            {
                                string error = "";
                                GeneralResult<StatisticResult[]> result = new GeneralResult<StatisticResult[]>();
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                result.data = StatRegionData(out error, id);
                                if(error == "")
                                {
                                    result.success = true;
                                }
                                else
                                {
                                    result.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "create_partial_table":
                            {
                                bool flag = false;
                                try
                                {
                                    string sql = "exec CreateSegmentTable";
                                    string error = "";
                                    DataBaseHelper.ExecuteNonQuery(sql, out error);
                                    if(error == "")
                                    {
                                        flag = true;
                                    }
                                }
                                catch(Exception ex)
                                {
                                    SystemLog.WriteErrorLog("分表创建失败", "1099", ex.Message, ex.StackTrace);
                                }
                                str_result = flag.ToString();
                                break;
                            }
                        default:
                            {
                                GeneralResult<string> result = new GeneralResult<string>("没有该方法，请检查参数");
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                    }
                }
                
                
                byte[] bytes = Encoding.UTF8.GetBytes(str_result);
                MemoryStream ms = new MemoryStream(bytes);

                return WebOperationContext.Current.CreateStreamResponse(ms, "application/json; charset=utf-8");
            }
            catch(Exception ex)
            {
                SystemLog.WriteErrorLog("请求失败", "2002", ex.Message, ex.StackTrace);
            }
            return null;
        }

        public GeneralResult<string> upload(Stream stream)
        {
            string str_error = "";
            string str_result = FileOperation.upload(out str_error, stream);
            if(str_result != "" && str_error == "")
            {
                GeneralResult<string> result = new GeneralResult<string>();
                result.data = str_result;
                result.success = true;
                return result;
            }
            return null;
        }

        public GeneralResult<string[]> upload2(Stream stream)
        {
            GeneralResult<string[]> result = new GeneralResult<string[]>();
            try
            {
                List<string> paths = new List<string>();
                MultipartParser parser = new MultipartParser(stream);
                string error;
                string file_name = Guid.NewGuid() + "." + parser.Filename.Split('.')[1];
                string single = FileOperation.SaveFile(out error, file_name, parser.FileContents);
                single = single.Substring(AppHome.BaseDirectory.Length - 1);
                single = single.Replace("\\", "/");
                if (error == "")
                {
                    paths.Add(single);
                }

                result.success = true;
                result.data = paths.ToArray();
            }
            catch(Exception ex)
            {
                SystemLog.WriteErrorLog("请求失败", "2002", ex.Message, ex.StackTrace);
            }

            return result;
        }
    }
}

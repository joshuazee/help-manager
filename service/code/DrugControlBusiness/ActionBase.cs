using CMHL.Entity;
using CMHL.Lawer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace DrugControlBusiness
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
                        case "get_app_version":
                            {
                                string error = "";
                                string type = param.ContainsKey("type") ? param["type"] : "";
                                string version = GetFileVersion(type);
                                GeneralResult<string> result = new GeneralResult<string>();
                                result.data = version;
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
                        case "query_homepage_picture":
                            {
                                string error = "";
                                string type = param.ContainsKey("type") ? param["type"] : "";
                                string[] paths = QueryHomepagePicture(out error, type);
                                GeneralResult<string[]> result = new GeneralResult<string[]>();
                                result.data = paths;
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
                        case "query_index_other_content": 
                            {
                                int id = param.ContainsKey("userid") ? Convert.ToInt32(param["userid"]) : 0;
                                int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                                GeneralResult<Dictionary<string, string>> result = new GeneralResult<Dictionary<string, string>>();
                                string str_error = "";
                                Dictionary<string, string> data = QueryIndexContent(out str_error, id, dep);
                                if (str_error == "")
                                {
                                    result.success = true;
                                    result.data = data;
                                }
                                else
                                {
                                    result.message = str_error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "count_sign_by_dep":
                            {
                                string dep = param.ContainsKey("dep") ? param["dep"] : "";
                                string startTime = param.ContainsKey("start_time") ? param["start_time"] : "";
                                string endTime = param.ContainsKey("end_time") ? param["end_time"] : "";
                                GeneralResult<string> result = new GeneralResult<string>();
                                string str_error = "";
                                string data = CountSignByDep(out str_error, dep, startTime, endTime);
                                if (str_error == "")
                                {
                                    result.success = true;
                                    result.data = data;
                                }
                                else
                                {
                                    result.message = str_error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "count_urinalysis_by_dep":
                            {
                                string dep = param.ContainsKey("dep") ? param["dep"] : "";
                                string startTime = param.ContainsKey("start_time") ? param["start_time"] : "";
                                string endTime = param.ContainsKey("end_time") ? param["end_time"] : "";
                                GeneralResult<string> result = new GeneralResult<string>();
                                string str_error = "";
                                string data = CountUrinalysisByDep(out str_error, dep, startTime, endTime);
                                if (str_error == "")
                                {
                                    result.success = true;
                                    result.data = data;
                                }
                                else
                                {
                                    result.message = str_error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "upload_news_images":
                            {
                                string images = param.ContainsKey("images") ? param["images"] : "";
                                MultipartParser parser = new MultipartParser(param["images"]);
                                string[] paths = parser.GetBase64Images();
                                GeneralResult<string[]> result = new GeneralResult<string[]>();
                                if(paths.Length > 0)
                                {
                                    result.success = true;
                                    result.data = paths;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "add_news":
                            {
                                int user = param.ContainsKey("user") ? Convert.ToInt32(param["user"]) : 0;
                                string title = param.ContainsKey("title") ? param["title"] : "";
                                string content = param.ContainsKey("content") ? param["content"] : "";
                                string type = param.ContainsKey("type") ? param["type"] : "";
                                string photo = param.ContainsKey("photo") ? param["photo"] : "";
                                int order = param.ContainsKey("order") ? Convert.ToInt32(param["order"]) : 0;
                                int top = param.ContainsKey("top") ? Convert.ToInt32(param["top"]) : 0;
                                int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                                string error = "";
                                bool flag = AddNews(out error, title, content, user, type, photo, order, top, dep);
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
                        case "update_news":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                int user = param.ContainsKey("user") ? Convert.ToInt32(param["user"]) : 0;
                                string title = param.ContainsKey("title") ? param["title"] : "";
                                string content = param.ContainsKey("content") ? param["content"] : "";
                                string type = param.ContainsKey("type") ? param["type"] : "";
                                string photo = param.ContainsKey("photo") ? param["photo"] : "";
                                int order = param.ContainsKey("order") ? Convert.ToInt32(param["order"]) : 0;
                                int top = param.ContainsKey("top") ? Convert.ToInt32(param["top"]) : 0;
                                string error = "";
                                bool flag = UpdateNews(out error, id, title, content, user, type, photo, order, top);
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
                        case "delete_news":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string error = "";
                                bool flag = DeleteNews(out error, id);
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
                        case "query_news":
                            {
                                int user = param.ContainsKey("user") ? Convert.ToInt32(param["user"]) : 0;
                                string title = param.ContainsKey("title") ? param["title"] : "";
                                string type = param.ContainsKey("type") ? param["type"] : "";
                                int top = param.ContainsKey("top") ? Convert.ToInt32(param["top"]) : 0;
                                string error = "";
                                News[] news = QueryNews(out error, type, user, title, top);
                                GeneralResult<News[]> result = new GeneralResult<News[]>();
                                result.data = news;
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
                        case "add_sign":
                            {
                                int user1 = param.ContainsKey("user1") ? Convert.ToInt32(param["user1"]) : 0;
                                string location1 = param.ContainsKey("location1") ? param["location1"] : "";
                                string address1 = param.ContainsKey("address1") ? param["address1"] : "";
                                string time1 = param.ContainsKey("time1") ? param["time1"] : "";
                                string photo = param.ContainsKey("photo") ? param["photo"] : "";
                                string type = param.ContainsKey("type") ? param["type"] : "";
                                string state = param.ContainsKey("state") ? param["state"] : "";
                                string remark = param.ContainsKey("remark") ? param["remark"] : "";
                                int user2 = param.ContainsKey("user2") ? Convert.ToInt32(param["user2"]) : 0;
                                string location2 = param.ContainsKey("location2") ? param["location2"] : "";
                                string address2 = param.ContainsKey("address2") ? param["address2"] : "";
                                string time2 = param.ContainsKey("time2") ? param["time2"] : "";
                                string appointment = param.ContainsKey("appointment") ? param["appointment"] : "";
                                string appointment_time = param.ContainsKey("appointment_time") ? param["appointment_time"] : "";
                                string error = "";
                                bool flag = AddSign(out error, user1, location1, address1, time1, photo, type, state, remark, user2, location2, address2, time2, appointment, appointment_time);
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
                        case "update_sign":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                int user1 = param.ContainsKey("user1") ? Convert.ToInt32(param["user1"]) : 0;
                                string location1 = param.ContainsKey("location1") ? param["location1"] : "";
                                string address1 = param.ContainsKey("address1") ? param["address1"] : "";
                                string time1 = param.ContainsKey("time1") ? param["time1"] : "";
                                string photo = param.ContainsKey("photo") ? param["photo"] : "";
                                string type = param.ContainsKey("type") ? param["type"] : "";
                                string state = param.ContainsKey("state") ? param["state"] : "";
                                string remark = param.ContainsKey("remark") ? param["remark"] : "";
                                int user2 = param.ContainsKey("user2") ? Convert.ToInt32(param["user2"]) : 0;
                                string location2 = param.ContainsKey("location2") ? param["location2"] : "";
                                string address2 = param.ContainsKey("address2") ? param["address2"] : "";
                                string time2 = param.ContainsKey("time2") ? param["time2"] : "";
                                string appointment = param.ContainsKey("appointment") ? param["appointment"] : "";
                                string error = "";
                                bool flag = UpdateSign(out error, id, user1, location1, address1, time1, photo, type, state, remark, user2, location2, address2, time2, appointment);
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
                        case "delete_sign":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string error = "";
                                bool flag = DeleteSign(out error, id);
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
                        case "query_sign":
                            {
                                //int leader = param.ContainsKey("leader") ? Convert.ToInt32(param["leader"]) : 0;
                                //int user = param.ContainsKey("user") ? Convert.ToInt32(param["user"]) : 0;
                                //string mobile = param.ContainsKey("mobile") ? param["mobile"] : "";
                                //string identity = param.ContainsKey("identity") ? param["identity"] : "";
                                //string time = param.ContainsKey("time") ? param["time"] : "";
                                int user = 0;
                                if (param.ContainsKey("user"))
                                {
                                    int.TryParse(param["user"], out user);
                                }
                                int year = param.ContainsKey("year") ? Convert.ToInt32(param["year"]) : 0;
                                int month = param.ContainsKey("month") ? Convert.ToInt32(param["month"]) : 0;
                                int leader = param.ContainsKey("leader") ? Convert.ToInt32(param["leader"]) : 0;
                                string state = param.ContainsKey("state") ? param["state"] : "";
                                int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                                string error = "";
                                SignInfo[] info = QuerySign(out error, leader, user, year, month, state, dep);
                                GeneralResult<SignInfo[]> result = new GeneralResult<SignInfo[]>();
                                result.data = info;
                                if (string.IsNullOrWhiteSpace(error))
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
                        case "query_sign_app":
                            {
                                int user1 = param.ContainsKey("user1") ? Convert.ToInt32(param["user1"]) : 0;
                                int user2 = param.ContainsKey("user2") ? Convert.ToInt32(param["user2"]) : 0;
                                string type = param.ContainsKey("type") ? param["type"] : "";
                                string state = param.ContainsKey("state") ? param["state"] : "";
                                string time = param.ContainsKey("time") ? param["time"] : "";
                                string error = "";
                                SignInfo[] info = QuerySignForApp(out error, user1, user2, type, state, time);
                                GeneralResult<SignInfo[]> result = new GeneralResult<SignInfo[]>();
                                result.data = info;
                                if (string.IsNullOrWhiteSpace(error))
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
                        case "add_trace_point":
                            {
                                double x = param.ContainsKey("x") ? Convert.ToDouble(param["x"]) : 0;
                                double y = param.ContainsKey("y") ? Convert.ToDouble(param["y"]) : 0;
                                int user = param.ContainsKey("user") ? Convert.ToInt32(param["user"]) : 0;
                                string error = "";
                                bool flag = AddTracePoint(out error, x, y, user);
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                result.data = flag;
                                if (string.IsNullOrWhiteSpace(error))
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
                        case "query_trace":
                            {
                                int user = param.ContainsKey("user") ? Convert.ToInt32(param["user"]) : 0;
                                string time = param.ContainsKey("time") ? param["time"] : "";
                                string error = "";
                                Trace trace = QueryTrace(out error, user, time);
                                GeneralResult<Trace> result = new GeneralResult<Trace>();
                                result.data = trace;
                                if (string.IsNullOrWhiteSpace(error))
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
                        case "add_urinalysis":
                            {
                                int user1 = param.ContainsKey("user1") ? Convert.ToInt32(param["user1"]) : 0;
                                string result = param.ContainsKey("result") ? param["result"] : "";
                                string remark = param.ContainsKey("remark") ? param["remark"] : "";
                                string photo = param.ContainsKey("photo") ? param["photo"] : "";
                                string error = "";
                                bool flag = AddUrinalysisRecord(out error, user1, result, remark, photo);
                                GeneralResult<bool> r = new GeneralResult<bool>();
                                r.data = flag;
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    r.success = true;
                                }
                                else
                                {
                                    r.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(r);
                                break;
                            }
                        case "update_urinalysis":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                int user2 = param.ContainsKey("user2") ? Convert.ToInt32(param["user2"]) : 0;
                                string result = param.ContainsKey("result") ? param["result"] : "";
                                string remark = param.ContainsKey("remark") ? param["remark"] : "";
                                string photo = param.ContainsKey("photo") ? param["photo"] : "";
                                string state = param.ContainsKey("state") ? param["state"] : "";
                                string error = "";
                                bool flag = UpdateUrinalysisRecord(out error, id, user2, result, remark, photo, state);
                                GeneralResult<bool> r = new GeneralResult<bool>();
                                r.data = flag;
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    r.success = true;
                                }
                                else
                                {
                                    r.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(r);
                                break;
                            }
                        case "delete_urinalysis":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string error = "";
                                bool flag = DeleteUrinalysisRecord(out error, id);
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
                        case "query_urinalysis":
                            {
                                int user = 0;
                                if(param.ContainsKey("user"))
                                {
                                    int.TryParse(param["user"], out user);
                                }
                                int year = param.ContainsKey("year") ? Convert.ToInt32(param["year"]) : 0;
                                int month = param.ContainsKey("month") ? Convert.ToInt32(param["month"]) : 0;
                                int leader = param.ContainsKey("leader") ? Convert.ToInt32(param["leader"]) : 0;
                                string result = param.ContainsKey("result") ? param["result"] : "";
                                string state = param.ContainsKey("state") ? param["state"] : "";
                                int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                                string error = "";
                                UrinalysisRecord[] list = QueryUrinalysisRecord(out error, user, year, month, leader, result, state, dep);
                                GeneralResult<UrinalysisRecord[]> r = new GeneralResult<UrinalysisRecord[]>();
                                r.data = list;
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    r.success = true;
                                }
                                else
                                {
                                    r.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(r);
                                break;
                            }
                        case "add_video":
                            {
                                string title = param.ContainsKey("title") ? param["title"] : "";
                                string uploader = param.ContainsKey("uploader") ? param["uploader"] : "";
                                string url = param.ContainsKey("url") ? param["url"] : "";
                                int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                                string error = "";
                                bool flag = AddVideo(out error, title, uploader, url, dep);
                                GeneralResult<bool> r = new GeneralResult<bool>();
                                r.data = flag;
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    r.success = true;
                                }
                                else
                                {
                                    r.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(r);
                                break;
                            }
                        case "delete_video":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string error = "";
                                bool flag = DeleteVideo(out error, id);
                                GeneralResult<bool> r = new GeneralResult<bool>();
                                r.data = flag;
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    r.success = true;
                                }
                                else
                                {
                                    r.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(r);
                                break;
                            }
                        case "query_video":
                            {
                                string error = "";
                                VideoRecord[] list = QueryVideo(out error);
                                GeneralResult<VideoRecord[]> r = new GeneralResult<VideoRecord[]>();
                                r.data = list;
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    r.success = true;
                                }
                                else
                                {
                                    r.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(r);
                                break;
                            }
                        case "add_fence":
                            {
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                string type = param.ContainsKey("type") ? param["type"] : "";
                                string extent = param.ContainsKey("extent") ? param["extent"] : "";
                                string error = "";
                                bool flag = AddFence(out error, name, type, extent);
                                GeneralResult<bool> r = new GeneralResult<bool>();
                                r.data = flag;
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    r.success = true;
                                }
                                else
                                {
                                    r.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(r);
                                break;
                            }
                        case "update_fence":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string name = param.ContainsKey("name") ? param["name"] : "";
                                string extent = param.ContainsKey("extent") ? param["extent"] : "";
                                string error = "";
                                bool flag = UpdateFence(out error, id, name, extent);
                                GeneralResult<bool> r = new GeneralResult<bool>();
                                r.data = flag;
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    r.success = true;
                                }
                                else
                                {
                                    r.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(r);
                                break;
                            }
                        case "delete_fence":
                            {
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string error = "";
                                bool flag = DeleteFence(out error, id);
                                GeneralResult<bool> r = new GeneralResult<bool>();
                                r.data = flag;
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    r.success = true;
                                }
                                else
                                {
                                    r.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(r);
                                break;
                            }
                        case "query_fence":
                            {
                                string error = "";
                                FenceRecord[] list = QueryFence(out error);
                                GeneralResult<FenceRecord[]> r = new GeneralResult<FenceRecord[]>();
                                r.data = list;
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    r.success = true;
                                }
                                else
                                {
                                    r.message = error;
                                }
                                str_result = JsonConvert.SerializeObject(r);
                                break;
                            }
                        case "query_related_dep":
                            {
                                string error = "";
                                GeneralResult<ZTreeNode[]> result = new GeneralResult<ZTreeNode[]>();
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                result.data = QueryRelatedDep(out error, id);
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
                        case "fence_relate_dep":
                            {
                                string error = "";
                                GeneralResult<bool> result = new GeneralResult<bool>();
                                int id = param.ContainsKey("id") ? Convert.ToInt32(param["id"]) : 0;
                                string deps = param.ContainsKey("deps") ? param["deps"].ToString() : "";
                                result.data = FenceRelateDep(out error, id, deps);
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
                        case "query_region_map_data":
                            {
                                string regionCode = param.ContainsKey("code") ? param["code"] : "";
                                GeneralResult<string> result = new GeneralResult<string>();
                                result.data = QueryRegionMapData(regionCode);
                                if(!string.IsNullOrWhiteSpace(result.data))
                                {
                                    result.success = true;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        case "count_active_user_by_dep":
                            {
                                string dep = param.ContainsKey("dep") ? param["dep"] : "";
                                string startTime = param.ContainsKey("start_time") ? param["start_time"] : "";
                                string endTime = param.ContainsKey("end_time") ? param["end_time"] : "";
                                GeneralResult<string> result = new GeneralResult<string>();
                                string str_error = "";
                                string data = CountActiveUser(out str_error, dep, startTime, endTime);
                                if (str_error == "")
                                {
                                    result.success = true;
                                    result.data = data;
                                }
                                else
                                {
                                    result.message = str_error;
                                }
                                str_result = JsonConvert.SerializeObject(result);
                                break;
                            }
                        //case "index_statistics":
                        //    {
                        //        string error = "";
                        //        GeneralResult<string> result = new GeneralResult<string>();
                        //        int dep = param.ContainsKey("dep") ? Convert.ToInt32(param["dep"]) : 0;
                        //        result.data = IndexStatistics(out error, dep);
                        //        if(error == "")
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

        //public bool VideoUpload(string title, string uploader, Stream stream)
        //{
        //    try
        //    {
        //        MultipartParser parser = new MultipartParser(stream);
        //        string error;
        //        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + parser.Filename.Split('.')[1];
        //        string filePath = FileOperation.SaveFile(out error, fileName, parser.FileContents);
                
        //        string url = "";
        //        Utilities.FileUploadToCloudQiniu(title, filePath);
        //        //数据库处理
        //        string sql = string.Format("insert into video_records values('{0}', '{1}', '{2}', getdate(), 0)", title, uploader, url);
        //        int count = DataBaseHelper.ExecuteNonQuery(sql, out error);
        //        if(error == "" && count == 1)
        //        {
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SystemLog.WriteErrorLog("视频上传失败", "1021", ex.Message, ex.StackTrace);
        //    }
        //    return false;
        //}

        private string GetFileVersion(string type)
        {
            string version = "";
            string where = "where 1 = 1";
            if(!string.IsNullOrEmpty(type))
            {
                where+= string.Format(" and version_type='{0}'", type);
            }
            else
            {
                where += string.Format(" and (version_type = '' or version_type is null)");
            }
            string sql = string.Format("select top 1 version_code from app_version_records {0} order by create_time desc", where);
            string error = "";
            version = DataBaseHelper.ExecuteScalar(sql, out error).ToString();

            return version;
        }
    }
}

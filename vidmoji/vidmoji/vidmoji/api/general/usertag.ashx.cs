using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.general
{
    /// <summary>
    /// Summary description for usertag
    /// </summary>
    public class usertag : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = new StreamReader(context.Request.InputStream).ReadToEnd();
            var responseMsg = new Dictionary<string, string>();
            
            var _videoobj = new UserTagsBll();
            var _ld_video_data = new Dictionary<string, UserTagsObject>();

            if ((context.Request.Params["action"] != null))
            {
                switch (context.Request.Params["action"])
                {
                    case "process":
                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        UserTagsBll.Process(JsonConvert.DeserializeObject<UserTagEntity>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "process_v2":
                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        UserTagsBll.Process_V2(JsonConvert.DeserializeObject<UserTagEntity>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "process_mytag":
                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        UserTagsBll.Process_MyTAG(JsonConvert.DeserializeObject<UserTagEntity>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "delete":

                        UserTagsBll.Delete(JsonConvert.DeserializeObject<UserTagEntity>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;
                    case "update":

                        var _last_post = JsonConvert.DeserializeObject<UserTagEntity>(json);

                        UserTagsBll.Update(JsonConvert.DeserializeObject<UserTagEntity>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "update_status":
                        

                        UserTagsBll.Update_Status(JsonConvert.DeserializeObject<UserTagEntity>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;
                    case "check":
                        if (UserTagsBll.Check_Tag(JsonConvert.DeserializeObject<UserTagEntity>(json)))
                        {
                            responseMsg["status"] = "success";
                            responseMsg["message"] = "Validated";
                        }
                        else
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Not Validated";
                        }
                        context.Response.Write(responseMsg);
                        break;
                    case "validate_tags":
                        string usertags = "";
                        if (context.Request.Params["tag"] != null)
                        {
                            usertags = context.Request.Params["tag"].ToString();
                        }
                        if (UserTagsBll.Validate_Tags(usertags))
                        {
                            responseMsg["status"] = "success";
                            responseMsg["message"] = "Validated";
                        }
                        else
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Not Validated";
                        }
                        context.Response.Write(responseMsg);
                        break;

                    case "load_tags":

                        var _ld_video_json = JsonConvert.DeserializeObject<UserTagEntity>(json);
                        var _vObject = new UserTagsObject()
                        {
                            Data = UserTagsBll.LoadTags(_ld_video_json),
                            Count = UserTagsBll.CountItems(_ld_video_json)
                        };

                        var _ld_tag_data = new Dictionary<string, UserTagsObject>();

                        _ld_tag_data["data"] = _vObject;

                        context.Response.Write(_ld_tag_data);

                        break;
                }
            }
            else
            {
                // No action found
                responseMsg["status"] = "error";
                responseMsg["message"] = "No action found";
                context.Response.Write(JsonConvert.SerializeObject(responseMsg));
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class UserTagsObject
    {
        public List<UserTagEntity> Data { get; set; }
        public int Count { get; set; }
    }
}
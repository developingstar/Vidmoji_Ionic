using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
namespace vidmoji.api.groups
{
    /// <summary>
    /// Summary description for Process
    /// </summary>
    public class Process : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = new StreamReader(context.Request.InputStream).ReadToEnd();
            var responseMsg = new Dictionary<string, string>();

            long GroupID = 0;
            int Type = 0;
            string UserName = "";
            bool IsUpdate = false;
            long GalleryID = 0;
            int Status = 0;
            int isApproved = 0;
            int OldValue = 0;
            int NewValue = 0;
            string Value = "";
            string FieldName = "";
            bool isAdmin = false;
            int Records = 0;
            var _videoobj = new GroupsBLL();
            var _ld_video_data = new Dictionary<string, GroupsObject>();

            if ((context.Request.Params["action"] != null))
            {
                switch (context.Request.Params["action"])
                {
                    case "add":
                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }
                        if (context.Request.Params["isadmin"] != null)
                        {
                            isAdmin = Convert.ToBoolean(context.Request.Params["isadmin"]);
                        }
                        GroupsBLL.Add(JsonConvert.DeserializeObject<Group_Struct>(json), isAdmin);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;


                    case "check":

                        if (context.Request.Params["vid"] != null)
                        {
                            GroupID = Convert.ToInt64(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (GroupsBLL.Check(GroupID, UserName))
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



                    // This update is only for pubplishing pending videos (unpublished videos only)
                    case "update":

                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        GroupsBLL.Update(JsonConvert.DeserializeObject<Group_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    
                    case "update_info":

                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        GroupsBLL.UpdateInfo(JsonConvert.DeserializeObject<Group_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;
                                           
                 
                    case "delete":

                        var _del_photo = JsonConvert.DeserializeObject<Group_Struct>(json);

                        GroupsBLL.Delete(_del_photo.GroupID, _del_photo.UserName);

                        break;


                    case "count":
                        
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                          
                        }

                        var _Output = new Dictionary<string, int>();
                        _Output["records"] = GroupsBLL.Count(UserName);

                        context.Response.Write(_Output);

                        break;

                   

                    case "update_field":

                        if (context.Request.Params["vid"] != null)
                        {
                            GroupID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["val"] != null)
                        {
                            Value = context.Request.Params["val"].ToString();
                        }
                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }

                        GroupsBLL.Update_Field(GroupID, Value, FieldName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "get_field_value":

                        if (context.Request.Params["vid"] != null)
                        {
                            GroupID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }



                        responseMsg["value"] = GroupsBLL.Get_Field_Value(GroupID, FieldName);

                        context.Response.Write(responseMsg);
                        break;

                 

                    case "load_groups":

                        var _ld_video_json = JsonConvert.DeserializeObject<Group_Struct>(json);
                        var _vObject = new GroupsObject()
                        {
                            Data = GroupsBLL.Load_Groups_V3(_ld_video_json),
                            Count = GroupsBLL.Cache_Count_Groups_V3(_ld_video_json)
                        };

                        _ld_video_data["data"] = _vObject;

                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_photos_limit":
                        _ld_video_data["data"] = new GroupsObject()
                        {
                            Data = GroupsBLL.Fetch_Groups_Limit(JsonConvert.DeserializeObject<Group_Struct>(json)),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_record":
                        if (context.Request.Params["vid"] != null)
                        {
                            GroupID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new GroupsObject()
                        {
                            Data = GroupsBLL.Fetch_Group_Record(GroupID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_group_sm_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            GroupID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new GroupsObject()
                        {
                            Data = GroupsBLL.Fetch_Group_SM_Info(GroupID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "get_group_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            GroupID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new GroupsObject()
                        {
                            Data = GroupsBLL.Get_Group_Info(GroupID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

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

    public class GroupsObject
    {
        public List<Group_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}
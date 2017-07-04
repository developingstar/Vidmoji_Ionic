using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.forums
{
    /// <summary>
    /// Summary description for Procss
    /// </summary>
    public class Procss : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = new StreamReader(context.Request.InputStream).ReadToEnd();
            var responseMsg = new Dictionary<string, string>();

            int ForumID = 0;
            long TopicID = 0;
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
            var _videoobj = new Forum_Topics();
            var _ld_video_data = new Dictionary<string, ForumTopicsObject>();

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
                        if (context.Request.Params["isupdate"] != null)
                        {
                            IsUpdate = Convert.ToBoolean(context.Request.Params["isupdate"]);
                        }
                        Forum_Topics.Process(JsonConvert.DeserializeObject<Forum_Topics_Struct>(json), IsUpdate,isAdmin);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;


                    case "check":

                        if (context.Request.Params["vid"] != null)
                        {
                            TopicID = Convert.ToInt64(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (Forum_Topics.Check(TopicID, UserName))
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


                    case "update_topic_stats":

                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        Forum_Topics.Update_Topic_Stats(JsonConvert.DeserializeObject<Forum_Topics_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "delete":

                        var _rem_photo = JsonConvert.DeserializeObject<Forum_Topics_Struct>(json);

                        Forum_Topics.Delete(_rem_photo.TopicID,_rem_photo.ForumID, _rem_photo.UserName);
                        
                        break;

                   

                    case "update_isenabled":

                        if (context.Request.Params["oval"] != null)
                        {
                            OldValue = Convert.ToInt32(context.Request.Params["oval"]);
                        }
                        if (context.Request.Params["nval"] != null)
                        {
                            NewValue = Convert.ToInt32(context.Request.Params["nval"]);
                        }

                        var _upd_isenabled = JsonConvert.DeserializeObject<Forum_Topics_Struct>(json);

                        Forum_Topics.Update_IsEnabled(_upd_isenabled.TopicID, OldValue, NewValue, _upd_isenabled.ForumID, _upd_isenabled.UserName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "update_isapproved":

                        if (context.Request.Params["oval"] != null)
                        {
                            OldValue = Convert.ToInt32(context.Request.Params["oval"]);
                        }
                        if (context.Request.Params["nval"] != null)
                        {
                            NewValue = Convert.ToInt32(context.Request.Params["nval"]);
                        }

                        var _upd_isreviewed = JsonConvert.DeserializeObject<Forum_Topics_Struct>(json);

                        Forum_Topics.Update_IsApproved(_upd_isreviewed.TopicID, OldValue, NewValue, _upd_isreviewed.ForumID, _upd_isreviewed.UserName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;


                    case "update_field":

                        if (context.Request.Params["vid"] != null)
                        {
                            TopicID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["val"] != null)
                        {
                            Value = context.Request.Params["val"].ToString();
                        }
                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }

                        Forum_Topics.Update_Value(TopicID,  FieldName, Convert.ToInt64(Value));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "get_field_value":

                        if (context.Request.Params["vid"] != null)
                        {
                            TopicID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }



                        responseMsg["value"] = Forum_Topics.Return_Value(TopicID, FieldName);

                        context.Response.Write(responseMsg);
                        break;

                    case "mark_as_resolved":

                        if (context.Request.Params["vid"] != null)
                        {
                            TopicID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        long PostID = 0;
                        if (context.Request.Params["pid"] != null)
                        {
                            PostID = Convert.ToInt32(context.Request.Params["pid"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (context.Request.Params["value"] != null)
                        {
                            Value = context.Request.Params["value"].ToString();
                        }
                        Forum_Topics.MarkAsResolved(TopicID, PostID, UserName, Convert.ToInt32(Value));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "increment_views":

                        var _inc_obj = JsonConvert.DeserializeObject<Forum_Topics_Struct>(json);

                        Forum_Topics.Increment_Views(_inc_obj.TopicID, _inc_obj.Views);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;
                    case "load_topics":

                        var _ld_video_json = JsonConvert.DeserializeObject<Forum_Topics_Struct>(json);
                        var _vObject = new ForumTopicsObject()
                        {
                            Data = Forum_Topics.Load_Topics_V4(_ld_video_json),
                            Count = Forum_Topics.Cache_Count_Topics_V3(_ld_video_json)
                        };

                        _ld_video_data["data"] = _vObject;

                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_forums_limit":
                        _ld_video_data["data"] = new ForumTopicsObject()
                        {
                            Data = Forum_Topics.Load_Topics_Limit(JsonConvert.DeserializeObject<Forum_Topics_Struct>(json)),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "Fetch_Record":
                        if (context.Request.Params["vid"] != null)
                        {
                            TopicID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        int PageNumber = 1;
                        if (context.Request.Params["p"] != null)
                        {
                            PageNumber = Convert.ToInt32(context.Request.Params["p"]);
                        }
                        int PageSize = 20;
                        if (context.Request.Params["size"] != null)
                        {
                            PageSize = Convert.ToInt32(context.Request.Params["size"]);
                        }
                        bool SinglePost = false;
                        if (context.Request.Params["spost"] != null)
                        {
                            SinglePost = Convert.ToBoolean(context.Request.Params["spost"]);
                        }
                        _ld_video_data["data"] = new ForumTopicsObject()
                        {
                            Data = Forum_Topics.Fetch_Record(TopicID,PageNumber,PageSize, SinglePost),
                            Count = Forum_Topics.Count_Topics_V3(TopicID)
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_topic_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            TopicID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new ForumTopicsObject()
                        {
                            Data = Forum_Topics.Load_Topic_Info(TopicID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_last_post":

                        if (context.Request.Params["vid"] != null)
                        {
                            TopicID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new ForumTopicsObject()
                        {
                            Data = Forum_Topics.Load_Last_Post(TopicID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                                            

                    case "archive_list":

                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }
                        if (context.Request.Params["records"] != null)
                        {
                            Records = Convert.ToInt32(context.Request.Params["records"]);
                        }
                        bool isAll = false;
                        if (context.Request.Params["isall"] != null)
                        {
                            isAll = Convert.ToBoolean(context.Request.Params["isall"]);
                        }
                        var _archive = new Dictionary<string, List<Archive_Struct>>();
                        _archive["data"] = Forum_Topics.Load_Arch_List(Records, isAll);
                        context.Response.Write(_archive);

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

    public class ForumTopicsObject
    {
        public List<Forum_Topics_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}
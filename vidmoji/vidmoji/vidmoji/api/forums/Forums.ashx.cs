using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.forums
{
    /// <summary>
    /// Summary description for Forums
    /// </summary>
    public class Forums : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = new StreamReader(context.Request.InputStream).ReadToEnd();
            var responseMsg = new Dictionary<string, string>();

            int ForumID = 0;
           
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
            var _videoobj = new ForumBLLC();
            var _ld_video_data = new Dictionary<string, ForumObject>();

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
                        
                        if (context.Request.Params["isupdate"] != null)
                        {
                            IsUpdate = Convert.ToBoolean(context.Request.Params["isupdate"]);
                        }
                        ForumBLLC.Process(JsonConvert.DeserializeObject<Forum_Struct>(json), IsUpdate);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                        
                    case "delete":

                        var _rem_forum = JsonConvert.DeserializeObject<Forum_Struct>(json);

                        ForumBLLC.Delete(_rem_forum.ForumID);

                        break;
                    case "upate_last_post":

                        var _last_post = JsonConvert.DeserializeObject<Forum_Struct>(json);

                        ForumBLLC.Update_Last_Post(_last_post.ForumID, _last_post.LastPostID);

                        break;

                    case "update_isenabled":

                        if (context.Request.Params["nval"] != null)
                        {
                            NewValue = Convert.ToInt32(context.Request.Params["nval"]);
                        }

                        var _upd_isenabled = JsonConvert.DeserializeObject<Forum_Struct>(json);

                        ForumBLLC.Update_IsEnabled(_upd_isenabled.ForumID, NewValue);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;


                    case "update_field":

                        if (context.Request.Params["vid"] != null)
                        {
                            ForumID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["val"] != null)
                        {
                            Value = context.Request.Params["val"].ToString();
                        }
                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }

                        ForumBLLC.Update_Value(ForumID, FieldName, Convert.ToInt64(Value));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "get_field_value":

                        if (context.Request.Params["vid"] != null)
                        {
                            ForumID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }



                        responseMsg["value"] = ForumBLLC.Return_Value(ForumID, FieldName).ToString();

                        context.Response.Write(responseMsg);
                        break;

                    
                    case "load_topics":

                        var _ld_video_json = JsonConvert.DeserializeObject<Forum_Struct>(json);
                        var _vObject = new ForumObject()
                        {
                            Data = ForumBLLC.Load_Forums_V4(_ld_video_json),
                            Count = ForumBLLC.Cache_Count_Forums_V4(_ld_video_json)
                        };

                        _ld_video_data["data"] = _vObject;

                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_forums_limit":
                        _ld_video_data["data"] = new ForumObject()
                        {
                            Data = ForumBLLC.Load_Forum_Limit(JsonConvert.DeserializeObject<Forum_Struct>(json)),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_record_admin":
                        if (context.Request.Params["vid"] != null)
                        {
                            ForumID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                     
                        _ld_video_data["data"] = new ForumObject()
                        {
                            Data = ForumBLLC.Fetch_Record_Admin(ForumID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_forums":

                     
                        _ld_video_data["data"] = new ForumObject()
                        {
                            Data = ForumBLLC.Load_Forums(),
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

    public class ForumObject
    {
        public List<Forum_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}
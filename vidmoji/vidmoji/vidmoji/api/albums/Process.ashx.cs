using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.albums
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
            int Records = 0;
            bool isAdmin = false;
            var _videoobj = new GalleryBLLC();
            var _ld_video_data = new Dictionary<string, AlbumsObject>();

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
                        GalleryBLLC.Add(JsonConvert.DeserializeObject<Gallery_Struct>(json),isAdmin);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "check":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt64(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (GalleryBLLC.Check(GalleryID, UserName))
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

                    case "validate_album":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt64(context.Request.Params["vid"]);
                        }
                       
                        if (GalleryBLLC.Validate_GalleryID(GalleryID))
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

                        GalleryBLLC.Update(JsonConvert.DeserializeObject<Gallery_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    // Update video information - myaccount section - edit video
                    case "update_myaccount":

                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        GalleryBLLC.Update_MyAccount(JsonConvert.DeserializeObject<Gallery_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                   

                    case "delete":

                        var _rem_photo = JsonConvert.DeserializeObject<Gallery_Struct>(json);

                        GalleryBLLC.Delete(_rem_photo.GalleryID);

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

                        var _upd_isenabled = JsonConvert.DeserializeObject<Gallery_Struct>(json);

                        GalleryBLLC.Update_IsEnabled(_upd_isenabled.GalleryID, OldValue, NewValue, _upd_isenabled.UserName, _upd_isenabled.Type);

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

                        var _upd_isreviewed = JsonConvert.DeserializeObject<Gallery_Struct>(json);

                        GalleryBLLC.Update_IsApproved(_upd_isreviewed.GalleryID, OldValue, NewValue, _upd_isreviewed.UserName, _upd_isreviewed.Type);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;


                    case "update_field":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["val"] != null)
                        {
                            Value = context.Request.Params["val"].ToString();
                        }
                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }

                        GalleryBLLC.Update_Field(GalleryID, Value, FieldName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "get_field_value":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }



                        responseMsg["value"] = GalleryBLLC.Get_Field_Value(GalleryID, FieldName);

                        context.Response.Write(responseMsg);
                        break;

                    case "count_photos":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        if (context.Request.Params["status"] != null)
                        {
                            Status = Convert.ToInt32(context.Request.Params["status"]);
                        }
                       



                        responseMsg["value"] = GalleryBLLC.Count_Photos(GalleryID,Status).ToString();

                        context.Response.Write(responseMsg);
                        break;
                    case "count_videos":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        if (context.Request.Params["status"] != null)
                        {
                            Status = Convert.ToInt32(context.Request.Params["status"]);
                        }




                        responseMsg["value"] = GalleryBLLC.Count_Videos(GalleryID, Status).ToString();

                        context.Response.Write(responseMsg);
                        break;

                   

                    case "load_albums":

                        var _ld_video_json = JsonConvert.DeserializeObject<Gallery_Struct>(json);
                        var _vObject = new AlbumsObject()
                        {
                            Data = _videoobj.Load_Galleries_V4(_ld_video_json),
                            Count = _videoobj.Cache_Count_Galleries_V4(_ld_video_json)
                        };

                        _ld_video_data["data"] = _vObject;

                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_photos_limit":
                        _ld_video_data["data"] = new AlbumsObject()
                        {
                            Data = _videoobj.Load_Galleries_Limit(JsonConvert.DeserializeObject<Gallery_Struct>(json)),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_record_sm":
                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new AlbumsObject()
                        {
                            Data = GalleryBLLC.Fetch_Gallery_Record_SM(GalleryID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_album_myaccount":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new AlbumsObject()
                        {
                            Data = GalleryBLLC.Fetch_Gallery_MyAccount(GalleryID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_record":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new AlbumsObject()
                        {
                            Data = GalleryBLLC.Fetch_Gallery_Record(GalleryID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_gallery_summary":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        _ld_video_data["data"] = new AlbumsObject()
                        {
                            Data = GalleryBLLC.Fetch_Gallery_Summary(GalleryID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_gallery_record_admin":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        _ld_video_data["data"] = new AlbumsObject()
                        {
                            Data = GalleryBLLC.Fetch_Gallery_Record_Admin(GalleryID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "load_gallery_category_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        _ld_video_data["data"] = new AlbumsObject()
                        {
                            Data = GalleryBLLC.Load_Gallery_Category_Info(GalleryID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_gallery_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        
                        _ld_video_data["data"] = new AlbumsObject()
                        {
                            Data = GalleryBLLC.Fetch_Gallery_Info(GalleryID,UserName, isAdmin),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_user_albums":

                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }

                        _ld_video_data["data"] = new AlbumsObject()
                        {
                            Data = GalleryBLLC.Load_User_Albums(UserName, Type),
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
                        _archive["data"] = GalleryBLLC.Load_Arch_List(Records, isAll, Type);
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

        public class AlbumsObject
        {
            public List<Gallery_Struct> Data { get; set; }
            public int Count { get; set; }
        }
    }
}
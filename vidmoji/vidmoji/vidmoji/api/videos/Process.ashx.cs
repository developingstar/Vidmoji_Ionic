using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.videos
{
    /// <summary>
    /// Summary description for Process
    /// </summary>
    public class Process : IHttpHandler
    {
        // this api handles all BLL (VideoBLL.cs) -> For Videos / Audio
        public void ProcessRequest(HttpContext context)
        {
            var json = new StreamReader(context.Request.InputStream).ReadToEnd();
            var responseMsg = new Dictionary<string, string>();

            long VideoID = 0;
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
            var _videoobj = new VideoBLL();
            var _ld_video_data = new Dictionary<string, VideoObject>();

            if ((context.Request.Params["action"] != null))
            {
                switch (context.Request.Params["action"])
                {
                    case "process_video_file_info":
                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }
                        
                        VideoBLL.Process_Video_File_Information(JsonConvert.DeserializeObject<Video_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "process_video_cloud_info":
                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        VideoBLL.Process_Video_Cloud_Information(JsonConvert.DeserializeObject<Video_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "check":
                        
                        if(context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt64(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if(VideoBLL.Check(VideoID, UserName))
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


                    case "save":
                        
                        if (context.Request.Params["isupdate"] != null)
                        {
                            IsUpdate = Convert.ToBoolean(context.Request.Params["isupdate"]);
                        }
                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        VideoBLL.Process_Info(JsonConvert.DeserializeObject<Video_Struct>(json), IsUpdate);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
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

                        VideoBLL.Update(JsonConvert.DeserializeObject<Video_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    // Update video information - myaccount section - edit video
                    case "update_video_info":

                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        VideoBLL.Update_Video_Info(JsonConvert.DeserializeObject<Video_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "remove_video":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt64(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }
                        VideoBLL.RemoveVideo(VideoID, Type);
                        break;

                    case "update_video_adm":


                        VideoBLL.Update_VideoInfo_Adm(JsonConvert.DeserializeObject<Video_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;
                    case "update_info":


                        VideoBLL.Update_Info(JsonConvert.DeserializeObject<Video_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;
                    case "update_isapproved_all":

                        if (context.Request.Params["gid"] != null)
                        {
                            GalleryID = Convert.ToInt64(context.Request.Params["gid"]);
                        }
                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }
                        if (context.Request.Params["status"] != null)
                        {
                            Status = Convert.ToInt32(context.Request.Params["status"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        VideoBLL.Update_isApproved_All(GalleryID, UserName, Status, Type);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "update_isenabled_all":

                        if (context.Request.Params["gid"] != null)
                        {
                            GalleryID = Convert.ToInt64(context.Request.Params["gid"]);
                        }
                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }
                        if (context.Request.Params["status"] != null)
                        {
                            Status = Convert.ToInt32(context.Request.Params["status"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        VideoBLL.Update_isEnabled_All(GalleryID, UserName, Status, Type);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "count":

                        if (context.Request.Params["gid"] != null)
                        {
                            GalleryID = Convert.ToInt64(context.Request.Params["gid"]);
                        }
                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }
                        if (context.Request.Params["status"] != null)
                        {
                            Status = Convert.ToInt32(context.Request.Params["status"]);
                        }
                        if (context.Request.Params["approved"] != null)
                        {
                            isApproved = Convert.ToInt32(context.Request.Params["approved"]);
                        }
                        bool isUserName = false;
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                            isUserName = true;
                        }
                        
                        var _Output = new Dictionary<string, int>();
                        if (isUserName)
                        {
                            _Output["records"] = VideoBLL.Count(UserName, Type);
                        }
                        else
                        {
                            _Output["records"] = VideoBLL.Count(GalleryID, isApproved, Status, Type);
                        }
                                               
                        context.Response.Write(_Output);

                        break;

                    case "update_video":

                        var _data = JsonConvert.DeserializeObject<Video_Struct>(json);

                        VideoBLL.Update_Video(_data.VideoID, _data.CategoryID, _data.Title, _data.Description, _data.Tags, _data.isPrivate);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "update_unpublished_video_info":
                        
                        VideoBLL.Update_Unpublished_Video_Info(JsonConvert.DeserializeObject<Video_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "update_selected_thumbs":

                        var _sdata = JsonConvert.DeserializeObject<Video_Struct>(json);

                        VideoBLL.Update_Selected_Thumb(_sdata.VideoID, _sdata.ThumbFileName, _sdata.Thumb_Url);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "delete":

                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        var _delete_data = JsonConvert.DeserializeObject<Video_Struct>(json);
                        if(_delete_data.UserName != "")
                        {
                            VideoBLL.Delete(_delete_data.VideoID,_delete_data.UserName,_delete_data.Type);
                        }
                        else
                        {
                            VideoBLL.Delete(_delete_data.VideoID);
                        }
                        

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

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

                        var _upd_isenabled = JsonConvert.DeserializeObject<Video_Struct>(json);

                        VideoBLL.Update_IsEnabled(_upd_isenabled.VideoID,OldValue, NewValue, _upd_isenabled.UserName, _upd_isenabled.Type);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "update_isprivate":

                        if (context.Request.Params["oval"] != null)
                        {
                            OldValue = Convert.ToInt32(context.Request.Params["oval"]);
                        }
                        if (context.Request.Params["nval"] != null)
                        {
                            NewValue = Convert.ToInt32(context.Request.Params["nval"]);
                        }

                        var _upd_isprivate= JsonConvert.DeserializeObject<Video_Struct>(json);

                        VideoBLL.Update_IsPrivate(_upd_isprivate.VideoID, OldValue, NewValue, _upd_isprivate.UserName, _upd_isprivate.Type);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "Update_IsPublished":

                        if (context.Request.Params["val"] != null)
                        {
                            NewValue = Convert.ToInt32(context.Request.Params["val"]);
                        }
                      

                        var _upd_ispublished = JsonConvert.DeserializeObject<Video_Struct>(json);

                        VideoBLL.Update_IsPublished(_upd_ispublished.VideoID, NewValue,  _upd_ispublished.UserName, _upd_ispublished.Type);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "Update_IsReviewed":

                        if (context.Request.Params["oval"] != null)
                        {
                            OldValue = Convert.ToInt32(context.Request.Params["oval"]);
                        }
                        if (context.Request.Params["nval"] != null)
                        {
                            NewValue = Convert.ToInt32(context.Request.Params["nval"]);
                        }

                        var _upd_isreviewed = JsonConvert.DeserializeObject<Video_Struct>(json);

                        VideoBLL.Update_IsReviewed(_upd_isreviewed.VideoID, OldValue, NewValue, _upd_isreviewed.UserName, _upd_isreviewed.Type);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "review_video":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt64(context.Request.Params["vid"]);
                        }

                        var _videobb = new VideoBLL();
                        _videobb.Review_Video(VideoID);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "update_field":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["val"] != null)
                        {
                            Value = context.Request.Params["val"].ToString();
                        }
                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }
                       
                        VideoBLL.Update_Field(VideoID, Value,FieldName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "get_field_value":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                       
                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }

                       

                        responseMsg["value"] = VideoBLL.Get_Field_Value(VideoID, FieldName);

                        context.Response.Write(responseMsg);
                        break;

                    case "post_rating":
                        int CurrentRating = 0;
                        if (context.Request.Params["crating"] != null)
                        {
                            CurrentRating = Convert.ToInt32(context.Request.Params["crating"]);
                        }
                        var _post_rating = JsonConvert.DeserializeObject<Video_Struct>(json);

                        VideoBLL.Post_Rating(_post_rating.VideoID, CurrentRating, _post_rating.Total_Rating, _post_rating.Ratings);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "validate_videoid":
                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        var _validation_Output = new Dictionary<string, bool>();

                        _validation_Output["isvalid"] = VideoBLL.Validate_VideoID(VideoID);
                        context.Response.Write(_validation_Output);

                        break;

                    case "load_videos":

                        var _ld_video_json = JsonConvert.DeserializeObject<Video_Struct>(json);
                        var _vObject = new VideoObject()
                        {
                            Data = _videoobj.Load_Videos_V4(_ld_video_json),
                            Count = _videoobj.Cache_Count_Videos_V4(_ld_video_json)
                        };

                        _ld_video_data["data"] = _vObject;

                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_videos_limit":
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = _videoobj.Load_Videos_V4(JsonConvert.DeserializeObject<Video_Struct>(json)),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_record":
                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = _videoobj.Fetch_Record_v2(VideoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_record_sm":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = _videoobj.Fetch_Record_SM(VideoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "get_information":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = _videoobj.Get_Information(VideoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "get_sm_information":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = VideoBLL.Get_SM_Info(VideoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_recent_videos":

                        if (context.Request.Params["records"] != null)
                        {
                            Records = Convert.ToInt32(context.Request.Params["records"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = VideoBLL.Fetch_Rec_Videos(Records),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "get_basic_video_information":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = VideoBLL.Load_Video_Basic_Information(VideoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "get_download_file_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = VideoBLL.Get_Download_File_Info(VideoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_tags":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = VideoBLL.FetchTags(VideoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_video_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = VideoBLL.Fetch_Video_Info(VideoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_finalize_video":

                        if (context.Request.Params["gid"] != null)
                        {
                            GalleryID = Convert.ToInt64(context.Request.Params["gid"]);
                        }
                        if (context.Request.Params["records"] != null)
                        {
                            Records = Convert.ToInt32(context.Request.Params["records"]);
                        }
                        if (context.Request.Params["username"] != null)
                        {
                            UserName = context.Request.Params["username"].ToString();
                        }
                        long MaxVideoID = 0;
                        if (context.Request.Params["maxid"] != null)
                        {
                            MaxVideoID = Convert.ToInt64(context.Request.Params["maxid"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = VideoBLL.Fetch_Finalized_Videos(GalleryID, Records, UserName, MaxVideoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_video_data":

                        if (context.Request.Params["vid"] != null)
                        {
                            VideoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = VideoBLL.Fetch_Video_Data(VideoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_unpublished_videos":

                        if (context.Request.Params["records"] != null)
                        {
                            Records = Convert.ToInt32(context.Request.Params["records"]);
                        }
                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = VideoBLL.Fetch_UnPublished_Videos(Records,Type),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "related_videos":

                        var _related_data = JsonConvert.DeserializeObject<Video_Struct>(json);
                        _ld_video_data["data"] = new VideoObject()
                        {
                            Data = VideoBLL.Load_Related_Videos(_related_data.Title, _related_data.Tags,_related_data.VideoID,_related_data.Type,_related_data.GalleryID),
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
                        _archive["data"] = VideoBLL.Load_Arch_List(Type, Records, isAll);
                        context.Response.Write(_archive);

                        break;

                    case "max_videoid":

                        if (context.Request.Params["gid"] != null)
                        {
                            GalleryID = Convert.ToInt64(context.Request.Params["gid"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }

                        var _max_vid = new Dictionary<string, long>();
                        if(UserName != "")
                        {
                            _max_vid["data"] = VideoBLL.MaxVideoID(UserName, Type);
                        }
                       else
                        {
                            _max_vid["data"] = VideoBLL.MaxVideoID(GalleryID);
                        }
                        context.Response.Write(_max_vid);

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

    public class VideoObject
    {
        public List<Video_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}
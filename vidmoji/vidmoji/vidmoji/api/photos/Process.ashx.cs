using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.photos
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

            long PhotoID = 0;
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
            var _videoobj = new PhotosBLLC();
            var _ld_video_data = new Dictionary<string, PhotosObject>();

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

                        PhotosBLLC.Add(JsonConvert.DeserializeObject<Photos_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                   
                    case "check":

                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt64(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (PhotosBLLC.Check(PhotoID, UserName))
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

                        PhotosBLLC.Update(JsonConvert.DeserializeObject<Photos_Struct>(json));

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

                        PhotosBLLC.Update_Myaccount(JsonConvert.DeserializeObject<Photos_Struct>(json));

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

                        PhotosBLLC.Update_Info(JsonConvert.DeserializeObject<Photos_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "remove_photo":

                        var _rem_photo = JsonConvert.DeserializeObject<Photos_Struct>(json);

                        PhotosBLLC.RemovePhoto(_rem_photo.ImageID,_rem_photo.FileName, _rem_photo.UserName, _rem_photo.isCloud);

                        break;

                    case "delete_photo":

                        var _del_photo = JsonConvert.DeserializeObject<Photos_Struct>(json);

                        PhotosBLLC.DeletePhoto(_del_photo.ImageID, _del_photo.UserName);

                        break;

                    case "reset_next_prev_id":

                        if (context.Request.Params["id"] != null)
                        {
                            PhotoID = Convert.ToInt64(context.Request.Params["id"]);
                        }
                        PhotosBLLC.Reset_Next_Prev_ID(PhotoID);

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
                        PhotosBLLC.Update_isApproved_All(GalleryID, UserName, Status);

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
                        PhotosBLLC.Update_isEnabled_All(GalleryID, UserName, Status);

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
                            _Output["records"] = PhotosBLLC.Count(UserName,Status, isApproved);
                        }
                        else
                        {
                            _Output["records"] = PhotosBLLC.Count(GalleryID, isApproved, Status);
                        }

                        context.Response.Write(_Output);

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

                        var _upd_isenabled = JsonConvert.DeserializeObject<Photos_Struct>(json);

                        PhotosBLLC.Update_IsEnabled(_upd_isenabled.ImageID, OldValue, NewValue, _upd_isenabled.UserName);

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

                        var _upd_isreviewed = JsonConvert.DeserializeObject<Photos_Struct>(json);

                        PhotosBLLC.Update_IsApproved(_upd_isreviewed.ImageID, OldValue, NewValue, _upd_isreviewed.UserName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                   
                    case "update_field":

                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["val"] != null)
                        {
                            Value = context.Request.Params["val"].ToString();
                        }
                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }

                        PhotosBLLC.Update_Field(PhotoID, Value, FieldName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "get_field_value":

                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }



                        responseMsg["value"] = PhotosBLLC.Get_Field_Value(PhotoID, FieldName);

                        context.Response.Write(responseMsg);
                        break;

                    case "post_rating":

                        int CurrentRating = 0;
                        if (context.Request.Params["crating"] != null)
                        {
                            CurrentRating = Convert.ToInt32(context.Request.Params["crating"]);
                        }
                        var _post_rating = JsonConvert.DeserializeObject<Photos_Struct>(json);

                        PhotosBLLC.Post_Rating(_post_rating.ImageID, CurrentRating, _post_rating.Total_Rating, _post_rating.Rating);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "load_photos":

                        var _ld_video_json = JsonConvert.DeserializeObject<Photos_Struct>(json);
                        var _vObject = new PhotosObject()
                        {
                            Data = _videoobj.Load_Photos_V4(_ld_video_json),
                            Count = _videoobj.Cache_Count_Photos_V3(_ld_video_json)
                        };

                        _ld_video_data["data"] = _vObject;

                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_photos_limit":
                        _ld_video_data["data"] = new PhotosObject()
                        {
                            Data = _videoobj.Load_Photos_Limit(JsonConvert.DeserializeObject<Photos_Struct>(json)),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_record":
                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new PhotosObject()
                        {
                            Data = PhotosBLLC.Fetch_Photo(PhotoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_photo_myaccount":

                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new PhotosObject()
                        {
                            Data = PhotosBLLC.Fetch_Photo_MyAccount(PhotoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_photo_gallery":

                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new PhotosObject()
                        {
                            Data = PhotosBLLC.Fetch_Photo_Gallery(PhotoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_photo_admin":

                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        _ld_video_data["data"] = new PhotosObject()
                        {
                            Data = PhotosBLLC.Fetch_Photo_Admin(PhotoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_photo_sm":

                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new PhotosObject()
                        {
                            Data = PhotosBLLC.Fetch_Photo_SM(PhotoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "get_sm_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new PhotosObject()
                        {
                            Data = PhotosBLLC.Get_SM_Info(PhotoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "get_photo_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new PhotosObject()
                        {
                            Data = PhotosBLLC.Get_Photo_Info(PhotoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_tags":

                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new PhotosObject()
                        {
                            Data = PhotosBLLC.FetchTags(PhotoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_photo_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            PhotoID = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new PhotosObject()
                        {
                            Data = PhotosBLLC.Fetch_Photo_Info(PhotoID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_finalize_photos":

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
                        long MaxID = 0;
                        if (context.Request.Params["maxid"] != null)
                        {
                            MaxID = Convert.ToInt64(context.Request.Params["maxid"]);
                        }
                        _ld_video_data["data"] = new PhotosObject()
                        {
                            Data = PhotosBLLC.Fetch_Finalized_Photos(GalleryID, Records, UserName, MaxID),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_gallery_photos":

                        if (context.Request.Params["gid"] != null)
                        {
                            GalleryID = Convert.ToInt32(context.Request.Params["gid"]);
                        }
                        if (context.Request.Params["records"] != null)
                        {
                            Records = Convert.ToInt32(context.Request.Params["records"]);
                        }
                        string Order = "";
                        if (context.Request.Params["o"] != null)
                        {
                            Order = context.Request.Params["o"].ToString();
                        }

                        if(Order != "")
                        {
                            _ld_video_data["data"] = new PhotosObject()
                            {
                                Data = PhotosBLLC.Fetch_Gallery_Photos(GalleryID, Records, Order),
                                Count = 0
                            };
                        }
                        else
                        {
                            _ld_video_data["data"] = new PhotosObject()
                            {
                                Data = PhotosBLLC.Fetch_Gallery_Photos(GalleryID, Records),
                                Count = 0
                            };
                        }
                       
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
                        _archive["data"] = PhotosBLLC.Load_Arch_List(Records, isAll);
                        context.Response.Write(_archive);

                        break;

                    case "fetch_id_v2":

                        if (context.Request.Params["gid"] != null)
                        {
                            GalleryID = Convert.ToInt64(context.Request.Params["gid"]);
                        }
                        if (context.Request.Params["id"] != null)
                        {
                            PhotoID = Convert.ToInt64(context.Request.Params["id"]);
                        }
                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }

                        var _max_vid = new Dictionary<string, long>();
                        _max_vid["data"] = PhotosBLLC.Fetch_ID_V2(PhotoID,GalleryID, Type);
                        context.Response.Write(_max_vid);

                        break;
                    case "fetch_id_v3":
                        string Categories = "";
                        if (context.Request.Params["cat"] != null)
                        {
                            Categories = context.Request.Params["cat"].ToString();
                        }
                        if (context.Request.Params["id"] != null)
                        {
                            PhotoID = Convert.ToInt64(context.Request.Params["id"]);
                        }
                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }

                        var _max_pid = new Dictionary<string, long>();
                        _max_pid["data"] = PhotosBLLC.Fetch_ID_V3(PhotoID, Categories, Type);
                        context.Response.Write(_max_pid);

                        break;
                  
                    case "max_PhotoID":

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

                        var _max_phid = new Dictionary<string, long>();
                        if (UserName != "")
                        {
                            _max_phid["data"] = PhotosBLLC.MaxPhotoID(UserName);
                        }
                        else
                        {
                            _max_phid["data"] = PhotosBLLC.MaxPhotoID(GalleryID);
                        }
                        context.Response.Write(_max_phid);

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

    public class PhotosObject
    {
        public List<Photos_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}
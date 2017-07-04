using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.qa
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

            long Qid = 0;
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

            var _videoobj = new QABLL();
            var _ld_video_data = new Dictionary<string, QAObject>();

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
                        QABLL.Add(JsonConvert.DeserializeObject<QA_Struct>(json), isAdmin);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;


                    case "check":

                        if (context.Request.Params["vid"] != null)
                        {
                            Qid = Convert.ToInt64(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (QABLL.Check(Qid, UserName))
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

                        QABLL.Update(JsonConvert.DeserializeObject<QA_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                   
                    case "delete":

                        var _rem_photo = JsonConvert.DeserializeObject<QA_Struct>(json);

                        QABLL.Delete(_rem_photo.Qid, _rem_photo.UserName);

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
                        _Output["records"] = QABLL.Count(UserName, Status, isApproved);

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

                        var _upd_isenabled = JsonConvert.DeserializeObject<QA_Struct>(json);

                        QABLL.Update_IsEnabled(_upd_isenabled.Qid, OldValue, NewValue, _upd_isenabled.UserName);

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

                        var _upd_isreviewed = JsonConvert.DeserializeObject<QA_Struct>(json);

                        QABLL.Update_IsApproved(_upd_isreviewed.Qid, OldValue, NewValue, _upd_isreviewed.UserName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;


                    case "update_field":

                        if (context.Request.Params["vid"] != null)
                        {
                            Qid = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["val"] != null)
                        {
                            Value = context.Request.Params["val"].ToString();
                        }
                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }

                        QABLL.Update_Field(Qid, Value, FieldName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "get_field_value":

                        if (context.Request.Params["vid"] != null)
                        {
                            Qid = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }



                        responseMsg["value"] = QABLL.Get_Field_Value(Qid, FieldName);

                        context.Response.Write(responseMsg);
                        break;

                  
                    case "load_qa":

                        var _ld_video_json = JsonConvert.DeserializeObject<QA_Struct>(json);
                        var _vObject = new QAObject()
                        {
                            Data = QABLL.Load_QA_V4(_ld_video_json),
                            Count = QABLL.Count_qa_V3(_ld_video_json)
                        };

                        _ld_video_data["data"] = _vObject;

                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_photos_limit":
                        _ld_video_data["data"] = new QAObject()
                        {
                            Data = QABLL.Load_qa_Limit(JsonConvert.DeserializeObject<QA_Struct>(json)),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_record":
                        if (context.Request.Params["vid"] != null)
                        {
                            Qid = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new QAObject()
                        {
                            Data = QABLL.Fetch_QA(Qid),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_qa_admin":

                        if (context.Request.Params["vid"] != null)
                        {
                            Qid = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new QAObject()
                        {
                            Data = QABLL.Fetch_QA_Admin(Qid),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_qa_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            Qid = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new QAObject()
                        {
                            Data = QABLL.Fetch_QA_Info(Qid),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_edit_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            Qid = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        _ld_video_data["data"] = new QAObject()
                        {
                            Data = QABLL.Fetch_Edit_Info(Qid),
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
                        _archive["data"] = QABLL.Load_Arch_List(Records, isAll);
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

    public class QAObject
    {
        public List<QA_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.qa
{
    /// <summary>
    /// Summary description for Answers
    /// </summary>
    public class Answers : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = new StreamReader(context.Request.InputStream).ReadToEnd();
            var responseMsg = new Dictionary<string, string>();

            long Qid = 0;
            long Aid = 0;
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
            int Answers = 0;
            var _videoobj = new QAnswersBLL();
            var _ld_video_data = new Dictionary<string, QAnswerObject>();

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
                        if (context.Request.Params["answers"] != null)
                        {
                            Answers = Convert.ToInt32(context.Request.Params["answers"]);
                        }
                        QAnswersBLL.Add(JsonConvert.DeserializeObject<QAnswers_Struct>(json), Answers);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "checkbestanswer":
                        if (context.Request.Params["qid"] != null)
                        {
                            Qid = Convert.ToInt64(context.Request.Params["qid"]);
                        }

                        if (QAnswersBLL.CheckBestAnswer(Qid))
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

                    case "check":
                        if (context.Request.Params["qid"] != null)
                        {
                            Qid = Convert.ToInt64(context.Request.Params["qid"]);
                        }
                        if (context.Request.Params["vid"] != null)
                        {
                            Aid = Convert.ToInt64(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if(Qid > 0)
                        {
                            if (QAnswersBLL.Check(Aid, Qid, UserName))
                            {
                                responseMsg["status"] = "success";
                                responseMsg["message"] = "Validated";
                            }
                            else
                            {
                                responseMsg["status"] = "error";
                                responseMsg["message"] = "Not Validated";
                            }
                        }
                        else
                        {
                            if (QAnswersBLL.Check(Aid, UserName))
                            {
                                responseMsg["status"] = "success";
                                responseMsg["message"] = "Validated";
                            }
                            else
                            {
                                responseMsg["status"] = "error";
                                responseMsg["message"] = "Not Validated";
                            }
                        }
                        context.Response.Write(responseMsg);
                        break;


                    case "update":

                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        QAnswersBLL.Update(JsonConvert.DeserializeObject<QAnswers_Struct>(json));

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "update_all_qa_ans_member_stats":

                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }
                        if (context.Request.Params["qid"] != null)
                        {
                            Qid = Convert.ToInt64(context.Request.Params["qid"]);
                        }
                        QAnswersBLL.Update_All_QA_Ans_Member_Stats(Qid);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;
                        
                    case "reset_bestanswer":

                        var _rem_photo = JsonConvert.DeserializeObject<QAnswers_Struct>(json);

                        QAnswersBLL.Reset_BestAnswers(_rem_photo.Qid);

                        break;

                    case "delete":

                        var _del = JsonConvert.DeserializeObject<QAnswers_Struct>(json);
                        if (_del.UserName != "")
                            QAnswersBLL.Delete(_del.Aid);
                        else
                            QAnswersBLL.Delete(_del.Aid, _del.UserName);

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
                      
                        var _Output = new Dictionary<string, int>();
                        _Output["records"] = QAnswersBLL.Count(UserName, Status, isApproved);

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

                        var _upd_isenabled = JsonConvert.DeserializeObject<QAnswers_Struct>(json);

                        QAnswersBLL.Update_IsEnabled(_upd_isenabled.Aid, OldValue, NewValue, _upd_isenabled.UserName);

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

                        var _upd_isreviewed = JsonConvert.DeserializeObject<QAnswers_Struct>(json);

                        QAnswersBLL.Update_IsApproved(_upd_isreviewed.Aid, OldValue, NewValue, _upd_isreviewed.UserName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;


                    case "update_field":

                        if (context.Request.Params["vid"] != null)
                        {
                            Aid = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        if (context.Request.Params["val"] != null)
                        {
                            Value = context.Request.Params["val"].ToString();
                        }
                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }

                        QAnswersBLL.Update_Field(Aid, Value, FieldName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "get_field_value":

                        if (context.Request.Params["vid"] != null)
                        {
                            Aid = Convert.ToInt32(context.Request.Params["vid"]);
                        }

                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }



                        responseMsg["value"] = QAnswersBLL.Get_Field_Value(Aid, FieldName);

                        context.Response.Write(responseMsg);
                        break;
                  
                    case "fetch_answers":
                        if (context.Request.Params["vid"] != null)
                        {
                            Qid = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new QAnswerObject()
                        {
                            Data = QAnswersBLL.Fetch_Answers(Qid),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_answers_admin":

                        if (context.Request.Params["vid"] != null)
                        {
                            Qid = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new QAnswerObject()
                        {
                            Data = QAnswersBLL.Fetch_Answers_Admin(Qid),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_qa_users":

                        if (context.Request.Params["vid"] != null)
                        {
                            Qid = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new QAnswerObject()
                        {
                            Data = QAnswersBLL.Fetch_QA_Users(Qid),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_answers_info":

                        if (context.Request.Params["vid"] != null)
                        {
                            Aid = Convert.ToInt32(context.Request.Params["vid"]);
                        }
                        _ld_video_data["data"] = new QAnswerObject()
                        {
                            Data = QAnswersBLL.Fetch_Answer_Info(Qid),
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

    public class QAnswerObject
    {
        public List<QAnswers_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}
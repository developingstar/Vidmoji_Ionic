using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.friend
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


            bool isUpdate = false;
            int Records = 0;
            string Keywords = "";
            string Text = "";
            string Order = "";
            int PageNumber = 0;
            int PageSize = 0;

            var _friendobj = new FriendsBLL();
            var _friends = JsonConvert.DeserializeObject<Friends_Struct>(json);

            var _ld_dictionary_data = new Dictionary<string, FriendObject>();

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

                        FriendsBLL.Add(_friends.UserName, _friends.Friend_UserName,_friends.Status,_friends.Message,_friends.Val_Key,_friends.Reject_Key);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "update_status":

                        FriendsBLL.Update_Status(_friends.UserName,_friends.Friend_UserName,_friends.Status);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "delete":

                        FriendsBLL.Delete(_friends.UserName,_friends.Friend_UserName);
                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "isfriend":

                        FriendsBLL.isFriend(_friends.UserName, _friends.Friend_UserName,_friends.Status);
                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "fetch_user_friends_sm":

                        FriendsBLL.Fetch_User_Friends_SM(_friends.UserName);
                        responseMsg["status"] = "success";
                        context.Response.Write(responseMsg);

                        break;
                    case "refreshstats":

                        FriendsBLL.RefreshStats(_friends.UserName);
                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;
                    case "fetch_user_friends":
                        if (context.Request.Params["pagenumber"] != null)
                        {
                            PageNumber = Convert.ToInt32(context.Request.Params["pagenumber"]);
                        }
                        if (context.Request.Params["pagesize"] != null)
                        {
                            PageSize = Convert.ToInt32(context.Request.Params["pagesize"]);
                        }

                        FriendsBLL.Fetch_User_Friends(_friends.UserName, PageNumber, PageSize);
                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;                   
                }
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

    public class FriendObject
    {
        public List<Friends_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}
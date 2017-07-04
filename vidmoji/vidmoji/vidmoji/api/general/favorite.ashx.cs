using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.general
{
    /// <summary>
    /// Summary description for favorites
    /// </summary>
    public class favorite : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = new StreamReader(context.Request.InputStream).ReadToEnd();
            var responseMsg = new Dictionary<string, string>();

            long ContentID = 0;
            int Type = 0;
            int MediaType = 0;
            string UserName = "";
            int Status = 0;
            int isApproved = 0;
            int OldValue = 0;
            int NewValue = 0;
            string Value = "";
            string FieldName = "";
            int Records = 0;
            bool isAdmin = false;

           
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
                        if (context.Request.Params["cid"] != null)
                        {
                            ContentID = Convert.ToInt64(context.Request.Params["cid"]);
                        }
                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }
                        if (context.Request.Params["mtype"] != null)
                        {
                            MediaType = Convert.ToInt32(context.Request.Params["mtype"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        Favorites.Add(UserName, ContentID, MediaType, Type);
                    
                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;


                    case "check":

                        if (context.Request.Params["cid"] != null)
                        {
                            ContentID = Convert.ToInt64(context.Request.Params["cid"]);
                        }
                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }
                       
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (Favorites.Check_Favorites(UserName, ContentID, Type))
                        {
                            responseMsg["status"] = "success";
                            responseMsg["message"] = "Validated";
                        }
                        else
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Not Validated";
                        }
                        break;



                    // This update is only for pubplishing pending videos (unpublished videos only)
                    case "delete":

                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        if (context.Request.Params["cid"] != null)
                        {
                            ContentID = Convert.ToInt64(context.Request.Params["cid"]);
                        }
                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }
                        if (context.Request.Params["mtype"] != null)
                        {
                            MediaType = Convert.ToInt32(context.Request.Params["mtype"]);
                        }
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }

                        Favorites.Delete(ContentID,UserName, MediaType, Type);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
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
}
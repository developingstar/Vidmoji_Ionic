using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.Configuration
{
    /// <summary>
    /// Summary description for Process
    /// </summary>
    public class Process : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            
            var json = new StreamReader(context.Request.InputStream).ReadToEnd();
            var responseMsg = new Dictionary<string, string>(); var _ld_post_data = new Dictionary<string, CategoryObject>();

            if ((context.Request.Params["action"] != null))
            {
                switch (context.Request.Params["action"])
                {
                    
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
                        var _data = JsonConvert.DeserializeObject<Category_Struct>(json);
                        ConfigurationBLL.Update_Value(_data.ID,_data.Name);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "return_value":
                        int ID = 0;
                        if (context.Request.Params["id"] != null)
                        {
                            ID = Convert.ToInt32(context.Request.Params["id"]);
                        }
                      
                        responseMsg["value"] = ConfigurationBLL.Return_Value(ID);
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
    public class CategoryObject
    {
        public List<Category_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}
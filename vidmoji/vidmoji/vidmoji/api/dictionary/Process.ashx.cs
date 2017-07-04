using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.dictionary
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
            int PageNumnber = 0;
            var _dictionaryobj = new DictionaryBLL();
            var _ld_dictionary = JsonConvert.DeserializeObject<Dictionary_Struct>(json);

            var _ld_dictionary_data = new Dictionary<string, DictionaryObject>();

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

                        DictionaryBLL.Add(_ld_dictionary.Value, _ld_dictionary.Type);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "delete":                       
                        
                        DictionaryBLL.Delete(_ld_dictionary.ID);
                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;

                    case "load":
                        
                        DictionaryBLL.Load(_ld_dictionary.Type);
                        responseMsg["status"] = "success";
                        context.Response.Write(responseMsg);

                        break;

                    case "return_value":

                        DictionaryBLL.Return_Values(_ld_dictionary.Type);
                        responseMsg["status"] = "success";
                        context.Response.Write(responseMsg);

                        break;

                    case "process_screening":
                        if (context.Request.Params["text"] != null)
                        {
                            Text = context.Request.Params["text"].ToString();
                        }
                        DictionaryBLL.Process_Screening(Text);
                        responseMsg["status"] = "success";
                        context.Response.Write(responseMsg);

                        break;

                    case "is_match":
                        if (context.Request.Params["text"] != null)
                        {
                            Text = context.Request.Params["text"].ToString();
                        }
                        if (context.Request.Params["keywords"] != null)
                        {
                            Keywords = context.Request.Params["keywords"].ToString();
                        }
                        DictionaryBLL.isMatch(Text,Keywords);
                        responseMsg["status"] = "success";
                        context.Response.Write(responseMsg);

                        break;

                    case "validate_search_word":
                        if (context.Request.Params["text"] != null)
                        {
                            Text = context.Request.Params["text"].ToString();
                        }
                        DictionaryBLL.Validate_Search_Word(Text);
                        responseMsg["status"] = "success";
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
    public class DictionaryObject
    {
        public List<Dictionary_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}


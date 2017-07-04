using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace vidmoji.api.general
{
    /// <summary>
    /// Summary description for tag
    /// </summary>
    public class tag : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = new StreamReader(context.Request.InputStream).ReadToEnd();
            var responseMsg = new Dictionary<string, string>();

            long ContentID = 0;
            int tagType = 0;
            int Type = 0;
            int PageNumber = 1;
            string Order = "tagname asc";
            bool isCache = true;

            int Status = 0;
            string Value = "";
            string FieldName = "";
            int Records = 0;
           
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
                        var _addobj = JsonConvert.DeserializeObject<Tags_Struct>(json);
                     
                        if (context.Request.Params["ttype"] != null)
                        {
                            tagType = Convert.ToInt32(context.Request.Params["ttype"]);
                        }
                        if (context.Request.Params["cid"] != null)
                        {
                            ContentID = Convert.ToInt64(context.Request.Params["cid"]);
                        }

                        TagsBLL.Process_Tags(_addobj.TagName,_addobj.Type,tagType, ContentID);
                        
                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;


                    case "delete":

                        var _rem_forum = JsonConvert.DeserializeObject<Tags_Struct>(json);

                        TagsBLL.Delete(_rem_forum.TagID);

                        break;
                   

                    case "load_tags":
                        tagType = 0;
                        
                        if (context.Request.Params["ttype"] != null)
                        {
                            tagType = Convert.ToInt32(context.Request.Params["ttype"]);
                        }
                        if (context.Request.Params["records"] != null)
                        {
                            Records = Convert.ToInt32(context.Request.Params["records"]);
                        }
                        if (context.Request.Params["p"] != null)
                        {
                            PageNumber = Convert.ToInt32(context.Request.Params["p"]);
                        }
                        if (context.Request.Params["o"] != null)
                        {
                            Order = context.Request.Params["o"].ToString();
                        }
                        if (context.Request.Params["iscache"] != null)
                        {
                            isCache = Convert.ToBoolean(context.Request.Params["iscache"]);
                        }
                        var _ld_json = JsonConvert.DeserializeObject<Tags_Struct>(json);
                        _ld_json.Tag_Level = 100;
                        var _vObject = new TagsObject()
                        {
                            Data = TagsBLL.Cache_FetchRecentTags_V2(_ld_json.Term, _ld_json.Type, tagType, _ld_json.Tag_Level,1,Records,PageNumber, Order, isCache),
                            Count = TagsBLL.CountRecentTags_V2(_ld_json.Term,_ld_json.Type, tagType,_ld_json.Tag_Level,1)
                        };

                        var _ld_tag_data = new Dictionary<string, TagsObject>();

                        _ld_tag_data["data"] = _vObject;

                        context.Response.Write(_ld_tag_data);

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

    public class TagsObject
    {
        public List<Tags_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}
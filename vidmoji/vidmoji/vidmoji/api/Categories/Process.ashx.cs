using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace vidmoji.api.Categories
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
            int AssignCategoryID = 0;
            string TableName = "";
            string Search = "";
            string Order = "";
            int PageNumnber = 0;
            var _categoryobj = new CategoriesBLL();
            var _ld_categories_json = JsonConvert.DeserializeObject<Category_Struct_V2>(json);

            var _ld_post_data = new Dictionary<string, CategoryObject>();

            if ((context.Request.Params["action"] != null))
            {
                switch (context.Request.Params["action"])
                {
                    case "process":
                        
                        var cat= JsonConvert.DeserializeObject<Category_Struct_V2>(json);
                        
                        isUpdate = false;
                        if (context.Request.Params["isupdate"] != null)
                        {
                            isUpdate = Convert.ToBoolean(context.Request.Params["isupdate"]);
                            isUpdate = true;
                        }
                        
                        if (isUpdate)
                        {
                            CategoriesBLL.Process(cat,isUpdate);
                        }    
                       
                        context.Response.Write(responseMsg);
                        break;

                    case "delete":
                        var _delcategory = JsonConvert.DeserializeObject<Category_Struct>(json);
                        if (context.Request.Params["assigncategoryid"] != null)
                        {
                            AssignCategoryID = Convert.ToInt32(context.Request.Params["assigncategoryid"]);
                            CategoriesBLL.Delete(_delcategory.ID, AssignCategoryID);
                        }
                        else
                        {
                            CategoriesBLL.Delete(_delcategory.ID);
                        }

                        break;

                    // This update is only for pubplishing pending videos (unpublished videos only)
                    case "update_category":

                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }
                        if (context.Request.Params["assigncategoryid"] != null)
                        {
                            AssignCategoryID = Convert.ToInt32(context.Request.Params["assigncategoryid"]);                            
                        }
                        if (context.Request.Params["tablename"] != null)
                        {
                            TableName = context.Request.Params["tablename"].ToString();
                        }


                        CategoriesBLL.Update_Category(JsonConvert.DeserializeObject<Category_Struct_V2>(json).CategoryID,AssignCategoryID,TableName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                   
                    case "load_categories":
                       
                        bool isAll = false;
                        if (context.Request.Params["isall"] != null)
                        {
                            isAll = Convert.ToBoolean(context.Request.Params["isall"]);
                        }
                        var _query = JsonConvert.DeserializeObject<Category_Struct_V2>(json);

                        var _lst_sm = CategoriesBLL.Load_CategoriesSummary(_query.CategoryID, _query.Type,isAll, _query.isPrivate, _query.Mode, "categoryname asc", _query.Records, 1);

                        var _vObject = new CategoryObject()
                        {
                            Data = _lst_sm,
                            Count = CategoriesBLL.Count_CategoriesSummary(_query.Type, _query.isPrivate, _query.Mode)
                        };
                        
                        context.Response.Write(_vObject);

                        break;

                    case "fetch_category_names":

                        CategoriesBLL.Fetch_Category_Names(_ld_categories_json.Type);
                        responseMsg["status"] = "success";
                        //context.Response.Write(responseMsg);
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
        public List<Category_Struct_V2> Data { get; set; }
        public int Count { get; set; }
    }
}
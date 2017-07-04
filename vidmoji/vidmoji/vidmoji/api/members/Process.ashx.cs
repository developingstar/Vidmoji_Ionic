using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace vidmoji.api.member
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

            int Type = 0;
            string UserName = "";
            string Email = "";
            int Status = 0;
       
            int OldValue = 0;
            int NewValue = 0;
            string Value = "";
            string FieldName = "";
            int Records = 0;
            string Key = "";
            bool isAdmin = false;
            var _mem = new members();
            var _ld_video_data = new Dictionary<string, MembersObject>();

            if ((context.Request.Params["action"] != null))
            {
                switch (context.Request.Params["action"])
                {   
                    // url/api/members/process.ashx?action=login
                    // data
                    case "login":
                        var _login_member = JsonConvert.DeserializeObject<Member_Struct>(json);
                        // validate member
                        // Update Password Validation Script
                        if (_login_member.UserName == "" || _login_member.Password == "")
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Please enter username and password";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        int MemberType = 0;
                        int Readonly = 0;
                        List<Member_Struct> _lst = members.Get_Hash_Password(_login_member.UserName);
                        if (_lst.Count == 0)
                        {
                            // No user account found based on username search
                            responseMsg["status"] = "error";
                            responseMsg["message"] = Resources.vsk.message_06;
                            context.Response.Write(responseMsg);
                            return;
                        }

                        // check encrypted password
                        if (_lst[0].Password.Length < 20)
                        {
                            // backward compatibility
                            // check existing user passwords with old system
                            if (!_mem.Validate_Member(_login_member.UserName, _login_member.Password, false))
                            {
                                responseMsg["status"] = "error";
                                responseMsg["message"] = Resources.vsk.message_06;
                                context.Response.Write(responseMsg);
                                return;
                            }
                            MemberType = Convert.ToInt32(members.Return_Value(_login_member.UserName, "type"));
                            Readonly = Convert.ToInt32(members.Return_Value(_login_member.Password, "readonly"));
                        }
                        else
                        {
                            // check encrypted password with user typed password
                            bool matched = BCrypt.Net.BCrypt.Verify(_login_member.Password, _lst[0].Password);
                            if (!matched)
                            {
                                responseMsg["status"] = "error";
                                responseMsg["message"] = Resources.vsk.message_06;
                                context.Response.Write(responseMsg);
                                return;
                            }
                            MemberType = _lst[0].Type; // type
                            Readonly = _lst[0].ReadOnly;
                        }

                        string Role = "User";
                        switch (MemberType)
                        {
                            case 0:
                                Role = "User";
                                break;
                            case 1:
                                Role = "Admin";
                                break;
                            case 2:
                                Role = "PaidUser";
                                break;
                        }

                        if (MemberType == 1)
                        {
                            if (Readonly == 1)
                                Role = "ReadOnlyAdmin";
                        }
                        // IP Address tracking and processing
                        string ipaddress = context.Request.ServerVariables["REMOTE_ADDR"].ToString();
                        if (BlockIPBLL.Validate_IP(ipaddress))
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "IP Blocked";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        if (Site_Settings.Store_IPAddress)
                        {
                            // Store IP Address Log 
                            User_IPLogBLL.Process_Ipaddress_Log(_login_member.UserName, ipaddress);
                        }

                        // Update Last Login Activity of User
                        members.Update_Value(_login_member.UserName, "last_login", DateTime.Now);
                        // member is validated
                        FormsAuthenticationTicket _ticket = new FormsAuthenticationTicket(1, _login_member.UserName, DateTime.Now, DateTime.Now.AddMonths(1), true, Role, FormsAuthentication.FormsCookiePath);
                        string encTicket = FormsAuthentication.Encrypt(_ticket);
                        HttpCookie _cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                        //  if (chk_remember.Checked)
                        //    _cookie.Expires = DateTime.Now.AddMonths(1);
                        // Response.Cookies.Add(_cookie);

                        // check for membership upgrades

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Login Successfull";
                        responseMsg["role"] = Role;

                        context.Response.Write(responseMsg);
                        return;
                    // url/api/members/process.ashx?action=register
                    // data
                    case "register":
                        var _register_member = JsonConvert.DeserializeObject<Member_Struct>(json);

                        string res_values = DictionaryBLL.Return_RestrictedUserNames();
                        if (res_values != "")
                        {
                            if (DictionaryBLL.isMatch(_register_member.UserName, res_values))
                            {
                                responseMsg["status"] = "error";
                                responseMsg["message"] = Resources.vsk.message_reg_03;
                                context.Response.Write(responseMsg);
                              
                                return;
                            }
                        }

                        // IP Address tracking and processing
                        string ip = context.Request.ServerVariables["REMOTE_ADDR"].ToString();
                        if (BlockIPBLL.Validate_IP(ip))
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "IP Blocked";
                            context.Response.Write(responseMsg);
                            return;
                        }


                        if (_mem.Check_UserName(_register_member.UserName))
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = Resources.vsk.message_reg_03;
                            context.Response.Write(responseMsg);
                            return;
                        }
                        if (_mem.Check_Email(_register_member.Email))
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = Resources.vsk.message_reg_04;
                            context.Response.Write(responseMsg);
                            
                            return;
                        }

                        // validation key processing
                        string val_key = "none";
                        int isenabled = 1; // user account activated
                        if (Config.isRegistrationValidation())
                        {
                            val_key = Guid.NewGuid().ToString().Substring(0, 10);
                            isenabled = 0; // user account deactivated
                        }
                        // Add Member
                        int type = 0; // normal member
                      
                        // Credits and Default Space Allocation
                        int credits = 0;
                        int remained_video = 0;
                        int remained_audio = 0;
                        int remained_gallery = 0;
                        int remained_photos = 0;
                        int remained_blogs = 0;
                        double space_video = 0;
                        double space_audio = 0;
                        double space_photos = 0;

                        if (Config.GetMembershipAccountUpgradeType() == 0)
                        {
                            if (!User_PackagesBLL.Check_Package_Feature())
                            {
                                // free user have some restricted features and services.
                                // load default free user package settings
                                List<Package_Struct> pck = PackagesBLL.Fetch_Record(Site_Settings.General_Default_Package_ID, false);
                                if (pck.Count > 0)
                                {
                                    credits = pck[0].Credits;
                                    remained_video = pck[0].Num_Videos;
                                    remained_audio = pck[0].Num_Audio;
                                    remained_gallery = pck[0].Num_Galleries;
                                    remained_photos = pck[0].Num_Photos;
                                    remained_blogs = pck[0].Num_Blogs;
                                    space_audio = pck[0].Space_Audio;
                                    space_video = pck[0].Space_Video;
                                    space_photos = pck[0].Space_Photo;
                                }
                                else
                                {
                                    // default package info not found, either package not exist or package is disabled currently.
                                    // in this case users records updated with 0 status.
                                }
                            }
                        }
                        int userrole_id = Site_Settings.Default_UserRoleID; // assign user default role at time of register

                        // encrypt password
                        //int BCRYPT_WORK_FACTOR = 10;
                        string encrypted_password = BCrypt.Net.BCrypt.HashPassword(_register_member.Password);
                        int atype = 0;
                        members.Add(atype, _register_member.UserName, encrypted_password, _register_member.Email, _register_member.CountryName, isenabled, _register_member.Gender, DateTime.Now, val_key, type, credits, remained_video, remained_audio, remained_gallery, remained_photos, remained_blogs, space_video, space_audio, space_photos, userrole_id);
                        // Create Required Directories
                        Directory_Process.CreateRequiredDirectories(context.Server.MapPath(context.Request.ApplicationPath) + "/contents/member/" + _register_member.UserName.ToLower());

                        // Send Mail
                        MailTemplateProcess_Register(_register_member.Email, _register_member.UserName, _register_member.Password, val_key);

                        if (Config.isRegistrationValidation())
                        {
                            responseMsg["status"] = "pending";
                            responseMsg["message"] = "Validation Required";
                            context.Response.Write(responseMsg);
                            return;
                        }
                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Registeration Completed";
                        context.Response.Write(responseMsg);
                        break;
                  
                    case "update_profile":
                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }
                        var _upd_mem = JsonConvert.DeserializeObject<Member_Struct>(json);
                        members.Update_User_Profile(_upd_mem.UserName,_upd_mem.FirstName,_upd_mem.LastName,_upd_mem.CountryName,_upd_mem.Gender,_upd_mem.RelationshipStatus,_upd_mem.AboutMe,_upd_mem.Website,_upd_mem.HometTown,_upd_mem.CurrentCity,_upd_mem.Zipcode,_upd_mem.Occupations,_upd_mem.Companies,_upd_mem.Schools,_upd_mem.Interests,_upd_mem.Movies,_upd_mem.Musics,_upd_mem.Books,_upd_mem.isAllowBirthDay);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "email_options":
                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        var _email_options = JsonConvert.DeserializeObject<Member_Struct>(json);

                        // validate email address and password.
                        var options = members.Get_Hash_Password(_email_options.UserName);
                        if (options.Count == 0)
                        {
                            // No user account found based on username search
                            responseMsg["status"] = "error";
                            responseMsg["message"] = Resources.vsk.message_emailoptions_03;
                            context.Response.Write(responseMsg);
                            return;
                        }
                        // check encrypted password
                        if (options[0].Password.Length < 20)
                        {
                            // backward compatibility
                            if (!members.Validate_Member_Email(_email_options.Email, _email_options.Password))
                            {
                                responseMsg["status"] = "error";
                                responseMsg["message"] = Resources.vsk.message_emailoptions_03;
                                context.Response.Write(responseMsg);
                                return;
                            }
                        }
                        else
                        {
                            // check encrypted password with user typed password
                            bool matched = BCrypt.Net.BCrypt.Verify(_email_options.Password, options[0].Password);
                            if (!matched)
                            {
                                responseMsg["status"] = "error";
                                responseMsg["message"] = Resources.vsk.message_emailoptions_03;
                                context.Response.Write(responseMsg);
                                return;
                            }
                        }

                        // update user validation key
                        var _key = Guid.NewGuid().ToString().Substring(0, 10);
                        members.Update_Value(_email_options.UserName, "val_key", _key);

                        // send mail validation request on new email address
                        MailTemplateProcess_EmailOptions(_email_options.Email, _email_options.UserName, _key);
                                               
                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Email change request sent on email";
                        context.Response.Write(responseMsg);
                        break;
                    case "signout":
                        // Authentication
                        FormsAuthentication.SignOut();

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Success";
                        context.Response.Write(responseMsg);
                        break;
                    case "change_password":
                        // Authentication
                        if (!context.User.Identity.IsAuthenticated)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Authentication Failed";
                            context.Response.Write(responseMsg);
                            return;
                        }

                        var _change_password = JsonConvert.DeserializeObject<Member_Struct>(json);
                        string _oldPassword = "";
                        string _newPassword = "";

                        if (context.Request.Params["op"] != null)
                        {
                            _oldPassword = context.Request.Params["op"].ToString();
                        }
                        if (context.Request.Params["np"] != null)
                        {
                            _newPassword = context.Request.Params["np"].ToString();
                        }
                        var _cPass = members.Get_Hash_Password(_change_password.UserName);
                        if (_cPass.Count == 0)
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = Resources.vsk.message_pass_01;
                            context.Response.Write(responseMsg);
                            return;
                        }
                        // check encrypted password
                        if (_cPass[0].Password.Length < 20)
                        {
                            // backward compatibility
                            // check existing user passwords with old system
                            if (!_mem.Validate_Member(_change_password.UserName, _oldPassword, false))
                            {
                                responseMsg["status"] = "error";
                                responseMsg["message"] = Resources.vsk.message_pass_01;
                                context.Response.Write(responseMsg);
                                return;
                            }
                        }
                        else
                        {
                            // check encrypted password with user typed password
                            bool matched = BCrypt.Net.BCrypt.Verify(_oldPassword, _cPass[0].Password);
                            if (!matched)
                            {
                                responseMsg["status"] = "error";
                                responseMsg["message"] = Resources.vsk.message_pass_01;
                                context.Response.Write(responseMsg);
                                return;
                            }
                        }
                        // change password
                        int BCRYPT_WORK_FACTOR = 10;
                        string _enc_pass = BCrypt.Net.BCrypt.HashPassword(_newPassword, BCRYPT_WORK_FACTOR);
                        members.Update_Value(_change_password.UserName, "password", _enc_pass);

                        MailTemplateProcess_ChangePassword(_change_password.UserName);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Email change request sent on email";
                        context.Response.Write(responseMsg);
                        break;

                  
                                          

                    case "validate_user":
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (members.Validate_Member(UserName))
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
                    /*case "validate_member_email":

                        var _val_email = JsonConvert.DeserializeObject<Member_Struct>(json);
                        if (members.Validate_Member_Email(_val_email.Email,_val_email.Password))
                        {
                            responseMsg["status"] = "success";
                            responseMsg["message"] = "Validated";
                        }
                        else
                        {
                            responseMsg["status"] = "error";
                            responseMsg["message"] = "Not Validated";
                        }
                        break; */
                   
                    case "check_username":

                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (_mem.Check_UserName(UserName))
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


                    case "check_email":

                        if (context.Request.Params["email"] != null)
                        {
                            Email = context.Request.Params["email"].ToString();
                        }
                        if (_mem.Check_Email(Email))
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

                    case "check_key":

                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (context.Request.Params["key"] != null)
                        {
                            Key = context.Request.Params["key"].ToString();
                        }
                        if (_mem.Check_Key(UserName, Key))
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
                    case "getpicture":
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        responseMsg["picture"] = _mem.Get_Picture_NO_Session(UserName);
                        context.Response.Write(responseMsg);

                        break;
                    case "increment_views":
                       
                        var _view_obj = JsonConvert.DeserializeObject<Member_Struct>(json);
                        members.Increment_Views(_view_obj.UserName, _view_obj.Views);
                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";

                        break;

                    case "update_isenabled":

                       
                        if (context.Request.Params["nval"] != null)
                        {
                            NewValue = Convert.ToInt32(context.Request.Params["nval"]);
                        }

                        var _upd_isenabled = JsonConvert.DeserializeObject<Member_Struct>(json);

                        _mem.Update_IsEnabled(_upd_isenabled.UserName, NewValue);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;
                    case "update_user_roles":

                        

                        var _update_role = JsonConvert.DeserializeObject<Member_Struct>(json);

                        members.Update_User_Role(_update_role.UserName, _update_role.RoleID);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);

                        break;
                        
                    case "update_field":

                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        if (context.Request.Params["val"] != null)
                        {
                            Value = context.Request.Params["val"].ToString();
                        }
                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }

                        members.Update_Value(UserName, FieldName, Value);

                        responseMsg["status"] = "success";
                        responseMsg["message"] = "Operation Commit";
                        context.Response.Write(responseMsg);
                        break;

                    case "get_field_value":

                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }

                        if (context.Request.Params["field"] != null)
                        {
                            FieldName = context.Request.Params["field"].ToString();
                        }

                        responseMsg["value"] = members.Return_Value(UserName, FieldName);

                        context.Response.Write(responseMsg);
                        break;

                    case "load_channels":

                        var _ld_video_json = JsonConvert.DeserializeObject<Member_Struct>(json);
                        var _vObject = new MembersObject()
                        {
                            Data = _mem.Load_Channels_ADV(_ld_video_json),
                            Count = _mem.Count_Channels(_ld_video_json)
                        };

                        _ld_video_data["data"] = _vObject;

                        context.Response.Write(_ld_video_data);

                        break;

                    case "load_users_autocomplete":
                        string _Term = "";
                        if (context.Request.Params["term"] != null)
                        {
                            _Term = context.Request.Params["term"].ToString();
                        }
                        
                        responseMsg["data"] = members.Load_User_AutoComplete(_Term);

                        context.Response.Write(responseMsg);
                        break;

                    case "fetch_record":
                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }

                        _ld_video_data["data"] = new MembersObject()
                        {
                            Data = members.Fetch_User_Profile(UserName),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_user_channels":

                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        _ld_video_data["data"] = new MembersObject()
                        {
                            Data = members.Fetch_User_Channel(UserName),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_user_detail_profile":

                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }
                        _ld_video_data["data"] = new MembersObject()
                        {
                            Data = members.Fetch_User_DetailProfile(UserName),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_user_status_info":

                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }

                        _ld_video_data["data"] = new MembersObject()
                        {
                            Data = members.Fetch_User_Status_Info(UserName),
                            Count = 0
                        };
                        context.Response.Write(_ld_video_data);

                        break;

                    case "fetch_user_usernames":

                        if (context.Request.Params["type"] != null)
                        {
                            Type = Convert.ToInt32(context.Request.Params["type"]);
                        }
                        _ld_video_data["data"] = new MembersObject()
                        {
                            Data = members.Fetch_User_UserNames(Type),
                            Count = 0
                        };

                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_user_info":

                        if (context.Request.Params["user"] != null)
                        {
                            UserName = context.Request.Params["user"].ToString();
                        }

                        _ld_video_data["data"] = new MembersObject()
                        {
                            Data = members.Fetch_User_Info(UserName),
                            Count = 0
                        };

                        context.Response.Write(_ld_video_data);

                        break;
                    case "fetch_usernames":

                      
                        _ld_video_data["data"] = new MembersObject()
                        {
                            Data = members.Fetch_User_UserNames(),
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

        private void MailTemplateProcess_Register(string emailaddress, string username, string password, string key)
        {
            //if sending mail option enabled
            if (Config.isEmailEnabled())
            {
                System.Collections.Generic.List<Struct_MailTemplates> lst = MailTemplateBLL.Get_Record("USRREG");
                if (lst.Count > 0)
                {
                    string subject = MailProcess.Process2(lst[0].Subject, "\\[username\\]", username);
                    string contents = MailProcess.Process2(lst[0].Contents, "\\[username\\]", username);
                    contents = MailProcess.Process2(contents, "\\[password\\]", password);
                    string validation_url = Config.GetUrl("validate.aspx?key=" + key.Trim() + "&user=" + username.Trim());
                    string url = "<a href=\"" + validation_url + "\">" + validation_url + "</a>";
                    contents = MailProcess.Process2(contents, "\\[key_url\\]", url);

                    MailProcess.Send_Mail(emailaddress, subject, contents);
                }
            }
        }

        private void MailTemplateProcess_EmailOptions(string emailaddress, string username, string key)
        {
            //if sending mail option enabled
            if (Config.isEmailEnabled())
            {
                System.Collections.Generic.List<Struct_MailTemplates> lst = MailTemplateBLL.Get_Record("USREMLCREQ");
                if (lst.Count > 0)
                {
                    string subject = MailProcess.Process2(lst[0].Subject, "\\[username\\]", username);
                    string contents = MailProcess.Process2(lst[0].Contents, "\\[username\\]", username);
                    contents = MailProcess.Process2(contents, "\\[key_url\\]", key);
                    contents = MailProcess.Process2(contents, "\\[emailaddress\\]", emailaddress);

                    MailProcess.Send_Mail(emailaddress, subject, contents);
                }
            }
        }

        private void MailTemplateProcess_ChangePassword(string username)
        {
            //if sending mail option enabled
            if (Config.isEmailEnabled())
            {
                string emailaddress = members.Return_Value(username, "email");
                System.Collections.Generic.List<Struct_MailTemplates> lst = MailTemplateBLL.Get_Record("USRPASSCHN");
                if (lst.Count > 0)
                {
                    string subject = MailProcess.Process2(lst[0].Subject, "\\[username\\]", username);
                    string contents = MailProcess.Process2(lst[0].Contents, "\\[username\\]", username);

                    MailProcess.Send_Mail(emailaddress, subject, contents);
                }
            }
        }
    }

    public class MembersObject
    {
        public List<Member_Struct> Data { get; set; }
        public int Count { get; set; }
    }
}
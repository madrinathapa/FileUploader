//****************************************************************************************
//FileName:Login.aspx.cs
// Description:This page is used for logging in. 
// Created By:Madrina Thapa
//******************************************************************************************

using IGroup.Modules.Common.Utility;
using Uploader.Business;
using Uploader.Entities;
using Uploader.Utilities;
using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Uploader
{
    public partial class Login : System.Web.UI.Page
    {
        /// <summary>
        /// page load event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                //Check whether the cookie has expired
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    CustomPrincipal existingUser = HttpContext.Current.User as CustomPrincipal;
                    string userHashCode = existingUser.Id;
                    BusinessLogic objBusiness = new BusinessLogic();
                    User objUser = objBusiness.GetUserDetailsByHashCode(userHashCode);

                    //Save data in session from cookies
                    if (objUser != null)
                    {
                        //Redirect to upload file page
                        Response.Redirect("MyUploadLandingPage.aspx", false);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Login button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            //Validates the user
            ValidateUser(txtEmail.Text.Trim(), txtPassword.Text.Trim());
        }

        /// <summary>
        /// vaidation of the user
        /// </summary>
        /// <param name="email">email textbox's content</param>
        /// <param name="password">Password textbox's content</param>
        private void ValidateUser(string email, string password)
        {
            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                bool isValidUser = objBusiness.ValidateUser(email, password, chkRememberMe.Checked);

                //Check if the email and password is a valid combination
                if (isValidUser)
                {
                    //Redirects to the upload file page.
                    System.Web.HttpContext.Current.Response.Redirect(ConstantUtility.UploadPage, false);
                }
                else
                {
                    divErrorMessage.Visible = true;
                    divErrorMessage.InnerHtml = ConstantUtility.UserValidationError;
                }
            }
            catch (Exception ex)
            {
                //log the error.
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// Save data in the session state
        /// </summary>
        /// <param name="email">user's email id.</param>
        /// <param name="userHashCode">user's unique id.</param>
        /// <param name="userName">First name and last name of the user</param>
        /// <param name="isAdmin">bool value to tell whether the user is admin or not</param>
        /// <param name="isRememberMe">bool value to tell whether to save data in cookies or not</param>
        public void SaveSession(User user, bool isRememberMe)
        {
            try
            {
                CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                serializeModel.Id = user.UserHashCode;
                serializeModel.UserName = user.UserName;
                serializeModel.IsAdmin = Convert.ToInt16(user.IsAdmin);

                JavaScriptSerializer serializer = new JavaScriptSerializer();

                string userData = serializer.Serialize(serializeModel);
                FormsAuthenticationTicket authTicket = null;

                //If remember me is checked cookie is saved for 15 days
                if (isRememberMe)
                {
                    authTicket = new FormsAuthenticationTicket(
                            1,
                            user.EmailId,
                            DateTime.Now,
                            DateTime.Now.AddDays(15),
                            false,
                            userData);
                }
                else
                {
                    authTicket = new FormsAuthenticationTicket(
                             1,
                             user.EmailId,
                             DateTime.Now,
                             DateTime.Now.AddMinutes(15),
                             false,
                             userData);
                }

                string encTicket = FormsAuthentication.Encrypt(authTicket);

                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                System.Web.HttpContext.Current.Response.Cookies.Add(faCookie);

                //save the encrypted cookie value in the database
                BusinessLogic objBusiness = new BusinessLogic();
                objBusiness.SaveSessionData(user.UserHashCode, encTicket);
            }

            catch (Exception ex)
            {
                //Log the error.
                ErrorUtility.WriteError(ex);
            }
        }
    }
}
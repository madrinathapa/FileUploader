//****************************************************************************************
//FileName:Site.Master.cs
// Description:This is the master page.
// Created By:Madrina Thapa
//******************************************************************************************

using IGroup.Modules.Common.Utility;
using Uploader.Data;
using Uploader.Entities;
using System;
using System.Web;

namespace Uploader
{
    public partial class Site : System.Web.UI.MasterPage
    {
        public CustomPrincipal CurrentUser
        {
            get
            {
                if (HttpContext.Current.User != null)
                {
                    return HttpContext.Current.User as CustomPrincipal;
                }
                else
                {
                    return null;
                }

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Check for user session
            //If session is not there, redirect to login page.
            if (CurrentUser == null)
            {
                Response.Redirect("Login.aspx", false);
                return;
            }

            //Save the email and userhashcode in session from cookies
            if (!IsPostBack && CurrentUser != null)
            {
                //get user's upload history
                GetUserDetails();
                hdnUserHashCode.Value = CurrentUser.Id;
            }

            //Code disables caching by browser.
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            try
            {
                System.Web.Security.FormsAuthentication.SignOut();

                Session.Clear();
                Session.Abandon();

                //clear authentication cookie
                HttpCookie authCookie = new HttpCookie(System.Web.Security.FormsAuthentication.FormsCookieName, "");
                authCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(authCookie);

                // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
                HttpCookie aspCookie = new HttpCookie("ASP.NET_SessionId", "");
                aspCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(aspCookie);

                //Redirect to the login page
                Response.Redirect("Login.aspx", false);
            }
            catch (Exception Ex)
            {
                //Log the error
                ErrorUtility.WriteError(Ex);
            }
        }

        private void GetUserDetails()
        {
            //get the account details of the logged in user
            try
            {
                DataAccessLayer ObjData = new DataAccessLayer();

                UserAccount objAccDetails = ObjData.GetAccountDetails(this.CurrentUser.Id);

                //Check if the record exists for the user
                if (objAccDetails != null)
                {
                    lblFiles.Text = objAccDetails.TotalFiles + lblFiles.Text;
                    lblTotalDownloads.Text = objAccDetails.TotalDownloads + lblTotalDownloads.Text;
                    lblTotalSize.Text = objAccDetails.TotalSize + lblTotalSize.Text;
                }

                lblUserName.Text = this.CurrentUser.UserName;
            }
            catch (Exception Ex)
            {
                //log the error
                ErrorUtility.WriteError(Ex);
            }

        }



    }
}
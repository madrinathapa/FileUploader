//****************************************************************************************
//FileName:RegisteredUsers.aspx.cs
// Description:This page shows all the registered users in a grid but only the admin can 
//             access this page.
// Created By:Madrina Thapa
//******************************************************************************************

using Uploader.Utilities;
using System;

namespace Uploader
{
    public partial class RegisteredUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Check whether the cookie has expired
            if (this.Master.CurrentUser == null)
            {
                Response.Redirect(ConstantUtility.LoginPage, false);
            }
            else if (!IsPostBack)
            {
                //Redirect to uploadpage if the logged in user isnt an admin.
                if (this.Master.CurrentUser.IsAdmin != 1)
                {
                    Response.Redirect(ConstantUtility.UploadPage, false);
                }
            }

        }
    }
}
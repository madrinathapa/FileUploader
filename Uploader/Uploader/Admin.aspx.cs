//****************************************************************************************
//FileName:Admin.aspx.cs
// Description:This page is used for showing all the files uploaded by
//             all the users.
// Created By:Madrina Thapa
//******************************************************************************************

using System;
using Uploader.Utilities;

namespace Uploader
{
    public partial class Admin : System.Web.UI.Page
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
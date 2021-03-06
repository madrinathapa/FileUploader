﻿//****************************************************************************************
//FileName:Global.asax.cs
// Description: This is used for form authentication
// Created By:Madrina Thapa
//******************************************************************************************

using Uploader.Entities;
using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Uploader
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            //read the cookie value
            
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                //Decrypt the cookie value.
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                JavaScriptSerializer serializer = new JavaScriptSerializer();

                //Deserialize the cookie value
                CustomPrincipalSerializeModel serializeModel = serializer.Deserialize<CustomPrincipalSerializeModel>(authTicket.UserData);

                CustomPrincipal newUser = new CustomPrincipal(authTicket.Name);
                newUser.Id = serializeModel.Id;
                newUser.UserName = serializeModel.UserName;
                newUser.IsAdmin = serializeModel.IsAdmin;

                HttpContext.Current.User = newUser;

            }
        }
    }
}
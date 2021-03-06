﻿//****************************************************************************************
//FileName:Register.aspx.cs
// Description:This page is used for registering a new user. 
// Created By:Madrina Thapa
//******************************************************************************************

using Uploader.Business;
using Uploader.Entities;
using Uploader.Utilities;
using System;
using System.Web;
using System.Web.UI.WebControls;

namespace Uploader
{
    public partial class Register : System.Web.UI.Page
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
        bool invalidCaptcha = false;

        /// <summary>
        /// Page load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (CurrentUser != null)
                {
                    Response.Redirect(ConstantUtility.UploadPage, false);
                }
            }
        }

        /// <summary>
        /// btnRegister click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string password = string.Empty;
            string firstName = string.Empty;
            string lastName = string.Empty;
            string email = string.Empty;
            bool isUserExist = false;

            try
            {
                password = txtSignUpPassword.Text.Trim();

                if (!invalidCaptcha)
                {
                    firstName = txtFirstName.Text;
                    lastName = txtLastName.Text;
                    email = txtSignUpEmail.Text.ToLower();

                    BusinessLogic objBusiness = new BusinessLogic();
                    isUserExist = objBusiness.UserExist(email);

                    if (!isUserExist)
                    {
                        //Add a new user
                        SaveUser(firstName, lastName, email, password);
                    }
                    else
                    {
                        txtCaptcha.Text = string.Empty;
                        divExistingUser.InnerHtml = ConstantUtility.ExistingUserError;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// validation for captcha 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ValidateCaptcha(object sender, ServerValidateEventArgs e)
        {
            try
            {
                SignUpCaptcha.ValidateCaptcha(txtCaptcha.Text.Trim());
                e.IsValid = SignUpCaptcha.UserValidated;

                if (!e.IsValid)
                {
                    invalidCaptcha = true;
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// Saves the user details
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public void SaveUser(string firstName, string lastName, string email, string password)
        {
            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                bool isSuccess = objBusiness.SaveUser(firstName, lastName, email, password);

                if (isSuccess)
                {
                    dvSuccessfulAlert.Visible = true;
                    dvRegister.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }
        }
    }
}
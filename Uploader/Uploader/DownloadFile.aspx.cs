//****************************************************************************************
//FileName:DownloadFile.aspx.cs
// Description:This page is used for downloading a file using the url generated for download.
// Created By:Madrina Thapa
//******************************************************************************************

using IGroup.Modules.Common.Utility;
using Uploader.Business;
using Uploader.Data;
using Uploader.Entities;
using Uploader.Utilities;
using System;
using System.Configuration;
using System.Web;
using System.Web.UI;

namespace Uploader
{
    public partial class DownloadFile : System.Web.UI.Page
    {
        #region Properties

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

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            
            UploadedFile fileDetails = null;
            string fileId = string.Empty;

            try
            {
                if (!IsPostBack)
                {
                    if (HttpContext.Current.Request.QueryString["id"] != null)
                    {
                        if (CurrentUser.IsAdmin == 1)
                        {
                            fileId = HttpContext.Current.Request.QueryString["id"];
                            Page.ClientScript.RegisterStartupScript(typeof(Page), "CloseConsent", "window.close();", true);
                        }
                    }
                    else
                    {
                        fileId = HttpContext.Current.Request.QueryString["FileId"];
                        //Decrypt the file id
                        fileId = fileId.Replace(" ", "+");
                        fileId = IGroupUtility.Decrypt(fileId);
                    }
                    hdnFileId.Value = fileId;

                    DataAccessLayer objData = new DataAccessLayer();

                    //Get the name of the file.
                    fileDetails = objData.DownloadFile(Convert.ToInt32(fileId));

                    if (fileDetails != null && fileDetails.IsLocked
                        && !fileDetails.IsDeleted)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "myFunction", "ShowModalBox();", true);
                    }
                    else
                    {
                        string filePath = System.IO.Path.Combine(
                              IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.FolderPath]), fileDetails.SavedFileName);

                        //Check if the file is deleted or not by the user
                        if ((fileDetails != null && !fileDetails.IsDeleted && System.IO.File.Exists(filePath)) ||
                            (CurrentUser != null && CurrentUser.IsAdmin == 1 && fileDetails != null && !fileDetails.IsAdminDeleted
                            && System.IO.File.Exists(filePath)))
                        {
                            //download the file if the file exists.
                            BusinessLogic objBusiness = new BusinessLogic();

                            objBusiness.DownloadFile(Convert.ToInt32(fileId), fileDetails);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", ConstantUtility.DownloadError, true);
                        }
                    }
                }
                else
                {
                    hdnFilePassword.Value = txtFilePassword.Value;
                }
            }
            catch (Exception ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// click method for downloading a locked file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUnlock_ServerClick(object sender, EventArgs e)
        {
            BusinessLogic objBusiness = null;
            DataAccessLayer objData = null;

            try
            {
                objData = new DataAccessLayer();

                int fileId = 0;
                int.TryParse(Convert.ToString(hdnFileId.Value), out fileId);
                UploadedFile fileDetails = objData.DownloadFile(fileId);
                string filePwd = hdnFilePassword.Value;

                //If the file password matches and the file is not deleted it gets downloaded
                if (fileDetails != null && string.Equals(filePwd, fileDetails.FilePassword) && !fileDetails.IsDeleted)
                {
                    //download the file.
                    objBusiness = new BusinessLogic();
                    objBusiness.DownloadFile(Convert.ToInt32(fileId), fileDetails);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "myFunction", "ShowModalBoxWithMessage();", true);
                }

            }
            catch (Exception Ex)
            {
                ErrorUtility.WriteError(Ex);
            }
            finally
            {
                objBusiness = null;
                objData = null;

            }
        }
    }
}
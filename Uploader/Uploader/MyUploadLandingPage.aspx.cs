﻿//****************************************************************************************
//FileName:MyUploadLandingPage.aspx.cs
// Description:This page is Home page.
//              shows all the files uploade by the user and user can upload new files too.
// Created By:Madrina Thapa
//******************************************************************************************

using IGroup.Modules.Common.Utility;
using Uploader.Business;
using Uploader.Data;
using Uploader.Utilities;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Uploader
{
    public partial class MyUploadLandingPage : System.Web.UI.Page
    {

        #region PageMethods
        /// <summary>
        /// Get called on page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (this.Master.CurrentUser != null)
                    {
                        GetFileList(0, string.Empty);

                        if (Session["FileId"] != null)
                        {
                            hdnFileIdTinyUrl.Value = Convert.ToString(Session["FileId"]);
                            Session["FileId"] = null;
                            ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", "SaveTinyUrl();", true);
                        }

                        
                    }

                }
            }
            catch (Exception ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lvFile_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            int fileId = 0;
            BusinessLogic objBusiness = null;

            try
            {
                if (e.CommandArgument != null && e.CommandName.Equals("DownloadFile"))
                {
                    int.TryParse(e.CommandArgument.ToString(), out fileId);
                    objBusiness = new BusinessLogic();
                    bool isSuccess = objBusiness.DownloadFile(fileId);

                    //File could not be downloaded
                    if (!isSuccess)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", ConstantUtility.DownloadError, true);
                    }
                }

            }
            catch (Exception ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }

            finally
            {
                objBusiness = null;
        }
        }

        /// <summary>
        /// Search button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearchFiles_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearchFileMain.Text.Trim();

            //Gets the list if search term is not empty else
            //does nothing
            if (!string.IsNullOrEmpty(searchTerm))
            {
                GetFileList(1, searchTerm);
            }
            else
            {
                GetFileList(0, string.Empty);
            }
        }

        /// <summary>
        /// List view's item bound command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lvFile_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            string backGroundImage = string.Empty;
            string extension = string.Empty;

            try
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                if (e.Item.ItemType == ListViewItemType.DataItem)
                {
                    System.Web.UI.HtmlControls.HtmlGenericControl divFileBox = e.Item.FindControl(ConstantUtility.FileBox) as System.Web.UI.HtmlControls.HtmlGenericControl;
                    System.Web.UI.HtmlControls.HtmlGenericControl dvExtension = e.Item.FindControl(ConstantUtility.ExtensionBox) as System.Web.UI.HtmlControls.HtmlGenericControl;
                    System.Web.UI.HtmlControls.HtmlImage imgThumbnail = e.Item.FindControl(ConstantUtility.ThumbImage) as System.Web.UI.HtmlControls.HtmlImage;

                    if (System.Web.UI.DataBinder.Eval(e.Item.DataItem, ConstantUtility.ThumbName) != DBNull.Value
                        && !string.IsNullOrEmpty((string)System.Web.UI.DataBinder.Eval(e.Item.DataItem, ConstantUtility.ThumbName)))
                    {
                        imgThumbnail.Attributes.CssStyle.Add("display", "block");
                        dvExtension.Attributes.CssStyle.Add("display", "none");

                        backGroundImage = (string)System.Web.UI.DataBinder.Eval(e.Item.DataItem, ConstantUtility.ThumbName);

                        //For setting thumbnail.
                        if (!string.IsNullOrEmpty(backGroundImage))
                        {
                            backGroundImage = IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.ThumbFolder]) + @"\\" + backGroundImage;
                            string thumbnailPath = Server.MapPath(backGroundImage);   
                     
                            //if the image exists, set it as a source
                            if (System.IO.File.Exists(thumbnailPath))
                            {
                                imgThumbnail.Src = backGroundImage;
                            }
                        }
                    }
                    else if (System.Web.UI.DataBinder.Eval(e.Item.DataItem, ConstantUtility.Exe) != DBNull.Value)
                    {
                        dvExtension.Attributes.CssStyle.Add("display", "block");
                        imgThumbnail.Attributes.CssStyle.Add("display", "none");
                       
                            extension = (string)System.Web.UI.DataBinder.Eval(e.Item.DataItem, ConstantUtility.Exe);
                            string color = ConstantUtility.ExeBackColor;
                            divFileBox.Attributes.CssStyle.Add("background-color", color);
                            dvExtension.InnerText = extension;
                    }
                   
                    bool fileStatus = false;
                    if (System.Web.UI.DataBinder.Eval(e.Item.DataItem, ConstantUtility.FileStatus) != DBNull.Value)
                    {
                        bool.TryParse(Convert.ToString(System.Web.UI.DataBinder.Eval(e.Item.DataItem, ConstantUtility.FileStatus))
                            , out fileStatus);
                    }

                    System.Web.UI.HtmlControls.HtmlGenericControl dvLock = e.Item.FindControl(ConstantUtility.Lock) as System.Web.UI.HtmlControls.HtmlGenericControl;
                    System.Web.UI.HtmlControls.HtmlGenericControl dvUnlock = e.Item.FindControl(ConstantUtility.Unlock) as System.Web.UI.HtmlControls.HtmlGenericControl;

                    //If the file is locked
                    if (fileStatus)
                    {
                        dvLock.Attributes.CssStyle.Add("display", "none");
                        dvUnlock.Attributes.CssStyle.Add("display", "block");
                    }
                    else //if the file is not locked
                    {
                        dvLock.Attributes.CssStyle.Add("display", "block");
                        dvUnlock.Attributes.CssStyle.Add("display", "none");
                    }

                    if (System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TinyUrl") != DBNull.Value)
                    {
                        HiddenField hdnFileUrl = e.Item.FindControl("hdnFileUrl") as HiddenField;

                        if (hdnFileUrl != null)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TinyUrl"))))
                            {
                                hdnFileUrl.Value = Convert.ToString(System.Web.UI.DataBinder.Eval(e.Item.DataItem, "TinyUrl"));
                            }
                            else
                            {
                                if (System.Web.UI.DataBinder.Eval(e.Item.DataItem, "FileId") != DBNull.Value)
                                {
                                    int fileId = 0;

                                    if (int.TryParse(Convert.ToString(System.Web.UI.DataBinder.Eval(e.Item.DataItem, "FileId")), out fileId))
                                    {
                                        hdnFileUrl.Value = GetFileUrl(fileId);
                                    }
                                }

                            }

                        }
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region User Define Methods
        /// <summary>
        /// Gets the list of all the files according to the search string
        /// if the search is true
        /// </summary>
        /// <param name="isSearchable"></param>
        /// <param name="searchTerm"></param>
        public void GetFileList(int isSearchable, string searchTerm)
        {
            DataTable dtFile = null;
            DataAccessLayer ObjData = null;

            try
            {
                ObjData = new DataAccessLayer();

                //Get all the files uploaded by the logged in user.
                dtFile = ObjData.GetAllFiles(this.Master.CurrentUser.Id, this.Master.CurrentUser.IsAdmin, isSearchable, searchTerm);

                
                lvFile.DataSource = dtFile;
                lvFile.DataBind();
                
                if (dtFile != null && dtFile.Rows.Count > 0)
                {
                    lblNoData.Visible = false;
                }
                else
                {
                    lblNoData.Visible = true;
                }
                

            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
            finally
            {
                dtFile = null;
                ObjData = null;
            }

        }

        /// <summary>
        /// shortens a string if the length is 
        /// greater than 19
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetShortName(object value)
        {

            if (value != null)
            {
                string name = Convert.ToString(value);

                if (name.Length > 24)
                {
                    return name.Substring(0, 16) + ConstantUtility.Ellipsis + Path.GetExtension(name);
                }
                else
                {
                    return name;
                }
            }
            return value.ToString();
        }

        /// <summary>
        /// Check the value if null return empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ReturnEmptyIfNull(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }

       
        public string GetFileUrl(int value)
        {
            BusinessLogic objBusinessLogic = null;

            //We need the fileId in string format to encode it.
            string fileId = string.Empty;

            if (value != null)
            {
                fileId = Convert.ToString(value);
            }

            objBusinessLogic = new Business.BusinessLogic();

            return objBusinessLogic.GetFileUrl(fileId);
        }

        public string ReturnChangeFileNameString(object fileId, object name)
        {
            StringBuilder changeFileNameString = new StringBuilder();
            changeFileNameString.Append("ChangeFileName('" + ReturnEmptyIfNull(fileId) + "','" + ReturnEmptyIfNull(name) + "', this); return false;");
            return changeFileNameString.ToString();
        }
        #endregion
        
    }
}

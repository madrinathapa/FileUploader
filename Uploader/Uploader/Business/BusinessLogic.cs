//****************************************************************************************
//FileName:BusinessLogic.cs
// Description:This class has all the business logic
// Created By:Madrina Thapa
//******************************************************************************************

using IGroup.Modules.Common.Utility;
using Uploader.Data;
using Uploader.Entities;
using Uploader.Utilities;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace Uploader.Business
{
    public class BusinessLogic
    {
        # region Login

        /// <summary>
        /// this method calls the data layer to check
        /// whether the user is valid or not.
        /// </summary>
        /// <param name="email">registered email id of the user.</param>
        /// <param name="password">user's password.</param>
        /// <param name="isRememberMe">checkbox value of remember me</param>
        /// <returns>bool value whether the user is valid or not</returns>
        public bool ValidateUser(string email, string password, bool isRememberMe)
        {
            try
            {
                // Encrypt the password
                password = IGroupUtility.Encrypt(password.Trim());
                DataAccessLayer objData = new DataAccessLayer();
                User user = objData.GetUserDetails(email, password);

                //Check if the email and password is a valid combination
                if (user != null)
                {
                    //save the data in session state
                    Login objLogin = new Login();
                    user.EmailId = email;
                    objLogin.SaveSession(user, isRememberMe);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                return false;
            }
        }

        /// <summary>
        /// for saving user's details in the database
        /// </summary>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">last name</param>
        /// <param name="email">email id provided</param>
        /// <param name="password">password entered by the user</param>
        /// <returns>whether the data is saved successfully or not</returns>
        public bool SaveUser(string firstName, string lastName, string email, string password)
        {
            try
            {
                password = IGroupUtility.Encrypt(password);
                DataAccessLayer objData = new DataAccessLayer();
                return objData.SaveUserDetails(firstName, lastName, email, password);
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                return false;
            }
        }

        /// <summary>
        /// Uses email id to check whether the emailid
        /// is already registered or not.
        /// </summary>
        /// <param name="emailId">email id</param>
        /// <returns>a boolean value</returns>
        public bool UserExist(string emailId)
        {
            bool isUserExist = false;
            try
            {
                DataAccessLayer objData = new DataAccessLayer();

                //Check if user already exists.
                isUserExist = objData.GetUserByEmail(emailId);
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
            return isUserExist;
        }

        /// <summary>
        /// Gets the user details using hashcode
        /// </summary>
        /// <param name="userHashCode">user's unique hashcode</param>
        /// <returns>user type object with all the details of the user</returns>
        public User GetUserDetailsByHashCode(string userHashCode)
        {
            User objUser = null; 
            try
            {
                DataAccessLayer objData = new DataAccessLayer();

                objUser = objData.GetUserByHashCode(userHashCode);
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
            return objUser;
        }

        public void SaveSessionData(string userHashcode, string sessionData)
        {
            try
            {
                DataAccessLayer objData = new DataAccessLayer();
                objData.SaveSessionData(userHashcode, sessionData);
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
        }

        #endregion

        #region MyUploadLandingPage

        /// <summary>
        /// deletes the particular file
        /// </summary>
        /// <param name="fileId"> Unique file id</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="isAdmin">Bool value indicating whether the user is admin or not</param>
        public bool DeleteFile(int fileId, bool isAdmin = false)
        {
            bool isSuccess = false;
            bool status = false;
            DataAccessLayer objData = null;

            try
            {
                objData = new DataAccessLayer();
                UploadedFile file = null;

                file = objData.GetFileName(fileId);

               string FileLife= IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.FileLife]);

                //Delete the thumbnail  
                if (file != null && !string.IsNullOrEmpty(file.ThumbName)
                    && Convert.ToString(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.ImageExtensions]).Contains(file.Extension))
                {
                    string thumbNailPath = Path.Combine(IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.ThumbImagePath]), file.ThumbName);

                    //Check whether the file exists
                    if (File.Exists(thumbNailPath))
                    {
                        File.Delete(thumbNailPath);
                    }
                }

                //To delete the original file saved
                if (file != null && !string.IsNullOrEmpty(file.SavedFileName))
                {
                    string filePath =
                        Path.Combine(IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.FolderPath]), file.SavedFileName);

                    //checks whether the user exists and isAdmin or not
                    if (File.Exists(filePath) && (isAdmin || Convert.ToBoolean(FileLife)))
                    {
                        File.Delete(filePath);
                    }
                    isSuccess = true;
                }

                //Update the table if the file is deleted successfully
                if (isSuccess)
                {
                   status= objData.DeleteFile(fileId, isAdmin);
                }
            }
            catch (Exception ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
                status = false;
                isSuccess = false;
            }
            finally
            {
                objData = null;
            }
            return status;
        }

        /// <summary>
        /// Downloads the particular file.
        /// </summary>
        /// <param name="fileId">Id of the file</param>
        /// <param name="fileDetails">An Uploadfile obj containing all the file details</param>
        public void DownloadFile(int fileId, UploadedFile fileDetails)
        {
            //Update the log
            DataAccessLayer objData = new DataAccessLayer();
            objData.DownloadFileLog(Convert.ToInt32(fileId));

            //Download the file
            WebClient req = new WebClient();
            HttpResponse response = HttpContext.Current.Response;
            string filePath = string.Empty;

            try
            {
                req = new WebClient();
                response = HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                if (fileDetails != null)
                {
                    string SavedFileName = fileDetails.SavedFileName;
                    string OriginalFileName = fileDetails.OriginalFileName;
                    filePath = Path.Combine(
                        IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.FolderPath]), SavedFileName);
                    response.AddHeader("Content-Disposition", "attachment;filename=\"" + OriginalFileName + "\"");
                }

                byte[] data = req.DownloadData(filePath);
                response.BinaryWrite(data);
                HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
                HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
                HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.
            }
            catch (Exception ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// Downloads the file 
        /// </summary>
        /// <param name="fileId">Id of the file to be downloaded</param>
        public bool DownloadFile(int fileId)
        {
            DataAccessLayer ObjData = new DataAccessLayer();

            //Get the file details.
            UploadedFile fileDetails = ObjData.DownloadFile(fileId);

            //Download the file
            BusinessLogic objBusiness = new BusinessLogic();
            if (!fileDetails.IsDeleted)
            {
                objBusiness.DownloadFile(fileId, fileDetails);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Admin

        /// <summary>
        /// Gets the DB column name based 
        /// on jqGrid column name.
        /// </summary>
        /// <param name="colName">jqgrid's column name</param>
        /// <returns>Name of the corresponding column in the db</returns>
        public string GetColumnName(string colName)
        {
            try
            {
                switch (colName)
                {
                    case "id": colName = ConstantUtility.FileId;
                        break;
                    case "fileName": colName = ConstantUtility.OriginalFileName;
                        break;
                    case "uploadedBy": colName = ConstantUtility.UploadedBy;
                        break;
                    case "uploadedOn": colName = ConstantUtility.UploadedDate;
                        break;
                    case "size": colName = ConstantUtility.FileSize;
                        break;
                    case "status": colName = ConstantUtility.Status;
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
            return colName;
        }

        /// <summary>
        /// Gets the operator to be used in the SQl
        /// based on the search operator.
        /// </summary>
        /// <param name="searchOper">The search operator</param>
        /// <returns>The equivalent operator for SQL</returns>
        public string GetOperator(string searchOper)
        {
            try
            {
                switch (searchOper)
                {
                    case "cn": searchOper = ConstantUtility.Like;
                        break;
                    case "nc": searchOper = ConstantUtility.NotLike;
                        break;
                    case "eq": searchOper = ConstantUtility.Equal;
                        break;
                    case "ne": searchOper = ConstantUtility.NotEqual;
                        break;
                    case "lt": searchOper = ConstantUtility.LessThan;
                        break;
                    case "le": searchOper = ConstantUtility.LessThanEqual;
                        break;
                    case "gt": searchOper = ConstantUtility.GreaterThan;
                        break;
                    case "ge": searchOper = ConstantUtility.GreaterThanEqual;
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
            return searchOper;
        }

        /// <summary>
        /// gets the name of the icon image acc. 
        /// to the file extension
        /// </summary>
        /// <param name="ext">extension name</param>
        /// <returns>extension icon name</returns>
        public string ExtIconName(string ext)
        {
            string extImage = string.Empty;
            try
            {
                switch (ext)
                {
                    case ConstantUtility.Extensions.ASP:
                        extImage = ConstantUtility.ExtImage.ASP;
                        break;
                    case ConstantUtility.Extensions.ASPX:
                        extImage = ConstantUtility.ExtImage.ASPX;
                        break;
                    case ConstantUtility.Extensions.AVI:
                        extImage = ConstantUtility.ExtImage.VIDEO;
                        break;
                    case ConstantUtility.Extensions.CSHARP:
                        extImage = ConstantUtility.ExtImage.CSHARP;
                        break;
                    case ConstantUtility.Extensions.DOC:
                        extImage = ConstantUtility.ExtImage.DOC;
                        break;
                    case ConstantUtility.Extensions.DOCX:
                        extImage = ConstantUtility.ExtImage.DOC;
                        break;
                    case ConstantUtility.Extensions.EXE:
                        extImage = ConstantUtility.ExtImage.EXE;
                        break;
                    case ConstantUtility.Extensions.FLV:
                        extImage = ConstantUtility.ExtImage.VIDEO;
                        break;
                    case ConstantUtility.Extensions.HTM:
                        extImage = ConstantUtility.ExtImage.HTML;
                        break;
                    case ConstantUtility.Extensions.HTML:
                        extImage = extImage = ConstantUtility.ExtImage.HTML;
                        break;
                    case ConstantUtility.Extensions.MP3:
                        extImage = ConstantUtility.ExtImage.AUDIO;
                        break;
                    case ConstantUtility.Extensions.MP4:
                        extImage = ConstantUtility.ExtImage.VIDEO;
                        break;
                    case ConstantUtility.Extensions.MPEG:
                        extImage = ConstantUtility.ExtImage.VIDEO;
                        break;
                    case ConstantUtility.Extensions.MPG:
                        extImage = ConstantUtility.ExtImage.VIDEO;
                        break;
                    case ConstantUtility.Extensions.PDF:
                        extImage = ConstantUtility.ExtImage.PDF;
                        break;
                    case ConstantUtility.Extensions.PPT:
                        extImage = ConstantUtility.ExtImage.PPT;
                        break;
                    case ConstantUtility.Extensions.PPTX:
                        extImage = ConstantUtility.ExtImage.PPT;
                        break;
                    case ConstantUtility.Extensions.SQL:
                        extImage = ConstantUtility.ExtImage.SQL;
                        break;
                    case ConstantUtility.Extensions.TEXT:
                        extImage = ConstantUtility.ExtImage.TEXT;
                        break;
                    case ConstantUtility.Extensions.WAV:
                        extImage = ConstantUtility.ExtImage.AUDIO;
                        break;
                    case ConstantUtility.Extensions.WMV:
                        extImage = ConstantUtility.ExtImage.VIDEO;
                        break;
                    case ConstantUtility.Extensions.XLS:
                        extImage = ConstantUtility.ExtImage.XML;
                        break;
                    case ConstantUtility.Extensions.XLSX:
                        extImage = ConstantUtility.ExtImage.XML;
                        break;
                    case ConstantUtility.Extensions.XML:
                        extImage = ConstantUtility.ExtImage.XML;
                        break;
                    case ConstantUtility.Extensions.Zip:
                        extImage = ConstantUtility.ExtImage.Zip;
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
            return extImage;
        }

        /// <summary> 
        /// Get all the files of all the users
        /// </summary>
        /// <param name="numRows">number of rows in a page</param>
        /// <param name="pageNumber">current page number</param>
        /// <param name="sidx">sort column</param>
        /// <param name="sord">sort order</param>
        /// <param name="userId">Id of the user if all the files are to be loaded Id is passed as null</param>
        /// <param name="searchOper">the search operation to be done.</param>
        /// <param name="searchString">The term to be searched</param>
        /// <param name="searchField">The column to be searched for</param>
        /// <returns>returns json format list of all the files</returns>
        public string GetAllFiles(int numRows, int pageNumber, string sidx, string sord, string userId, 
                                  string searchOper, string searchString, string searchField)
        {
            string result = string.Empty;
            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                string colName = GetColumnName(sidx);
                searchOper = GetOperator(searchOper);
                searchField = GetColumnName(searchField);
                int id = (!string.IsNullOrEmpty(userId) ? Convert.ToInt32(userId) : 0);

                DataAccessLayer objData = new DataAccessLayer();
                DataTable dtFiles;
                dtFiles = objData.GetAllFilesForAdmin(id, searchField, searchOper, searchString);

                DataTable dtOut = null;
                dtFiles.DefaultView.Sort = colName + " " + sord;
                dtOut = dtFiles.DefaultView.ToTable();

                var fileField = dtOut.AsEnumerable().Select(i => new FileGridColumns
                {
                    id = i.Field<int>(ConstantUtility.FileId),
                    fileName = i.Field<string>(ConstantUtility.OriginalFileName),
                    uploadedBy = i.Field<string>(ConstantUtility.UploadedBy),
                    uploadedOn = i.Field<string>(ConstantUtility.UploadedDate),
                    size = i.Field<decimal>(ConstantUtility.FileSize),
                    status = i.Field<string>(ConstantUtility.Status)
                }).AsQueryable();

                //Get total count of records
                int totalCount = dtFiles.Rows.Count;
                //Get total page of records
                int totalPages;
                int.TryParse(Convert.ToString(Math.Floor((decimal)(totalCount / numRows) + 1)), out totalPages);

                result = JsonConvert.SerializeObject(new GridProperties<FileGridColumns>
                {
                    rows = fileField.Skip((pageNumber - 1) * numRows).Take(numRows).ToList(),
                    records = totalCount,
                    total = totalPages,
                    page = pageNumber
                });
            }

            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }

            return result;
        }

        /// <summary>
        /// Deletes multiple files selected by the admin
        /// </summary>
        /// <param name="selectedRows">multiple file ids separated by a comma</param>
        public void DeleteFile(string selectedRows)
        {
            try
            {
                string[] selectedIds = selectedRows.Split(',');

                foreach (string id in selectedIds)
                {
                    DeleteFile(Convert.ToInt32(id), true);
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
        }

        #endregion

        #region UploaderService

        /// <summary>
        /// Changes the file password of the selected locked file
        /// </summary>
        /// <param name="newPassword">New password</param>
        /// <param name="oldPassword">old password textbox value</param>
        /// <param name="filePassword">existing password</param>
        /// <param name="fileId">File id of the locked file</param>
        /// <returns>bool value indicating whether the password has been reset or not</returns>
        public bool ChangeFilePassword(string newPassword, string oldPassword, string filePassword, string fileId)
        {
            bool isPwdChanged = false;
            try
            {
                filePassword = IGroupUtility.Decrypt(filePassword);

                //Check if the current pwd provided in the txtbox matches with the current pwd.
                if (string.Equals(oldPassword, filePassword))
                {
                    newPassword = IGroupUtility.Encrypt(newPassword);
                    DataAccessLayer objData = new DataAccessLayer();

                    objData.ChangeFilePassword(Convert.ToInt32(fileId), newPassword);
                    isPwdChanged = true;
                }
            }
            catch (Exception Ex)
            {
                ErrorUtility.WriteError(Ex);
                isPwdChanged = false;
            }
            return isPwdChanged;
        }

        /// <summary>
        /// Unlocks the locked file
        /// </summary>
        /// <param name="fileId">file id of the locked file</param>
        public void UnlockFile(int fileId)
        {
            try
            {
                DataAccessLayer objData = new DataAccessLayer();
                objData.UnlockFile(fileId);
            }
            catch (Exception Ex)
            {
                ErrorUtility.WriteError(Ex);
            }
        }

        /// <summary>
        /// returns the file password of the locked file
        /// </summary>
        /// <param name="fileId">id of the locked file</param>
        /// <returns></returns>
        public string ViewPassword(int fileId)
        {
            string filePassword = string.Empty;
            try
            {
                DataAccessLayer objData = new DataAccessLayer();
                filePassword = IGroupUtility.Decrypt(objData.GetFilePassword(fileId));
            }
            catch (Exception Ex)
            {
                ErrorUtility.WriteError(Ex);
            }
            return filePassword;
        }

        /// <summary>
        /// Gets the name of the file 
        /// </summary>
        /// <param name="fileId">Id of the file whose name is needed</param>
        /// <returns>name of the file</returns>
        public string GetFileName(int fileId)
        {
            DataAccessLayer objData = new DataAccessLayer();
            string fileName = string.Empty;

            try
            {UploadedFile objFileDetails = new UploadedFile();

                objFileDetails = objData.GetFileName(fileId);

                if (objFileDetails != null)
                {
                    fileName = objFileDetails.NewFileName;
                }
            }
            catch (Exception Ex)
            {
                ErrorUtility.WriteError(Ex);
            }
            return fileName;
        }
        /// <summary>
        /// Locks the individual file adn sets a password too
        /// </summary>
        /// <param name="filePassword">file password of the file</param>
        /// <param name="fileId">Id of the file</param>
        /// <returns>a bool value to indicate whether the file is locked or not</returns>
        public bool LockFile(string filePassword, int fileId)
        {
            try
            {
                DataAccessLayer ObjData = new DataAccessLayer();

                //Encrypt the password before saving it.
                filePassword = IGroupUtility.Encrypt(filePassword);
                ObjData.LockFile(filePassword, fileId);
            }
            catch (Exception Ex)
            {
                ErrorUtility.WriteError(Ex);
            }
            return true;
        }

        /// <summary>
        /// Chnages the name or title of an
        /// individual file
        /// </summary>
        /// <param name="newFileName">new name of the file</param>
        /// <param name="fileId">Id of the file.</param>
        /// <returns>a bool value indicating whether the name is changed or not</returns>
        public bool ChangeName(string newFileName, int fileId)
        {
            try
            {
                DataAccessLayer objData = new DataAccessLayer();
                objData.ChangeTitle(newFileName, Convert.ToInt32(fileId));
            }
            catch (Exception Ex)
            {
                ErrorUtility.WriteError(Ex);
            }
            return true;
        }

        /// <summary>
        /// Generates the url for downloading the particular file
        /// </summary>
        /// <param name="fileId">file id of the fiel to be downloaded</param>
        /// <returns>url for download</returns>
        //public string GetTinyFileUrl(string fileId)
        //{
        //    string downloadUrl = string.Empty;
        //    string tinyUrl = string.Empty;

        //    try
        //    {
        //        fileId = IGroupUtility.Encrypt(fileId);
        //        fileId = HttpUtility.UrlEncode(fileId);
        //        string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
        //                                                   HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
        //        downloadUrl = baseUrl + "DownloadFile.aspx?FileId=" + fileId;

        //        var address = new System.Uri("http://tinyurl.com/api-create.php?url=" + downloadUrl);
        //        var client = new System.Net.WebClient();
        //        tinyUrl = client.DownloadString(address);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorUtility.WriteError(ex);
        //    }
        //    return tinyUrl;
        //}

        /// <summary>
        /// Generates the url for downloading the particular file
        /// </summary>
        /// <param name="fileId">file id of the fiel to be downloaded</param>
        /// <returns>url for download</returns>
        public string SaveTinyUrl(string fileId)
        {
            string downloadUrl = string.Empty;
            string tinyUrl = string.Empty;
            string decodedFileId = IGroupUtility.Decrypt(fileId);

            try
            {
                fileId = HttpUtility.UrlEncode(fileId);
                string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                                                           HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";

                downloadUrl = baseUrl + "DownloadHandler.aspx?FileId=" + fileId;
               // downloadUrl = baseUrl + "DownloadFile.aspx?FileId=" + fileId;

                var address = new System.Uri("http://tinyurl.com/api-create.php?url=" + downloadUrl);
                var client = new System.Net.WebClient();
                tinyUrl = client.DownloadString(address);
                DataAccessLayer objData = new DataAccessLayer();
                objData.SaveTinyUrl(tinyUrl,  Convert.ToInt32(decodedFileId));
                
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
            return tinyUrl;
        }

        /// <summary>
        /// Generates the url for downloading the particular file
        /// </summary>
        /// <param name="fileId">file id of the fiel to be downloaded</param>
        /// <returns>url for download</returns>
        public string GetFileUrl(string fileId)
        {
            string downloadUrl = string.Empty;

            try
            {
                fileId = IGroupUtility.Encrypt(fileId);
                fileId = HttpUtility.UrlEncode(fileId);
                string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                                                           HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
                downloadUrl = baseUrl + "DownloadHandler.aspx?FileId=" + fileId;
                //downloadUrl = baseUrl + "DownloadFile.aspx?FileId=" + fileId;
                
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }

            return downloadUrl;
        }

       /// <summary>
        /// Resets the password of the registered user
       /// </summary>
       /// <param name="userHashCode"></param>
       /// <param name="currentPassword"></param>
       /// <param name="newPassword"></param>
       /// <returns></returns>
        public bool ResetPassword(string userHashCode, string currentPassword, string newPassword)
        {
            bool isSuccess = false;

            try
            {
                if (!string.IsNullOrEmpty(userHashCode))
                {
                    DataAccessLayer objData = new DataAccessLayer();
                    isSuccess = objData.ResetPassword(userHashCode, IGroupUtility.Encrypt(currentPassword),IGroupUtility.Encrypt(newPassword));
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
            return isSuccess;
        }

        /// <summary>
        /// Gets the details of all the registered users
        /// </summary>
        /// <param name="numRows">No. of rows to be displayed</param>
        /// <param name="pageNumber">page number</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sorting order</param>
        /// <param name="searchOperator">the search operation to be done</param>
        /// <param name="searchTerm">the term to be searched for</param>
        /// <returns>json format string <returns>
        public string GetAllUsersDetail(int numRows, int pageNumber, string sidx, string sord,
                                        string searchOperator, string searchTerm)
        {
            string result = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(searchOperator))
                {
                    searchOperator = GetOperator(searchOperator); 
                }

                BusinessLogic objBusiness = new BusinessLogic();

                DataAccessLayer objData = new DataAccessLayer();
                DataTable dtFiles;
                dtFiles = objData.GetAllUsersDetail(searchOperator, searchTerm);

                DataTable dtOut = null;
                dtFiles.DefaultView.Sort = sidx + " " + sord;
                dtOut = dtFiles.DefaultView.ToTable();

                var fileField = dtOut.AsEnumerable().Select(i => new Uploader.Entities.RegisteredUsers
                {
                    UserId=i.Field<int>(ConstantUtility.UserId),
                    UserName = i.Field<string>(ConstantUtility.UserName),
                    AllocatedSpace = i.Field<decimal?>(ConstantUtility.AllocatedSpace),
                    UsedSpace = i.Field<decimal?>(ConstantUtility.UsedSpace),
                    FreeSpace = i.Field<decimal?>(ConstantUtility.FreeSpace),
                    TotalFiles = i.Field<string>(ConstantUtility.TotalFiles),
                    LastLogin = i.Field<string>(ConstantUtility.LastLogin)
                }).AsQueryable();

                //Get total count of records
                int totalCount = dtFiles.Rows.Count;
                //Get total page of records
                int totalPages;
                int.TryParse(Convert.ToString(Math.Floor((decimal)(totalCount / numRows) + 1)), out totalPages);

                result = JsonConvert.SerializeObject(new GridProperties<Uploader.Entities.RegisteredUsers>
                {
                    rows = fileField.Skip((pageNumber - 1) * numRows).Take(numRows).ToList(),
                    records = totalCount,
                    total = totalPages,
                    page = pageNumber
                });
            }

            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }

            return result;
        }

        /// <summary>
        /// method for updating the total allocated space
        /// for a particular user.
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="colValue">new allocated space for the user</param>
        /// <returns>boolean value for indication whether the updation was successful or not</returns>
        public bool UpdateTotalSpace(int userId, string colValue)
        {
            try
            {
                DataAccessLayer objData = new DataAccessLayer();
                return objData.UpdateTotalSpace(userId, colValue);
            }

            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Deletes multiple files selected by the admin
        /// </summary>
        /// <param name="selectedRows">multiple file ids separated by a comma</param>
        public void DeleteFiles(string selectedRows)
        {
            try
            {
                string[] selectedIds = selectedRows.Split(',');

                foreach (string id in selectedIds)
                {
                    DeleteFile(Convert.ToInt32(id), false);
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// Return the owner of the File As per FileID
        /// </summary>
        /// <param name="fileID">id of the File</param>
        /// <returns></returns>
        public DataTable GetFileOwnerAndDownloadHistory(int fileID)
        {
            try
            {
                var objData = new DataAccessLayer();
                return objData.GetFileOwnerAndDownloadHistory(fileID);
            }
            catch 
            {
                return null;
            }
        }

        #endregion
    }
}
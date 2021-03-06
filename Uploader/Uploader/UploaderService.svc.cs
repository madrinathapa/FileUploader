﻿//--------------------------------------------------------------------------------
// FileName:UploaderService.svc.cs
// Description:This file contains the definition for all the service contracts
// Created By:Madrina Thapa
//--------------------------------------------------------------------------------

using Uploader.Business;
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Uploader
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UploaderService
    {
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";

        /// <summary>
        /// Resets the password of the registered user
        /// </summary>
        /// <param name="userHashCode">a unique user id</param>
        /// <param name="currentPassword">current password of the user</param>
        /// <param name="newPassword">new password</param>
        /// <returns>a bool value</returns>
        [OperationContract]
        public bool ResetPassword(string userHashCode, string currentPassword, string newPassword)
        {
            bool isSuccess = false;

            try
            {
                if (!string.Equals(userHashCode, string.Empty))
                {
                    BusinessLogic objBusiness = new BusinessLogic();
                    isSuccess = objBusiness.ResetPassword(userHashCode, currentPassword, newPassword);
                }
            }
            catch (Exception Ex)
            {
                ErrorUtility.WriteError(Ex);
            }

            return isSuccess;
        }

        /// <summary>
        /// Changing the name of the file
        /// </summary>
        /// <param name="newFileName">New title of the file</param>
        /// <param name="fileId">Id of the file</param>
        /// <returns>A bool value indicated whether the title was changes successfully or not</returns>
        [OperationContract]
        public bool ChangeName(string newFileName, string fileId)
        {
            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                objBusiness.ChangeName(newFileName, Convert.ToInt32(fileId));
            }
            catch (Exception Ex)
            {
                ErrorUtility.WriteError(Ex);
            }
            return true;
        }

        /// <summary>
        /// Locks the individual file adn sets a password too
        /// </summary>
        /// <param name="filePassword">file password of the file</param>
        /// <param name="fileId">Id of the file</param>
        /// <returns>a bool value to indicate whether the file is locked or not</returns>
        [OperationContract]
        public bool LockFile(string filePassword, string fileId)
        {
            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                objBusiness.LockFile(filePassword, Convert.ToInt32(fileId));
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }
            return true;
        }

        /// <summary>
        /// returns the file password of the locked file
        /// </summary>
        /// <param name="fileId">id of the locked file</param>
        /// <returns></returns>
        [OperationContract]
        public string ViewPassword(string fileId)
        {
            string filePassword = null;

            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                filePassword = objBusiness.ViewPassword(Convert.ToInt32(fileId));
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }
            return filePassword;
        }

        [OperationContract]
        public string GetFileName(int fileId)
        {
            string fileName = null;

            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                fileName = objBusiness.GetFileName(Convert.ToInt32(fileId));
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }
            return fileName;
        }

        /// <summary>
        /// Unlocks the locked file
        /// </summary>
        /// <param name="fileId">file id of the locked file</param>
        [OperationContract]
        public void UnlockFile(string fileId)
        {
            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                objBusiness.UnlockFile(Convert.ToInt32(fileId));
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Changes the file password of the selected locked file
        /// </summary>
        /// <param name="newPassword">New password</param>
        /// <param name="oldPassword">old password textbox value</param>
        /// <param name="filePassword">existing password</param>
        /// <param name="fileId">File id of the locked file</param>
        /// <returns>bool value indicating whether the password has been reset or not</returns>
        [OperationContract]
        public bool ChangeFilePassword(string newPassword, string oldPassword, string filePassword, string fileId)
        {
            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                return objBusiness.ChangeFilePassword(newPassword, oldPassword, filePassword, fileId);
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                return false;
            }
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
        [OperationContract]
        public string GetAllFiles(int numRows, int pageNumber, string sidx, string sord,
                                 string userId, string searchOper, string searchString, string searchField)
        {
            string result = string.Empty;

            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                result = objBusiness.GetAllFiles(numRows, pageNumber, sidx, sord, userId, searchOper, searchString, searchField);
            }

            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Deletes all the selected rows.
        /// </summary>
        /// <param name="selectedRows"></param>
        [OperationContract]
        public void DeleteFile(string selectedRows)
        {
            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                objBusiness.DeleteFile(selectedRows);
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }
        }


        /// <summary>
        /// Deletes the selected file.
        /// </summary>
        /// <param name="fileId">id of the file</param>
        /// <param name="fileName">name in which the file was saved</param>
        [OperationContract]
        public bool RemoveFile(string fileId)
        {
            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                return objBusiness.DeleteFile(Convert.ToInt32(fileId));
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }
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
        [OperationContract]
        public string GetAllUsersDetail(int numRows, int pageNumber, string sidx, string sord, string searchOper, string searchString)
        {
            string result = string.Empty;

            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                result = objBusiness.GetAllUsersDetail(numRows, pageNumber, sidx, sord, searchOper, searchString);
            }

            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Updates the total allocated space for the particular user
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="colValue">New allocated space</param>
        /// <returns>A bool valuse to indicate whether the udpdation was successful or not</returns>
        [OperationContract]
        public bool UpdateTotalSpace(string userId, string colValue)
        {
            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                return objBusiness.UpdateTotalSpace(Convert.ToInt32(userId), colValue);
            }

            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Deletes all the selected rows.
        /// </summary>
        /// <param name="selectedRows"></param>
        [OperationContract]
        public void DeleteFiles(string selectedRows)
        {
            try
            {
                BusinessLogic objBusiness = new BusinessLogic();
                objBusiness.DeleteFiles(selectedRows);
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }
        }

        [OperationContract]
        public string SaveTinyUrl(string fileId)
        {
            string tinyUrl = string.Empty;
            BusinessLogic objBusiness = null;
            try
            {
                objBusiness = new BusinessLogic();
                tinyUrl = objBusiness.SaveTinyUrl(fileId);
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
                throw ex;
            }
            finally
            {
                objBusiness = null;
            }
            return tinyUrl;
           
        }
        
    }
}
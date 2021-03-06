﻿//--------------------------------------------------------------------------------
// FileName:DataAccessLayer.cs
// Description:has methods for accessing database.
// Created By:Madrina Thapa.
//--------------------------------------------------------------------------------

using IGroup.Modules.Common.Utility;
using Uploader.Entities;
using Uploader.Utilities;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace Uploader.Data
{
    public class DataAccessLayer
    {
        private string ConnectionString
        {
            get
            {
                string connectionString = string.Empty;
                if (ConfigurationManager.ConnectionStrings["connString"] != null &&
                       !string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.ConnectionStrings["connString"])))
                {
                    connectionString = IGroupUtility.Decrypt((Convert.ToString(ConfigurationManager.ConnectionStrings["connString"])));
                }
                return connectionString;
            }
        }

        /// <summary>
        /// Get the user details from the login table
        /// i.e checks whether the user exists or not
        /// </summary>
        /// <param name="email">registered email id</param>
        /// <param name="password">password of the user</param>
        /// <returns>user type object with all the reqd. details </returns>
        public User GetUserDetails(string email, string password)
        {
            SqlDataReader reader = null;
            User user = null;

            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.GetUserDetails, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Adding all parameters
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.Email, email));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.Password, password));

                            dbConnection.Open();
                            reader = command.ExecuteReader();

                            //Read data using datareader
                            if (reader.HasRows && reader.Read())
                            {
                                user = new User();

                                if (reader[ConstantUtility.UserHashcode] != DBNull.Value)
                                {
                                    user.UserHashCode = Convert.ToString(reader[ConstantUtility.UserHashcode]);
                                }

                                if (reader[ConstantUtility.UserName] != DBNull.Value)
                                {
                                    user.UserName = Convert.ToString(reader[ConstantUtility.UserName]);
                                }

                                if (reader[ConstantUtility.IsAdmin] != DBNull.Value)
                                {
                                    user.IsAdmin = Convert.ToBoolean(reader[ConstantUtility.IsAdmin]);
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //log the error
                ErrorUtility.WriteError(ex);
            }

            finally
            {
                reader.Close();
            }
            return user;
        }

        /// <summary>
        /// For saving the details of the files that are uploaded by the user to the server
        /// </summary>
        /// <param name="originalFileNames">Name of the file that is to be uploaded</param>
        /// <param name="savedFileNames">Original name of the file followed by the Guid</param>
        /// <param name="userHashCode">unique user id.</param>
        /// <param name="machineName">Name of the machine used</param>
        /// <param name="ipAddress">IP address of the machine used</param>
        /// <param name="fileSize">total size of the file</param>
        /// <param name="extension">extension of the file being uploaded</param>
        /// <param name="thumbFileName">thumbnail image's name</param>
        /// <returns>file id of the file uploaded</returns>
        public int SaveFileDetails(string originalFileNames, string savedFileNames, string userHashCode,
                                   string machineName, string ipAddress, double fileSize,
                                   string extension, string thumbFileName)
        {
            int fileId = 0;

            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.SaveFileDetail, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding all parameters
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.OriginalFileName, originalFileNames));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.SavedFileName, savedFileNames));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.UserHashCode, userHashCode));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.MachineName, machineName));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.IpAddress, ipAddress));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileSize, fileSize));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.Extension, extension));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.ThumbFileName, thumbFileName));

                            dbConnection.Open();

                            //Get the file id.
                            var result = command.ExecuteScalar();

                            if (result != null)
                            {
                                int.TryParse(Convert.ToString(result), out fileId);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //log the error
                ErrorUtility.WriteError(ex);
            }

            return fileId;
        }

        /// <summary>
        /// Save Tiny URL against the file
        /// </summary>
        /// <param name="tinyUrl"></param>
        /// <param name="fileId"></param>
        public void SaveTinyUrl(string tinyUrl, int fileId)
        {
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString) && !string.IsNullOrEmpty(tinyUrl))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.SaveTinyUrl, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding all parameters
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileId, fileId));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.TinyUrl, tinyUrl));
                            dbConnection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //log the error
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// gets all the files uploaded by the user.
        /// </summary>
        /// <param name="userHashCode">unique user Id</param>
        /// <param name="isAdmin"></param>
        /// <param name="isSearchable"></param>
        /// <param name="searchTerm"></param>
        /// <returns>original name and saved name of the files uploaded by the user.</returns>
        public DataTable GetAllFiles(string userHashCode, int isAdmin, int isSearchable, string searchTerm)
        {
            DataTable fileNames = new DataTable();

            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.GetFileDetails, connection))
                        {
                            connection.Open();
                            command.CommandType = CommandType.StoredProcedure;

                            // Adding parameters
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.UserHashCode, userHashCode));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.IsAdmin, isAdmin));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.IsSearchable, isSearchable));

                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.SearchTerm, searchTerm));
                            }

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    fileNames.Load(reader);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }

            return fileNames;
        }

        /// <summary>
        /// Deletes the particular file from db
        /// </summary>
        /// <param name="fileId">Unique file Id of the file to be deleted</param>
        /// <param name="isAdmin">whether the user is admin or not.</param>
        /// <returns>returns whether the update was successful or not</returns>
        public bool DeleteFile(int fileId, bool isAdmin)
        {
            bool isSuccess = false;
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.DeleteFile, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileId, fileId));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.IsAdmin, isAdmin));

                            dbConnection.Open();

                            var result = command.ExecuteScalar();
                            int recordsAffected = 0;

                            if (result != null)
                            {
                                if (int.TryParse(result.ToString(), out recordsAffected) && recordsAffected > 0)
                                {
                                    isSuccess = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
            return isSuccess;
        }

        /// <summary>
        /// Returns the name of the file to be downloaded
        /// </summary>
        /// <param name="isDeleted">Whether the file is deleted or not</param>
        /// <param name="isLocked">whether the file is locked or not</param>
        /// <param name="filePassword">Filepassword of the locked file</param>
        /// <param name="fileId">fileid of the file to be downloaded</param>
        public UploadedFile DownloadFile(int fileId)
        {
            UploadedFile file = null;
            SqlDataReader reader = null;

            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.FileDownload, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileId, fileId));
                            dbConnection.Open();
                            reader = command.ExecuteReader();

                            //Read data using datareader
                            if (reader.HasRows && reader.Read())
                            {
                                file = new UploadedFile();

                                if (reader[ConstantUtility.IsDeleted] != DBNull.Value)
                                {
                                   file.IsDeleted = Convert.ToBoolean(reader[ConstantUtility.IsDeleted]);
                                }

                                if (reader[ConstantUtility.FileStatus] != DBNull.Value)
                                {
                                    file.IsLocked = Convert.ToBoolean(reader[ConstantUtility.FileStatus]);
                                }

                                if (reader[ConstantUtility.IsAdminDelete] != DBNull.Value)
                                {
                                    file.IsAdminDeleted = Convert.ToBoolean(reader[ConstantUtility.IsAdminDelete]);
                                }

                                if (reader[ConstantUtility.OriginalFileName] != DBNull.Value)
                                {
                                    file.OriginalFileName = Convert.ToString(reader[ConstantUtility.OriginalFileName]);
                                }

                                if (reader[ConstantUtility.SavedFileName] != DBNull.Value)
                                {
                                    file.SavedFileName = Convert.ToString(reader[ConstantUtility.SavedFileName]);
                                }

                                //Check if the file password exists or not.
                                if (reader[ConstantUtility.FilePassword] != DBNull.Value)
                                {
                                    file.FilePassword = IGroupUtility.Decrypt(Convert.ToString(reader[ConstantUtility.FilePassword]));
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }

            finally
            {
                if (reader != null)
                {
                    ((IDisposable)reader).Dispose();
                }
            }
            return file;
        }

        /// <summary>
        /// After the download process is over,
        /// downloaded files log is updated.
        /// </summary>
        /// <param name="fileId">id of the file downloaded</param>
        public void DownloadFileLog(int fileId)
        {
            string machineName = string.Empty;
            string ipAddress = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    machineName = System.Environment.MachineName;//Get the machine name
                    ipAddress = HttpContext.Current.Request.UserHostAddress; // Get the IP

                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.FileDownloadLog, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Adding all parameters
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileId, fileId));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.MachineName, machineName));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.IpAddress, ipAddress));

                            dbConnection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// gets the number of file uploaded and downloaded
        /// by the user.
        /// </summary>
        /// <param name="userHashCode">Unique user code</param>
        public UserAccount GetAccountDetails(string userHashCode)
        {
            UserAccount objAccDetails = null;
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.GetAccountDetails, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.UserHashCode, userHashCode));

                            dbConnection.Open();
                            SqlDataReader reader = command.ExecuteReader();

                            //Read data using datareader
                            if (reader.HasRows && reader.Read())
                            {
                                objAccDetails = new UserAccount();
                                objAccDetails.TotalFiles = Convert.ToString(reader[ConstantUtility.TotalFiles]);
                                objAccDetails.TotalDownloads = Convert.ToString(reader[ConstantUtility.TotalDownloads]);
                                objAccDetails.TotalSize = Convert.ToString(reader[ConstantUtility.TotalSize]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
            return objAccDetails;
        }

        /// <summary>
        /// Returns all the files for the admin
        /// </summary>
        /// <param name="userId">Id of the user if all the files are to be loaded then Id is passed as 0</param>
        /// <param name="searchField">Optional parameter will be null if all the files are to be loaded</param>
        /// <param name="searchOper">the search operation to be done</param>
        /// <param name="searchString">The term to be searched for</param>
        /// <returns>list of files and details satisfying the condition</returns>
        public DataTable GetAllFilesForAdmin(int userId, string searchField, string searchOper, string searchString)
        {
            DataTable allFileDetails = new DataTable();
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.GetAllFilesForAdmin, connection))
                        {
                            connection.Open();
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.UserId, userId));

                            if (!string.IsNullOrEmpty(searchField))
                            {
                                command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.SearchField, searchField));
                                command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.SearchOper, searchOper));
                                command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.SearchString, searchString));
                            }

                            using (SqlDataAdapter adapter = new SqlDataAdapter())
                            {
                                adapter.SelectCommand = command;
                                adapter.Fill(allFileDetails);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
            return allFileDetails;
        }

        /// <summary>
        /// Checks whether a user with the email exists or not
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool GetUserByEmail(string email)
        {
            bool isUserExist = false;

            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.GetUserByEmail, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.Email, email));

                            dbConnection.Open();
                            var result = command.ExecuteScalar();

                            if (result != null)
                            {
                                bool.TryParse(Convert.ToString(result), out isUserExist);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
            return isUserExist;
        }

        /// <summary>
        /// Saves the user details and returns the hashcode
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool SaveUserDetails(string firstName, string lastName, string email, string password)
        {
            bool isSuccess = false;
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.SaveUserDetail, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FirstName, firstName));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.LastName, lastName));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.Email, email));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.Password, password));

                            dbConnection.Open();

                            //Get the number of row affected.
                            var result = command.ExecuteScalar();
                            int recordsAffected = 0;

                            if (result != null)
                            {
                                if (int.TryParse(result.ToString(), out recordsAffected) && recordsAffected > 0)
                                {
                                    isSuccess = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //log the error
                ErrorUtility.WriteError(ex);
            }
            return isSuccess;
        }

        /// <summary>
        /// Resets the password of the current user.
        /// </summary>
        /// <param name="userHashCode">Unique user hashcode.</param>
        /// <param name="currentPassword">Existing password</param>
        /// <param name="newPassword">new password</param>
        /// <returns></returns>
        public bool ResetPassword(string userHashCode, string currentPassword, string newPassword)
        {
            bool isReset = false;

            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.ResetPassword, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.UserHashCode, userHashCode));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.CurrentPassword, currentPassword));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.NewPassword, newPassword));

                            dbConnection.Open();
                            var status = command.ExecuteScalar();

                            if (status != null)
                            {
                                bool.TryParse(status.ToString(), out isReset);
                            }
                        }
                    }
                }
            }

            catch (SqlException ex)
            {
                //log the error
                ErrorUtility.WriteError(ex);
            }
            return isReset;
        }

        /// <summary>
        /// Sets the new title of the file.
        /// </summary>
        /// <param name="newFileName">New title</param>
        /// <param name="fileId">Unique file id.</param>
        public void ChangeTitle(string newFileName, int fileId)
        {
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.ChangeFileName, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.NewFileName, newFileName));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileId, fileId));

                            dbConnection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// Locks the individual file,
        /// i.e sets the password.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="fileId"></param>
        public void LockFile(string password, int fileId)
        {
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.LockFile, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.Password, password));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileId, fileId));

                            dbConnection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// This method is used for unlocking the file
        /// </summary>
        /// <param name="fileId">Id of the file to be unlocked</param>
        public void UnlockFile(int fileId)
        {
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.UnlockFile, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileId, fileId));

                            dbConnection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// Method to change the password of a locked file
        /// </summary>
        /// <param name="fileId">Id of the file </param>
        /// <param name="newPassword">New password</param>
        public void ChangeFilePassword(int fileId, string newPassword)
        {
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.ChangeFilePassword, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileId, fileId));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.NewPassword, newPassword));

                            dbConnection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //log the error
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// Gets the password of the locked file
        /// </summary>
        /// <param name="fileId">Id of the file</param>
        /// <returns></returns>
        public string GetFilePassword(int fileId)
        {
            string filePassword = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.GetFilePassword, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileId, fileId));
                            dbConnection.Open();
                            SqlDataReader reader = command.ExecuteReader();

                            //Read data using datareader
                            if (reader.HasRows && reader.Read())
                            {
                                if (reader[ConstantUtility.FilePassword] != DBNull.Value)
                                {
                                    filePassword = Convert.ToString(reader[ConstantUtility.FilePassword]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
            return filePassword;
        }

        /// <summary>
        /// Gets the name of the file using file id
        /// </summary>
        /// <param name="fileId">id of the file whose name is required</param>
        /// <returns>Name of the file</returns>
        public UploadedFile GetFileName(int fileId)
        {
            UploadedFile objFileDetails = null;

            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.GetFileName, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileId, fileId));
                            dbConnection.Open();
                            SqlDataReader reader = command.ExecuteReader();

                            //Read data using datareader
                            if (reader.HasRows && reader.Read())
                            {
                                objFileDetails = new UploadedFile();

                                if (reader[ConstantUtility.NewFileName] != DBNull.Value)
                                {
                                    objFileDetails.NewFileName = Convert.ToString(reader[ConstantUtility.NewFileName]);
                                }

                                if (reader[ConstantUtility.SavedFileName] != DBNull.Value)
                                {
                                    objFileDetails.SavedFileName = Convert.ToString(reader[ConstantUtility.SavedFileName]);
                                }

                                if (reader[ConstantUtility.ThumbName] != DBNull.Value)
                                {
                                    objFileDetails.ThumbName = Convert.ToString(reader[ConstantUtility.ThumbName]);
                                }

                                if (reader[ConstantUtility.Exe] != DBNull.Value)
                                {
                                    objFileDetails.Extension = Convert.ToString(reader[ConstantUtility.Exe]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
            return objFileDetails;
        }
        /// <summary>
        /// Gets the user details using hashcode
        /// </summary>
        /// <param name="userHashCode">user's unique hashcode</param>
        /// <returns></returns>
        public User GetUserByHashCode(string userHashCode)
        {
            User objUser = null;

            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.GetUserDetailsByHashCode, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.UserHashCode, userHashCode));

                            dbConnection.Open();
                            SqlDataReader reader = command.ExecuteReader();

                            //Read data using datareader
                            if (reader.HasRows && reader.Read())
                            {
                                objUser = new User();
                                objUser.EmailId = Convert.ToString(reader[ConstantUtility.Email]);
                                objUser.IsAdmin = Convert.ToBoolean(reader[ConstantUtility.IsAdmin]);
                                objUser.UserName = Convert.ToString(reader[ConstantUtility.UserName]);
                                objUser.UserHashCode = userHashCode;
                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
            return objUser;
        }

        /// <summary>
        /// gets all the registered user's details.
        /// </summary>    
        /// <param name="searchOperator">the optional param it tells the search operation to be done</param>
        /// <param name="searchTerm">the term to be searched for</param>
        /// <returns>datatable of all the users record</returns>
        public DataTable GetAllUsersDetail(string searchOperator, string searchTerm)
        {
            DataTable registeredUsers = new DataTable();
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.GetAllUserDetails, connection))
                        {
                            connection.Open();
                            command.CommandType = CommandType.StoredProcedure;
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.SearchTerm, searchTerm));
                                command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.SearchOper, searchOperator));
                            }
                            registeredUsers.Load(command.ExecuteReader());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
            return registeredUsers;
        }

        /// <summary>
        /// updates the allocated space for a particular user
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="allocatedSpace">New allocated space</param>
        /// <returns>boolean value indication whether the updation was successful or not</returns>
        public bool UpdateTotalSpace(int userId, string allocatedSpace)
        {
            bool isUpdated = false;
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.UpdateAllocatedSpace, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            //Adding parameter
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.UserId, userId));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.AllocatedSpace, allocatedSpace));

                            dbConnection.Open();
                            var status = command.ExecuteScalar();

                            if (status != null)
                            {
                                bool.TryParse(status.ToString(), out isUpdated);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
            return isUpdated;
        }

        /// <summary>
        /// Save the session data in the session table
        /// </summary>
        /// <param name="userHashcode">unique code of the user</param>
        /// <param name="sessionData">encrypted cookie value</param>
        public void SaveSessionData(string userHashcode, string sessionData)
        {
            string machineName = string.Empty;
            string ipAddress = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    machineName = System.Environment.MachineName;//Get the machine name
                    ipAddress = HttpContext.Current.Request.UserHostAddress; // Get the IP

                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.SaveSessionDetails, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Adding all parameters
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.UserHashCode, userHashcode));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.IpAddress, ipAddress));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.MachineName, machineName));
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.SessionData, sessionData));

                            dbConnection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //Log the error
                ErrorUtility.WriteError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public DataTable GetFileOwnerAndDownloadHistory(int fileId)
        {

            DataTable dt = new DataTable(); ;
            try
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(ConstantUtility.StoredProcedures.GetFileOwnerAndDownloadHistory, dbConnection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter(ConstantUtility.StoredProcedureParams.FileId, fileId));

                            dbConnection.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    dt.Load(reader);
                                }
                            }
                          //  ds = command.ExecuteNonQuery();

                            //dt.Load(command.ExecuteReader(),LoadOption.OverwriteChanges);

                            
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorUtility.WriteError(ex);
                dt = null;
            }
            return dt;
 
        }

    }
}
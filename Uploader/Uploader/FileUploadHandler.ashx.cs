//--------------------------------------------------------------------------------
// FileName:FileUploadHandler.ashx.cs
// Description:Uploads files and Images and resizes when the image is uploaded.
//             Saves the Images and thumbnails of original image.
// Created By:Madrina Thapa.
//--------------------------------------------------------------------------------

using IGroup.Modules.Common.Utility;
using Uploader.Business;
using Uploader.Data;
using Uploader.Entities;
using Uploader.Utilities;
using ImageResizer;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace Uploader
{
    /// <summary>
    /// Summary description for FileUploadHandler
    /// </summary>
    public class FileUploadHandler : IHttpHandler, IRequiresSessionState
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
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string machineName = System.Environment.MachineName;
                string ipAddress = HttpContext.Current.Request.UserHostAddress;

                int fileId = 0;
                string fileName = string.Empty;
                string originalFileName = string.Empty;
                string savedFileName = string.Empty;
                string thumbFileName = null;
                string extension = string.Empty;
                double fileSize = 0.0;

                BusinessLogic objBusiness = new BusinessLogic();

                //if there is more than one file, zip it before uploading it
                if (context.Request.Files.Count > 1)
                {
                    HttpFileCollection files = context.Request.Files;
                    savedFileName = ZipFiles(files, out fileSize, out originalFileName, out extension);
                    thumbFileName = objBusiness.ExtIconName(extension);
                }
                else if (context.Request.Files.Count == 1)
                {
                    HttpFileCollection files = context.Request.Files;

                    foreach (string key in files)
                    {
                        HttpPostedFile file = files[key];
                        string guid = Guid.NewGuid().ToString();
                        fileName = file.FileName;

                        //Get the index of the last dot(.)
                        int indexOfExtension = fileName.LastIndexOf(".");

                        //Get the file extension
                        string fileExtension = fileName.Substring(indexOfExtension);

                        //Get the file name with out extension
                        string fileNameWithoutExtension = fileName.Substring(0, (fileName.Length - fileExtension.Length));

                        //Concat the file name with new guid
                        fileName = string.Concat(fileNameWithoutExtension, "_", guid, fileExtension);

                        //Get the new thumb file name
                        thumbFileName = string.Concat(fileNameWithoutExtension, "_", guid, "_thumb", fileExtension);
                        savedFileName = fileName;

                        //Get the physical path, where the file will be saved.
                        fileName = System.IO.Path.Combine(IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.FolderPath]), fileName);

                        extension = Path.GetExtension(fileName).Trim().ToLower();
                        originalFileName = file.FileName;

                        //saving the file with a unique name.
                        file.SaveAs(fileName);
                        fileSize = ((double)file.ContentLength / 1024);
                    }
                    if (Convert.ToString(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.ImageExtensions]).Contains(extension))
                    {
                        ResizeImage(fileName, thumbFileName);
                    }
                    else
                    {
                        thumbFileName = objBusiness.ExtIconName(extension);
                    }
                }

                DataAccessLayer objDataAccess = new DataAccessLayer();

                if (objDataAccess != null)
                {
                    fileId = objDataAccess.SaveFileDetails(originalFileName, savedFileName, CurrentUser.Id,
                        machineName, ipAddress, fileSize, extension, thumbFileName);
                    string fileEncryptedId = IGroupUtility.Encrypt(fileId.ToString());

                    //saving the data in session state.
                    HttpContext.Current.Session["FileId"] = fileEncryptedId;
                }
                context.Response.Output.Write(ConstantUtility.True);
            }
            catch (Exception ex)
            {
                //Log error
                ErrorUtility.WriteError(ex);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// For zipping the multiple files into a single file
        /// </summary>
        /// <param name="files"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public string ZipFiles(HttpFileCollection files, out double fileSize, out string originalFileName,
            out string extension)
        {
            fileSize = 0.0;
            originalFileName = string.Empty;
            extension = string.Empty;
            string savedFileName = string.Empty;

            try
            {
                List<string> fileNames = new List<string>();
                extension = ConstantUtility.Extensions.Zip;

                //Saving the files to the server
                foreach (string key in files)
                {
                    HttpPostedFile file = files[key];

                    originalFileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.')) + ConstantUtility.Extensions.Zip;
                    savedFileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.')) + "_" + Guid.NewGuid().ToString() + ConstantUtility.Extensions.Zip;

                    fileNames.Add(file.FileName);

                    fileSize += (double)file.ContentLength / 1024;
                    string fileName = Path.Combine(IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.TempFolder]), file.FileName);

                    file.SaveAs(fileName);
                }

                //Zipping the multiple files
                using (ZipFile zip = new ZipFile())
                {

                    for (int i = 0; i < fileNames.Count; i++)
                    {
                        zip.AddFile(Path.Combine(IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.TempFolder]), fileNames[i]), string.Empty);
                    }
                    zip.Save(Path.Combine(IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.FolderPath]), savedFileName));
                }

                //Deleting the individual file from the server
                foreach (string name in fileNames)
                {
                    string filePath = Path.Combine(IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.TempFolder]), name);

                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                //Log error
                ErrorUtility.WriteError(ex);
            }
            return savedFileName;
        }

        /// <summary>
        /// This is used for resizing the images.
        /// </summary>
        /// <param name="originalFilepath"></param>
        /// <param name="thumbFileName"></param>
        public void ResizeImage(string originalFilepath, string thumbFileName)
        {
            Image FullsizeImage = null;
            try
            {
                int NewHeight = 1100;
                int NewWidth = 800;
                using (FullsizeImage = Image.FromFile(originalFilepath))
                {
                    // Prevent using images internal thumbnail
                    FullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    FullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

                    if (FullsizeImage.Height > 500)
                    {
                        NewHeight = 1100;
                        // Resize with height instead
                        NewWidth = FullsizeImage.Width * NewHeight / FullsizeImage.Height;
                    }
                }

                FullsizeImage.Dispose();
                Dictionary<string, string> Versions = new Dictionary<string, string>
                {
                    {"_Thumb", "width=96&height=72&format=png&dpi=150"}
                };

                string thumbPath = System.IO.Path.Combine(IGroupUtility.Decrypt(ConfigurationManager.AppSettings[ConstantUtility.ConfigKeys.ThumbImagePath]), thumbFileName);

                ImageJob ObjImageJobThumbnail = new ImageJob(originalFilepath, thumbPath, new Instructions(Versions["_Thumb"]));
                ObjImageJobThumbnail.Build();
            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }
        }
    }
}

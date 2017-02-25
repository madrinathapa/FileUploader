//****************************************************************************************
//FileName:ErrorUtility.cs
// Description:This page is used for custom error logging.
// Created By:Madrina Thapa
//******************************************************************************************

using IGroup.Modules.Common.Utility;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;

namespace Uploader
{
    public class ErrorUtility
    {
        public static void WriteError(Exception ex)
        {
            //Gets errorLog path from configUtility                        
            string errorPath = IGroupUtility.Decrypt(ConfigurationManager.AppSettings["ErrorLogPath"]);
            string requsetUrl = string.Empty;

            if (HttpContext.Current != null && HttpContext.Current.Request != null
                && HttpContext.Current.Request.UrlReferrer != null)
            {
                requsetUrl = HttpContext.Current.Request.UrlReferrer.ToString();
            }
            else
            {
                requsetUrl = "no url";
            }

            //Formats error message
            string completeInfo = requsetUrl + "__Source :" + ex.Source + Environment.NewLine +
                "Stack Trace :" + ex.StackTrace + Environment.NewLine + "Error Message :"
                + ex.Message + Environment.NewLine + "Date Time :" + DateTime.Now.ToString();

            completeInfo = completeInfo + Environment.NewLine +
                "--------------------------------------------------------------" + Environment.NewLine;

            //FileName for ErrorLog.
            string fileName = DateTime.Now.ToShortDateString().Replace("/", "_") + ".txt";

            //Writes Error in LocalPath.
            WriteErrorInFile(completeInfo, fileName, errorPath, FileMode.Append);
        }

        /// <summary>
        /// Writes message to  local directory
        /// </summary>
        /// <param name="content">Content of the File</param>
        /// <param name="fileName">Filename with Extension</param>
        /// <param name="directoryName">path of the directory</param>
        /// <param name="objFileMode">FileMode to append or create</param>
        public static void WriteErrorInFile(string content, string fileName, string directoryName,
                                                   FileMode objFileMode)
        {
            try
            {
                //Create the directory if not exists
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                //Generates the Local path.
                string path = Path.Combine(directoryName, fileName);

                ASCIIEncoding encoding = new ASCIIEncoding();
                FileStream writeStream = new FileStream(path, objFileMode, FileAccess.Write);
                byte[] Buffer = encoding.GetBytes(content);
                writeStream.Write(Buffer, 0, Buffer.Length);

                //Releasing resource
                writeStream.Close();
                writeStream.Dispose();
            }
            catch (Exception)
            {
                //Exception is skipped as this methods is used for writing Error
            }
        }
    }
}
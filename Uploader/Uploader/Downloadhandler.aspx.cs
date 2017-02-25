using System;
using System.Web;
using IGroup.Modules.Common.Utility;

namespace Uploader
{
    public partial class Downloadhandler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string fileId;
                fileId = HttpContext.Current.Request.QueryString["FileId"];
                fileId = fileId.Replace(" ", "+");
                //Page.ClientScript.RegisterClientScriptBlock(GetType(), "PushNotification", "PushNotification();", true);

                frmDownloadhandler.Attributes.Add("src", "DownLoadFile.aspx?FileId=" + fileId);
                fileId = IGroupUtility.Decrypt(fileId);
                hdnDownloadFileId.Value = fileId;

            }
            catch (Exception ex)
            {
                ErrorUtility.WriteError(ex);
            }

        }
    }
}
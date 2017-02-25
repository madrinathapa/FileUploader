using System;
using Microsoft.AspNet.SignalR;
using Uploader.Business;
using System.Data;

namespace Uploader.Entities
{
    public class NotificationHub : Hub
    {
        public void Send(int FileID)
        {
            if (FileID>=0)
            {
                DataTable fileOwnerName = new DataTable();
                BusinessLogic objBusiness = new BusinessLogic();
               fileOwnerName= objBusiness.GetFileOwnerAndDownloadHistory(FileID);
               fileOwnerName.DefaultView.ToTable();

               string Name = Convert.ToString(fileOwnerName.Rows[0]["Name"]);
               bool IsAdmin = Convert.ToBoolean(fileOwnerName.Rows[0]["IsAdmin"]);
               string MachineName = Convert.ToString(fileOwnerName.Rows[0]["MachineName"]);
               string IpAddress = Convert.ToString(fileOwnerName.Rows[0]["IpAddress"]);
               string DownloadDate = Convert.ToString(fileOwnerName.Rows[0]["DownloadDate"]);
               string fileName = Convert.ToString(fileOwnerName.Rows[0]["OriginalFileName"]);
               

                   //  fileOwnerName = objBusiness.GetFileOwnerAndDownloadHistory(FileID);
               Clients.All.broadcastMessage(FileID, Name, IsAdmin, MachineName, IpAddress, DownloadDate, fileName);
            }
        }
    }
}
//--------------------------------------------------------------------------------
// FileName:ICustomPrincipal.cs
// Description:An entity class for user account details.
// Created By:Madrina Thapa.
//--------------------------------------------------------------------------------

namespace Uploader.Entities
{
    public class UserAccount
    {
        public string TotalFiles { get; set; }
        public string TotalDownloads { get; set; }
        public string TotalSize { get; set; }
    }
}
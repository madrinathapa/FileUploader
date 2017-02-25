//--------------------------------------------------------------------------------
// FileName:User.cs
// Description:An entity class for the user details
// Created By:Madrina Thapa.
//--------------------------------------------------------------------------------

namespace Uploader.Entities
{
    public class User
    {
        public string UserHashCode
        {
            get;
            set;
        }

        public string EmailId
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public bool IsAdmin
        {
            get;
            set;
        }
    }
}
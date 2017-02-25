//--------------------------------------------------------------------------------
// FileName:CustomPrincipal.cs
// Description:Entity class Inherited from ICustomPrincipal class.
// Created By:Madrina Thapa.
//--------------------------------------------------------------------------------

using System.Security.Principal;

namespace Uploader.Entities
{
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return false; }

        public CustomPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public int IsAdmin { get; set; }
    }
}
//--------------------------------------------------------------------------------
// FileName:ICustomPrincipal.cs
// Description:An entity class inherited from IPrincipal interface.
// Created By:Madrina Thapa.
//--------------------------------------------------------------------------------

using System.Security.Principal;

namespace Uploader.Entities
{
    interface ICustomPrincipal : IPrincipal
    {
        string Id { get; set; }
        string UserName { get; set; }
    }
}

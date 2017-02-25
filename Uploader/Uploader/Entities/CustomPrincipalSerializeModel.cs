//--------------------------------------------------------------------------------
// FileName:CustomPrincipalSerializeModel.cs
// Description:An entity class for the serialized customprincipal class
// Created By:Madrina Thapa.
//--------------------------------------------------------------------------------


namespace Uploader.Entities
{
    public class CustomPrincipalSerializeModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int IsAdmin { get; set; }
    }
}
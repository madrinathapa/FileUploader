﻿//--------------------------------------------------------------------------------
// FileName:RegisteredUsers.cs
// Description:Has all the properties for the user grid.
// Created By:Madrina Thapa.
//--------------------------------------------------------------------------------

namespace Uploader.Entities
{
    public class RegisteredUsers
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public decimal? AllocatedSpace { get; set; }
        public decimal? UsedSpace { get; set; }
        public decimal? FreeSpace { get; set; }
        public string TotalFiles { get; set; }
        public string LastLogin { get; set; }
    }
}
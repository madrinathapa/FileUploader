﻿//--------------------------------------------------------------------------------
// FileName:FileGridColumn.cs
// Description:Has all the properties for file grid on admin page.
// Created By:Madrina Thapa.
//--------------------------------------------------------------------------------


namespace Uploader.Entities
{
    public class FileGridColumns
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public string uploadedBy { get; set; }
        public string uploadedOn { get; set; }
        public decimal size { get; set; }
        public string status { get; set; }
    }
}
//--------------------------------------------------------------------------------
// FileName:GridProperties.cs
// Description:Grid properties entity class.
// Created By:Madrina Thapa.
//--------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Uploader.Entities
{
    public class GridProperties<T>
    {
        public List<T> rows { get; set; }

        public int records { get; set; }

        public int total
        {
            get;
            set;
        }
        public int page { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace canakkale.Models
{
    public class Image
    {
        public int Id { get; set; } // Primary Key
        public string Url { get; set; } // Resim URL'si
        public string Description { get; set; } // Resim Açıklaması
        public string Category { get; set; } // Resim Kategorisi (Slider, Canakkale vb.)
    }
}

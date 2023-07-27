using System;
using System.Collections.Generic;
using System.Linq;

namespace FileManager.Web.ViewModel
{
    public class BasketViewModel
    {
        public Guid UserId { get; set; }
        public long ResourceId { get; set; }
        public string ResourceTitle { get; set; }
        public string ResourceThumbPath { get; set; }
    }
}
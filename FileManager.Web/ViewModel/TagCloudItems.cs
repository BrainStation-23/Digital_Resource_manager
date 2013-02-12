using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileManager.Web.ViewModel
{
    public class TagCloudItems
    {
        public long TotalResource { get; set; }
        public List<TagCloud> Tags { get; set; }
    }
}
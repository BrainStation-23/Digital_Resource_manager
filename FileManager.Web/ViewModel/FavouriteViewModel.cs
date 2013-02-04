using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileManager.Web.ViewModel
{
    public class FavouriteViewModel
    {
        public long Id
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        public string imgPath
        {
            get;
            set;
        }
        public string Download
        {
            get;
            set;
        }
        public string ResourceZipName { get; set; }
    }
}
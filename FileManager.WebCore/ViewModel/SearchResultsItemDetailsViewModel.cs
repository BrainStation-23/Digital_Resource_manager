using System;
using System.Collections.Generic;
using System.Linq;
using FileManager.Model;

namespace FileManager.Web.ViewModel
{
    public class SearchResultsItemDetailsViewModel
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
        public string Description
        {
            get;
            set;
        }
        public string Rank
        {
            get;
            set;
        }
        public string Category
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
        public int ItemCount
        {
            get;
            set;
        }
        public int UserRating { get; set; }
        public int RatingCount { get; set; }
        public List<TagsViewModel> Tags { get; set; }

        public string FileName { get; set; }

        public int CategoryId { get; set; }
        public long TotalDownload { get; set; }
        public string UserName { get; set; }
    }
}
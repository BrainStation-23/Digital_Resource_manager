using System;
using System.Collections.Generic;
using System.Linq;

namespace FileManager.Web.ViewModel
{
	public class SearchResultsViewModel
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

        public int DownloadCount { get; set; }
        public int UserRating { get; set; }
        public int RatingCount { get; set; }

        public string ResourceZipName { get; set; }

        public string FavouriteIconClass { get; set; }
        public string FavouriteIconHelpTitle { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
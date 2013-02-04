using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileManager.Web.ViewModel
{
	public class CategoryViewModel
	{
		public int Id
		{
			get;
			set;
		}
		public string Title
		{
			get;
			set;
		}
        public int? CategoryId { get; set; }
		public string ParentCategory
		{
			get;
			set;
		}
	}
}
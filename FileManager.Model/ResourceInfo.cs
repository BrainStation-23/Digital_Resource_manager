using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
 public	class ResourceInfo
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
        public int UserRating { get; set; }
        public int RatingCount { get; set; }
       public string ResourceName { get; set; }
		public int CategoryId
		{
			get;
			set;
		}
		public virtual Category Category
		{
			get;
			set;
		}
		public virtual ICollection<Tag> Tags
		{
			get;
			set;
		}

        public int DownloadCount { get; set; }
        public string ThumbName { get; set; }
        public Guid UserId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
public	class Category
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
		public int? CategoryId
		{
			get;
			set;
		}
		public virtual Category ParentsCategory
		{
			get;
			set;
		}
	}
}

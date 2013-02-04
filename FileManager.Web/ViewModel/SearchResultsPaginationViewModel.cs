using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileManager.Web.ViewModel
{
    public class SearchResultsPaginationViewModel
    {
        public IEnumerable<SearchResultsViewModel> SearchResult { get; set; }
        public int TotalPage { get; set; }
    }
}
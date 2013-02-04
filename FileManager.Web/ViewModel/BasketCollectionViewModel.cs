using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileManager.Web.ViewModel
{
    public class BasketCollectionViewModel
    {
        public string Zippath { get; set; }
        public List<BasketViewModel> BasketViewModelList { get; set; }
    }
}
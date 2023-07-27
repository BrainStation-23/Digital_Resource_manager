using System;
using System.Collections.Generic;
using System.Linq;

namespace FileManager.Web.ViewModel
{
    public class BasketCollectionViewModel
    {
        public string Zippath { get; set; }
        public List<BasketViewModel> BasketViewModelList { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileManager.Web.ViewModel
{
    public class RolePermissionModel
    {
        public string RoleName { get; set; }
        public List<PermissionViewModel> PermissionViewModelList { get; set; }
        public List<RolePermissionViewModel> RolePermissionViewModelList { get; set; }
    }
}
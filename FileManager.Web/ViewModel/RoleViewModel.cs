using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileManager.Web.ViewModel
{
    public class RoleViewModel
    {
        public string RoleId { get; set; }
        public string UserRole { get; set; }
        public List<PermissionViewModel> Userpermissions { get; set; }
    }
}
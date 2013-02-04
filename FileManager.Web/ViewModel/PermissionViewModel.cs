using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FileManager.Web.ViewModel
{
    public class PermissionViewModel
    {

        public string PermissionName { get; set; }
        public string PermissionNameRead { get; set; }
        public string PermissionNameWrite { get; set; }
        public string PermissionNameDelete { get; set; }

        public bool AllowRead { get; set; }
        public bool AllowWrite { get; set; }
        public bool AllowDelete { get; set; }

        public string ReadClass { get; set; }
        public string WriteClass { get; set; }
        public string DeleteClass { get; set; }
    }
}
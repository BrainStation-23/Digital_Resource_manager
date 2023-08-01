using FileManager.DAL.DataContext;
using FileManager.Model;
using System;
using System.Linq;


namespace FileManager.BusinessFacade
{
	public class FileManagerAuthorizationFacade
	{
		private FileManagerDbContext db = new FileManagerDbContext();

		public bool IsAuthorize(string orginName, string permissonType)
		{
			string userName = GetCurrentUserName();
			if (!string.IsNullOrEmpty(userName))
			{
				string userRole = this.GetUserRole(userName);
				if (userRole.Equals("admin", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				var roleId = db.Roles.FirstOrDefault(x => x.RoleName == userRole).RoleId.ToString();
				var permissionId = (from per in db.Permissions where per.PermissionName == orginName select per.Id).First();

				RolePermission rolePermission = (from rolePer in db.RolePermissions
												 where rolePer.RoleId == roleId && rolePer.PermissionId == permissionId
												 select rolePer).FirstOrDefault();
				if (rolePermission != null && permissonType != string.Empty && orginName != string.Empty)
				{
					if (permissonType.Equals("read"))
						return rolePermission.AllowRead;
					else if (permissonType.Equals("write"))
						return rolePermission.AllowWrite;
					else if (permissonType.Equals("delete"))
						return rolePermission.AllowDelete;
					else
						return false;
				}
				else
					return false;

			}
			return false;
		}
		public string GetCurrentUserRole()
		{
			return GetUserRole(this.GetCurrentUserName());
		}

		public string GetCurrentUserName()
		{
			string useremail = string.Empty;
			if (HttpContext.Current.Session != null && HttpContext.Current.Session["emailAddress"] != null)
			{
				useremail = HttpContext.Current.Session["emailAddress"].ToString();
			}
			return useremail;
		}

		private string GetUserRole(string userName)
		{
			string userRole = string.Empty;
			if (!string.IsNullOrEmpty(userName))
			{
				userRole = new CodeFirstRoleProvider().GetRolesForUser(userName).FirstOrDefault();
			}
			return userRole;
		}
		public string GetCurrentUserRoleId()
		{
			string userId = string.Empty;
			var currentUserName = GetCurrentUserName();
			if (!string.IsNullOrEmpty(currentUserName))
			{
				userId = db.Users.FirstOrDefault(x => x.Username.Equals(currentUserName, StringComparison.OrdinalIgnoreCase)).Roles.FirstOrDefault().RoleId.ToString();
				return userId;
			}
			return userId;
		}
		public Guid GetCurrentUserId()
		{
			var currentUserName = this.GetCurrentUserName();
			if (!string.IsNullOrEmpty(currentUserName))
			{
				User user = db.Users.FirstOrDefault(x => x.Username.Equals(currentUserName, StringComparison.OrdinalIgnoreCase));
				if (user != null)
					return user.UserId;
			}
			return Guid.Empty;
		}
		public User GetCurrentUser()
		{
			User user = new User();
			var currentUserName = GetCurrentUserName();
			if (!string.IsNullOrEmpty(currentUserName))
			{
				user = db.Users.FirstOrDefault(x => x.Username.Equals(currentUserName, StringComparison.OrdinalIgnoreCase));
				if (user != null)
					return user;
			}
			return user;
		}
		public bool HasSession()
		{
			var hasSession = false;
			if (HttpContext.Current.Session != null && HttpContext.Current.Session["emailAddress"] != null)
			{
				hasSession = true;
			}
			return hasSession;
		}
	}
}

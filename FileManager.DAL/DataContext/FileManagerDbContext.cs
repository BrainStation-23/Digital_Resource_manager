using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using FileManager.Model;
using System.Data.Entity.Infrastructure;


namespace FileManager.DAL.DataContext
{
	public class FileManagerDbContext : DbContext
	{
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Downloadhistory> Downloadhistories { get; set; }

        public DbSet<Basket> Baskets { get; set; }
        

		public DbSet<ResourceInfo> ResourceInfos
		{
			get;
			set;
		}
		public DbSet<Tag> Tags
		{
			get;
			set;
		}
		public DbSet<Category> Categories
		{
			get;
			set;
		}
        public DbSet<UserFavouriteResource> UserFavouriteResources { get; set; }
       
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}
	}

	public class FileManagerContextInitializer : DropCreateDatabaseIfModelChanges<FileManagerDbContext>
	{
		protected override void Seed(FileManagerDbContext context)
		{

            WebSecurity.Register("admin@yourdomain.com", "123456", "admin@yourdomain.com", true, "admin", "admin");
            Roles.CreateRole("Admin");
            Roles.AddUserToRole("admin@yourdomain.com", "Admin");
            

            var categories = new List<Category>()
			{
				new Category(){Title="Category 1"},
				new Category(){Title="Category 2",CategoryId=1}
			};

			categories.ForEach(c => context.Categories.Add(c));
			context.SaveChanges();

			var tags = new List<Tag>()
			{
				new Tag(){Name="tag 1"},
				new Tag(){Name="tag 2"},
			};

			tags.ForEach(t=>context.Tags.Add(t));
			context.SaveChanges();

            var permissions = new List<Permission>()
			{
                new Permission(){PermissionName="Resource", ReadApplicable=true, WriteApplicable=true, DeleteApplicable=true},
                new Permission(){PermissionName="Download", ReadApplicable=true, WriteApplicable=false, DeleteApplicable=false},
                new Permission(){PermissionName="CreateRole", ReadApplicable=true, WriteApplicable=true, DeleteApplicable=true},
                new Permission(){PermissionName="Users", ReadApplicable=true, WriteApplicable=true, DeleteApplicable=true},
                new Permission(){PermissionName="Category", ReadApplicable=true, WriteApplicable=true, DeleteApplicable=true},
                new Permission(){PermissionName="DownloadHistory", ReadApplicable=true, WriteApplicable=false, DeleteApplicable=false}
			};

            permissions.ForEach(t => context.Permissions.Add(t));
            context.SaveChanges();
		
		}
	}
}

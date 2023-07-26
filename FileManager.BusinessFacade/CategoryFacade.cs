using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.Model;
using EntityState = System.Data.Entity.EntityState;

namespace FileManager.BusinessFacade
{
    public partial class Facade
    {
        public IEnumerable<Category> GetCategories()
        {
            return _db.Categories.ToList();
        }
        public bool HasCategories()
        {
            var categories = _db.Categories;
            return categories != null && categories.Count() > 0;
        }
        public int GetFirstCategoryId()
        {
            return _db.Categories.FirstOrDefault().Id;
        }
        public Category GetCategoryById(int id)
        {
            return _db.Categories.Find(id);
        }
        public Category GetCategoryByTitle(string title)
        {
            return _db.Categories.Where(x=>x.Title.Equals(title,StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        public List<Category> GetChildrenByParentID(int categoryId)
        {
            return _db.Categories.Where(x => x.CategoryId == categoryId).ToList();
        }
        public bool UpdateCategory(Category category)
        {
            _db.Entry(category).State = (EntityState)System.Data.EntityState.Modified;

            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool AddCategory(Category category)
        {
            try
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteCategory(int id)
        {

            Category category = _db.Categories.Find(id);
            if (category == null)
            {
                return false;
            }
            var resources = this.GetResourceByCategoryId(category.Id).ToList();
            _db.Categories.Remove(category);

            try
            {                
                foreach (var resource in resources)
                {
                    var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Resources/"), resource.Id.ToString());

                    DirectoryInfo d = new DirectoryInfo(path);

                    if (d.Exists)
                    {
                        d.Delete(true);
                    }
                }

                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}

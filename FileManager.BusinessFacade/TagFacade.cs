using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.Model;

namespace FileManager.BusinessFacade
{
    public partial class Facade
    {
        public IEnumerable<Tag> GetTags()
        {
            return _db.Tags.ToList();
        }
        public List<string> GetTagByChar(string chars)
        {
            return _db.Tags.Where(x => x.Name.StartsWith(chars)).Select(x => x.Name).ToList<string>();
        }
        public Tag GetTagByTagName(string tagName)
        {
            return _db.Tags.Where(x => x.Name.Equals(tagName.Trim(),StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// This function is used to save tags if Category Titel does not exists in the tag table
        /// </summary>
        /// <param name="category"></param>
        public void TagsSaveIfNotExists(Category category)
        {
            var _tag = _db.Tags.Where(t => t.Name.Equals(category.Title));
            if (_tag.Count() == 0)
            {
                Tag tag = new Tag()
                {
                    Name = category.Title
                };
                _db.Tags.Add(tag);
                _db.SaveChanges();
            }
        }
    }
}

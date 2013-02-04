using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
   public class UserFavouriteResource
    {
        public long Id { get; set; }
        public long ResourceId { get; set; }
        public Guid UserId { get; set; }
    }
}

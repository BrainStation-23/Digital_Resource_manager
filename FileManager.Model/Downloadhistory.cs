using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public class Downloadhistory
    {
        public long Id { get; set; }

        public Guid UserId { get; set; }

        public long ResourceId { get; set; }

        public DateTime DownloadDateTime { get; set; }
    }
}

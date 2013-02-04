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
        public IEnumerable<Downloadhistory> GetDowloadHistories()
        {
            return _db.Downloadhistories.ToList();
        }
        public IEnumerable<Downloadhistory> GetDownloadHistoriesByResourceId(long resourceId)
        {
            return _db.Downloadhistories.Where(x => x.ResourceId == resourceId).ToList();
        }
        public IEnumerable<Downloadhistory> GetDownloadHistoriesByResourceIdAndUserId(long resourceId, Guid userId)
        {
            return _db.Downloadhistories.Where(x => x.UserId == userId && x.ResourceId == resourceId).AsEnumerable();
        }
        public bool RemoveDownloadHistoriesByResourceId(long resourceId)
        {
            _db.Downloadhistories.Where(x => x.ResourceId == resourceId).ToList().ForEach(d => {
                _db.Downloadhistories.Remove(d);
             });
            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        public long CalculateTotalDownload(long resourceId)
        {
            long totalDownload = 0;
            List<long> downloadhistoryList = _db.Downloadhistories.Where(x => x.ResourceId == resourceId).Select(d=>d.Id).ToList();
            if (downloadhistoryList != null)
            {
                totalDownload = downloadhistoryList.Count;
            }
            return totalDownload;
        }
        public bool AddDownloadHistory(Downloadhistory downloadHistory)
        {
            try
            {
                _db.Downloadhistories.Add(downloadHistory);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public IEnumerable<Downloadhistory> GetDownloadHistoriesByUserId(Guid userId)
        {
            return _db.Downloadhistories.Where(h => h.UserId.Equals(userId)).AsEnumerable();
        }
        public List<Downloadhistory> GetDistinctDowloadHistoryByUser()
        {
            var distictdowloadedItem = _db.Downloadhistories.GroupBy(x => x.UserId)
               .Select(t => t.OrderByDescending(c => c.DownloadDateTime).FirstOrDefault()).ToList();
            return distictdowloadedItem;
        }

    }
}

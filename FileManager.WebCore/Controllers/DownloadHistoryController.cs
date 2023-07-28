using FileManager.BusinessFacade;
using FileManager.Model;
using FileManager.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Data;


namespace FileManager.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DownloadHistoryController : ControllerBase
	{
		private FileManagerAuthorizationFacade _fileManagerAuth = new FileManagerAuthorizationFacade();
		private Facade _facade = new Facade();

		[HttpGet]
		// GET api/<controller>
		public List<DownloadHistoryViewModel> Get()
		{
			List<DownloadHistoryViewModel> downloadHistoryViewModelList = new List<DownloadHistoryViewModel>();
			if (IsAuthorize("read"))
			{
				List<User> userList = _facade.GetUsers().ToList();
				List<ResourceInfo> resourceInfoList = _facade.GetResources().ToList();


				ResourceInfo resourceInfoLocal = null;
				var currentUserRole = _fileManagerAuth.GetCurrentUserRole();
				if (currentUserRole.Equals("admin", StringComparison.OrdinalIgnoreCase))
				{
					List<Downloadhistory> downloadhistoryList = _facade.GetDowloadHistories().ToList();
					List<long> distinctResourceIdList = downloadhistoryList.Select(i => i.ResourceId).Distinct().ToList<long>();
					List<Guid> distinctUserIdList = downloadhistoryList.Select(i => i.UserId).Distinct().ToList<Guid>();
					foreach (long id in distinctResourceIdList)
					{
						foreach (Guid userid in distinctUserIdList)
						{
							resourceInfoLocal = resourceInfoList.Where(x => x.Id == id).FirstOrDefault();
							var user = userList.Where(x => x.UserId == userid).FirstOrDefault();
							if (resourceInfoLocal != null && user != null)
							{
								DownloadHistoryViewModel downloadHistoryViewModel = new DownloadHistoryViewModel();
								downloadHistoryViewModel.ResourceId = id;
								downloadHistoryViewModel.ResourceTitle = resourceInfoLocal.Title;
								downloadHistoryViewModel.UserId = userid;
								downloadHistoryViewModel.UserName = userList.Where(x => x.UserId == userid).FirstOrDefault().Username;
								downloadHistoryViewModel.UserWiseDownloadCount = downloadhistoryList.Where(x => x.ResourceId == id && x.UserId == userid).ToList().Count;

								downloadHistoryViewModelList.Add(downloadHistoryViewModel);
							}
						}
					}
				}
				else
				{
					if (_fileManagerAuth.HasSession())
					{
						var currentUser = _fileManagerAuth.GetCurrentUser();
						List<Downloadhistory> downloadhistoryList = _facade.GetDownloadHistoriesByUserId(currentUser.UserId).ToList();
						List<long> distinctResourceIdList = downloadhistoryList.Select(i => i.ResourceId).Distinct().ToList<long>();
						foreach (long id in distinctResourceIdList)
						{
							resourceInfoLocal = resourceInfoList.Where(x => x.Id == id).FirstOrDefault();
							if (resourceInfoLocal != null)
							{
								DownloadHistoryViewModel downloadHistoryViewModel = new DownloadHistoryViewModel();
								downloadHistoryViewModel.ResourceId = id;
								downloadHistoryViewModel.ResourceTitle = resourceInfoLocal.Title;
								downloadHistoryViewModel.UserId = currentUser.UserId;
								downloadHistoryViewModel.UserName = currentUser.Username;
								downloadHistoryViewModel.UserWiseDownloadCount = downloadhistoryList.Where(x => x.ResourceId == id && x.UserId == currentUser.UserId).ToList().Count;

								downloadHistoryViewModelList.Add(downloadHistoryViewModel);
							}
						}
					}

				}
			}
			return downloadHistoryViewModelList;
		}

		[HttpGet("{userIdAndResourceId?}")]
		public List<DownloadHistoryViewModel> GetUserAndResourceWiseHistory(string userIdAndResourceId)
		{
			List<DownloadHistoryViewModel> downloadHistoryViewModelList = new List<DownloadHistoryViewModel>();
			string[] userResourcearr = userIdAndResourceId.Split('~');
			if (IsAuthorize("read") && userResourcearr.Length == 2)
			{
				Guid userId = Guid.Parse(userResourcearr[0]);
				long resourceId = Convert.ToInt64(userResourcearr[1]);


				List<Downloadhistory> downloadhistoryList = _facade.GetDownloadHistoriesByResourceIdAndUserId(resourceId, userId).ToList();
				List<User> userList = _facade.GetUsers().ToList();
				List<ResourceInfo> resourceInfoList = _facade.GetResources().ToList();

				foreach (Downloadhistory item in downloadhistoryList)
				{
					DownloadHistoryViewModel downloadHistoryViewModel = new DownloadHistoryViewModel();
					downloadHistoryViewModel.ResourceId = item.ResourceId;
					downloadHistoryViewModel.ResourceTitle = resourceInfoList.Where(x => x.Id == item.ResourceId).FirstOrDefault().Title;
					downloadHistoryViewModel.UserId = userId;
					downloadHistoryViewModel.UserName = userList.Where(x => x.UserId == userId).FirstOrDefault().Username;
					downloadHistoryViewModel.DownloaddateTime = item.DownloadDateTime.ToString("dd/MMM/yyyy : hh:mm");

					downloadHistoryViewModelList.Add(downloadHistoryViewModel);
				}
			}

			return downloadHistoryViewModelList;
		}

		[HttpGet("{resourceId?}")]

		// GET api/<controller>/5
		public long Get(long resourceId)
		{
			long totalDownloadCount = 0;

			List<Downloadhistory> downloadhistoryList = _facade.GetDownloadHistoriesByResourceId(resourceId).ToList();
			if (downloadhistoryList != null)
			{
				totalDownloadCount = downloadhistoryList.Count;
			}
			return totalDownloadCount;
		}

		[HttpPost("{downloadhistory?}")]

		// POST api/<controller>
		public bool Post(Downloadhistory downloadhistory)
		{
			Guid userId = _fileManagerAuth.GetCurrentUserId();
			downloadhistory.UserId = userId;
			downloadhistory.DownloadDateTime = DateTime.Now;

			if (!_facade.AddDownloadHistory(downloadhistory))
				return false;

			ResourceInfo resourceInfo = _facade.GetResourceById(downloadhistory.ResourceId);
			if (resourceInfo != null)
			{
				resourceInfo.DownloadCount++;
				_facade.UpdateResource(resourceInfo);
			}

			return true;
		}

		private bool IsAuthorize(string permissonType)
		{
			return _fileManagerAuth.IsAuthorize("DownloadHistory", permissonType);
		}
	}
}
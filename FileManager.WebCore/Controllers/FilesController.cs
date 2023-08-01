using FileManager.BusinessFacade;
using FileManager.Model;
using FileManager.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace FileManager.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class FilesController : ControllerBase
	{
		private readonly IWebHostEnvironment _hostEnvironment;
		private FileManagerAuthorizationFacade _fileManagerAuth = new FileManagerAuthorizationFacade();
		private Facade _facade = new Facade();

		public FilesController(IWebHostEnvironment hostEnvironment)
		{
			_hostEnvironment = hostEnvironment;
		}

		[HttpGet]
		// GET api/file

		public IEnumerable<ResourceInfo> GetFileInfos()
		{
			if (!IsAuthorize("read"))
			{
				return null;
			}
			return _facade.GetResources();
		}

		[HttpGet("{id?}")]

		// GET api/file/5
		public ResourceInfo Get(long id)
		{
			return _facade.GetResourceById(id);
		}

		private IEnumerable<SearchResultsViewModel> GetSearchResultsViewModel(List<ResourceInfo> resourceInfos)
		{
			List<SearchResultsViewModel> _searchResultsVM = new List<SearchResultsViewModel>();
			var unAuthorize = false;
			if (!IsAuthorizeForDownload())
			{
				unAuthorize = true;
			}
			if (resourceInfos == null) return _searchResultsVM;
			Guid userId = _fileManagerAuth.GetCurrentUserId();

			foreach (var item in resourceInfos)
			{
				var filePath = "/Resources/" + item.Id + "/" + item.ThumbName;

				string extension = System.IO.Path.GetExtension(item.ResourceName);
				string fileName = item.ResourceName.Substring(0, item.ResourceName.Length - extension.Length);

				var downloadPath = "/Resources/" + item.Id + "/" + fileName + ".zip";
				var resourceZipName = fileName + ".zip";
				if (unAuthorize)
				{
					downloadPath = "";
					resourceZipName = "";
				}
				bool isInFavouriteList = _facade.IsInFavouriteList(item.Id, userId);
				string favouriteIconClass = "";
				string favouriteIconHelpTitle = "";
				if (isInFavouriteList)
				{
					favouriteIconClass = "icon-yellow";
					favouriteIconHelpTitle = "In Favourite List";
				}
				else
				{
					favouriteIconClass = "icon-star";
					favouriteIconHelpTitle = "Add to Favourite";
				}

				var resultsVM = new SearchResultsViewModel()
				{
					Id = item.Id,
					Title = item.Title,
					Description = item.Description,
					Rank = item.Rank,
					Category = item.Category.Title,
					imgPath = filePath,
					Download = downloadPath,
					ItemCount = resourceInfos.Count,
					UserRating = item.UserRating,
					RatingCount = item.RatingCount,
					ResourceZipName = resourceZipName,
					FavouriteIconClass = favouriteIconClass,
					FavouriteIconHelpTitle = favouriteIconHelpTitle,
					DownloadCount = item.DownloadCount,
					CreateDate = item.CreateDate.ToUniversalTime()
				};
				_searchResultsVM.Add(resultsVM);
			}
			return _searchResultsVM;
		}

		[HttpGet("{searchText?}/{pageNumber?}")]
		public SearchResultsPaginationViewModel GetSearchResultsPagination(string searchText, int pageNumber)
		{
			if (!IsAuthorize("read"))
			{
				return null;
			}
			var searchResultsPagination = new SearchResultsPaginationViewModel();
			try
			{
				var data = this.GetSearchResultsViewModel(_facade.GetSearchResults(searchText));
				if (data != null)
				{
					var totalData = data.Count();

					double resultToDisplay = 18;
					double pagecount = Math.Ceiling(totalData / resultToDisplay);
					int totalPage = totalData > resultToDisplay ? Convert.ToInt32(pagecount) : 1;
					int actualDisplay = totalData < resultToDisplay ? totalData : Convert.ToInt32(resultToDisplay);

					searchResultsPagination.SearchResult = data.Skip(actualDisplay * pageNumber).Take(actualDisplay);
					searchResultsPagination.TotalPage = totalPage;
				}
			}
			catch (Exception e)
			{

			}
			return searchResultsPagination;
		}

		//changed from HttpResponseMessage return to ActionResult<Category>
		[ActionName("CreateFile")]
		[HttpPost]
		public async Task<ActionResult> PostFile(IFormFile file)
		{
			if (!IsAuthorize("write"))
			{
				return Unauthorized();
				//return Request.CreateResponse(HttpStatusCode.Unauthorized);
			}

			if (file == null || file.Length == 0)
				return BadRequest("No file uploaded");

			// Check if the request contains multipart/form-data.
			if (!HttpContext.Request.HasFormContentType && !HttpContext.Request.Form.Files.Any())
				return new UnsupportedMediaTypeResult();


			//if (!Request.Content.IsMimeMultipartContent())
			//{
			//	throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			//}
			//TODO Check if it works
			string root = Path.Combine(_hostEnvironment.WebRootPath, $"TempResources");
			//string root = Path.Combine("./TempResources");
			if (!Directory.Exists(root))
			{
				Directory.CreateDirectory(root);
			}

			using (var stream = new FileStream(root, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			var provider = new MultipartFormDataStreamProvider(root);

			try
			{
				// Read the form data and return an async task. singlefile.Headers.ContentDisposition.Name
				await Request.Content.ReadAsMultipartAsync(provider);

				var clientResourceName = "resource";
				var clientThumbName = "thumb";
				var resourceNameFromJson = string.Empty;
				var thumbNameFromJson = string.Empty;
				FileInfo fileInfo = null;
				FileInfo thumbFileInfo = null;

				foreach (var singlefile in provider.FileData)
				{
					//var localName = JsonConvert.DeserializeObject(singlefile.Headers.ContentDisposition.Name);
					var localName = Json.Decode(singlefile.Headers.ContentDisposition.Name);
					//var localresourceName= JsonConvert.DeserializeObject(singlefile.Headers.ContentDisposition.FileName);
					var localresourceName = Json.Decode(singlefile.Headers.ContentDisposition.FileName);
					if (string.Equals(localName, clientResourceName, StringComparison.OrdinalIgnoreCase))
					{
						resourceNameFromJson = localresourceName;
						fileInfo = new FileInfo(singlefile.LocalFileName);
					}
					else if (string.Equals(localName, clientThumbName, StringComparison.OrdinalIgnoreCase))
					{
						thumbNameFromJson = localresourceName;
						thumbFileInfo = new FileInfo(singlefile.LocalFileName);
					}
				}

				// Check if no file.
				if (string.IsNullOrEmpty(resourceNameFromJson) || string.IsNullOrEmpty(thumbNameFromJson))
				{
					return NoContent();
					//return Request.CreateResponse(HttpStatusCode.NoContent);
				}

				var resourceInfo = _facade.GetResourceInfoFromFormProvider(provider);
				resourceInfo.ResourceName = resourceNameFromJson;
				resourceInfo.ThumbName = string.Concat(Guid.NewGuid(), "_", thumbNameFromJson);
				resourceInfo.UserId = _fileManagerAuth.GetCurrentUserId();

				if (resourceInfo.CategoryId == 0)
				{
					if (_facade.HasCategories())
					{
						resourceInfo.CategoryId = _facade.GetFirstCategoryId();
					}
				}

				//Here we have to check Is model valid

				_facade.CreateResource(resourceInfo);
				var id = resourceInfo.Id;
				if (id > 0)
				{
					var path = _facade.MoveResources(fileInfo, thumbFileInfo, resourceInfo);

					// This code will create a zip file for downloading option.
					_facade.CreateResZipFile(path, resourceNameFromJson);
				}
				else
				{
					_facade.DeleteFile(fileInfo.FullName);
					_facade.DeleteFile(thumbFileInfo.FullName);
					var result = StatusCode(StatusCodes.Status500InternalServerError, resourceInfo.ResourceName);
					return result;
					//return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, resourceInfo.ResourceName);
				}

				return StatusCode(StatusCodes.Status201Created);
				//return Request.CreateResponse(HttpStatusCode.Created);
			}
			catch (System.Exception e)
			{
				var result = StatusCode(StatusCodes.Status500InternalServerError, e);
				return result;
				//return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
			}
		}

		[HttpPost("{rating?}/{fileId?}")]
		public bool PostRating(int rating, int fileId)
		{
			if (fileId < 0)
			{
				return false;
			}

			return _facade.AddRating(rating, fileId);
		}

		[HttpDelete("{id?}")]
		//changed from HttpResponseMessage return to ActionResult<long>
		// DELETE api/file/5
		public ActionResult<long> Delete(long id)
		{
			if (id < 1)
			{
				return StatusCode(StatusCodes.Status204NoContent);
				//return Request.CreateResponse(HttpStatusCode.NoContent);
			}
			if (!IsAuthorize("delete"))
			{
				return Unauthorized();
				//return Request.CreateResponse(HttpStatusCode.Unauthorized);
			}
			try
			{
				if (_facade.DeleteResource(id))
					return new OkObjectResult(id);
				//return Request.CreateResponse(HttpStatusCode.OK, id);
			}
			catch (Exception e)
			{

			}

			return NotFound();
			//return Request.CreateResponse(HttpStatusCode.NotFound);
		}

		[HttpGet("{chars?}")]
		public List<string> GetTagByChar(string chars)
		{
			var tags = _facade.GetTagByChar(chars);
			return tags;

		}

		[HttpGet]
		public List<TagCloud> GetTagCloud()
		{
			var tags = _facade.GetTags();
			//var cloudtags = new TagCloudItems();
			var tagClouds = new List<TagCloud>();

			tags.ToList().ForEach(t =>
			{
				var noOfResource = t.ResourceInfos.Count;
				tagClouds.Add(new TagCloud
				{

					TagName = t.Name.ToLower(),
					NoOfResource = noOfResource

				});
			});

			tagClouds.Sort((x, y) => x.TagName.CompareTo(y.TagName));
			return tagClouds;

		}

		[HttpGet("{itemDetailsId?}")]
		public SearchResultsItemDetailsViewModel GetSearchResultsDetailsViewModel(long itemDetailsId)
		{
			if (!IsAuthorize("read"))
			{
				return null;
			}
			var _searchResultsItemDetailsVM = new SearchResultsItemDetailsViewModel();

			var item = _facade.GetResourceById(itemDetailsId);
			if (item == null)
				return _searchResultsItemDetailsVM;

			var filePath = "/Resources/" + item.Id + "/" + item.ThumbName;

			string extension = System.IO.Path.GetExtension(item.ResourceName);
			string fileName = item.ResourceName.Substring(0, item.ResourceName.Length - extension.Length);

			var downloadPath = "/Resources/" + item.Id + "/" + fileName + ".zip";
			if (!IsAuthorizeForDownload())
			{
				downloadPath = "";
			}
			var tags = new List<TagsViewModel>();
			foreach (var tag in item.Tags)
			{
				tags.Add(new TagsViewModel { TagName = tag.Name });
			}
			long totaldownloadCount = _facade.CalculateTotalDownload(item.Id);

			//double rating = 0;
			int rating = item.UserRating != 0 ? item.UserRating / item.RatingCount : 0;  // as user rating is total rating
			_searchResultsItemDetailsVM = new SearchResultsItemDetailsViewModel()
			{
				Id = item.Id,
				Title = item.Title,
				Description = item.Description,
				Rank = item.Rank,
				Category = item.Category.Title,
				CategoryId = item.Category.Id,
				imgPath = filePath,
				Download = downloadPath,
				UserRating = rating,
				RatingCount = item.RatingCount,
				Tags = tags,
				FileName = item.ResourceName,
				TotalDownload = totaldownloadCount,
				UserName = _facade.GetUserNameByUserId(item.UserId)
			};

			return _searchResultsItemDetailsVM;
		}

		[ActionName("UpdateResource")]
		[HttpPost]
		public async Task<ActionResult> PostUpdateResource()
		{
			if (!IsAuthorize("write"))
			{
				return Unauthorized();
				//return Request.CreateResponse(HttpStatusCode.Unauthorized);
			}
			// Check if the request contains multipart/form-data.

			if (!HttpContext.Request.HasFormContentType && !HttpContext.Request.Form.Files.Any())
				return new UnsupportedMediaTypeResult();
			//if (!Request.Content.IsMimeMultipartContent())
			//{
			//	return new UnsupportedMediaTypeResult();
			//	//throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			//}

			//string root = HttpContext.Current.Server.MapPath("~/TempResources");
			string root = Path.Combine(_hostEnvironment.WebRootPath, $"TempResources");
			var provider = new MultipartFormDataStreamProvider(root);

			try
			{

				// Read the form data and return an async task. singlefile.Headers.ContentDisposition.Name
				await Request.Content.ReadAsMultipartAsync(provider);

				var clientResourceName = "resource";
				var clientThumbName = "thumb";
				var resourceNameFromJson = string.Empty;
				var thumbNameFromJson = string.Empty;
				FileInfo fileInfo = null;
				FileInfo thumbFileInfo = null;

				foreach (var singlefile in provider.FileData)
				{
					var localName = Json.Decode(singlefile.Headers.ContentDisposition.Name);
					var localresourceName = Json.Decode(singlefile.Headers.ContentDisposition.FileName);
					if (string.Equals(localName, clientResourceName, StringComparison.OrdinalIgnoreCase))
					{
						resourceNameFromJson = localresourceName;
						fileInfo = new FileInfo(singlefile.LocalFileName);
					}
					else if (string.Equals(localName, clientThumbName, StringComparison.OrdinalIgnoreCase))
					{
						thumbNameFromJson = localresourceName;
						thumbFileInfo = new FileInfo(singlefile.LocalFileName);
					}
				}
				var idString = provider.FormData.Get("Id");
				var id = 0;
				if (!string.IsNullOrEmpty(idString))
				{
					id = Int32.Parse(idString);
				}
				if (id == 0)
				{
					return BadRequest();
					//return Request.CreateResponse(HttpStatusCode.BadRequest);
				}
				var oldResource = _facade.GetResourceById(id);
				var hasResource = !string.IsNullOrEmpty(resourceNameFromJson);
				var hasThumb = !string.IsNullOrEmpty(thumbNameFromJson);
				thumbNameFromJson = string.Concat(Guid.NewGuid(), "_", thumbNameFromJson);
				var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Resources/"), oldResource.Id.ToString());
				var resourceInfo = _facade.GetResourceInfoFromFormProvider(provider);
				// Check if no file.
				if (hasResource && hasThumb)
				{
					_facade.DeleteZipFile(path, oldResource.ResourceName);
					_facade.DeleteFile(path, oldResource.ThumbName);

					var newFilePath = Path.Combine(path.ToString(), resourceNameFromJson);
					var newThumbFilePath = Path.Combine(path.ToString(), thumbNameFromJson);

					System.IO.File.Move(fileInfo.FullName, newFilePath.ToString());
					System.IO.File.Move(thumbFileInfo.FullName, newThumbFilePath.ToString());
					oldResource.ResourceName = resourceNameFromJson;
					oldResource.ThumbName = thumbNameFromJson;
					_facade.CreateResZipFile(path, resourceNameFromJson);
				}
				else if (hasResource)
				{
					_facade.DeleteZipFile(path, oldResource.ResourceName);
					var newFilePath = Path.Combine(path.ToString(), resourceNameFromJson);
					System.IO.File.Move(fileInfo.FullName, newFilePath.ToString());
					//File.Move(fileInfo.FullName, newFilePath.ToString());
					oldResource.ResourceName = resourceNameFromJson;
					_facade.CreateResZipFile(path, resourceNameFromJson);
				}
				else if (hasThumb)
				{
					_facade.UnZipResourceFile(path, oldResource.ResourceName);
					_facade.DeleteZipFile(path, oldResource.ResourceName);
					_facade.DeleteFile(path, oldResource.ThumbName);
					var newThumbFilePath = Path.Combine(path.ToString(), thumbNameFromJson);
					//Directory.Move(thumbFileInfo.FullName, newThumbFilePath.ToString());
					System.IO.File.Move(thumbFileInfo.FullName, newThumbFilePath.ToString());
					oldResource.ThumbName = thumbNameFromJson;
					_facade.CreateResZipFile(path, oldResource.ResourceName);
				}


				oldResource.Title = resourceInfo.Title;
				oldResource.Description = resourceInfo.Description;
				oldResource.Rank = resourceInfo.Rank;
				oldResource.CategoryId = resourceInfo.CategoryId;
				oldResource.Tags.Clear();
				oldResource.Tags = resourceInfo.Tags;
				//Here we have to check Is model valid

				if (!_facade.UpdateResource(oldResource))
				{
					return StatusCode(StatusCodes.Status500InternalServerError, oldResource.ResourceName);
					//return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, oldResource.ResourceName);
				}

				return StatusCode(StatusCodes.Status201Created);
				//return Request.CreateResponse(HttpStatusCode.Created);
			}
			catch (System.Exception e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.HResult.ToString());
				//return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.HResult.ToString());
			}
		}

		[HttpGet("{tagName?}/{pageNumber?}")]
		public SearchResultsPaginationViewModel GetSearchResultsByTag(string tagName, int pageNumber)
		{
			if (!IsAuthorize("read"))
			{
				return null;
			}
			var searchResultsPagination = new SearchResultsPaginationViewModel();
			var data = this.GetSearchResultsViewModel(_facade.GetResourceByTagName(tagName).ToList());
			if (data != null)
			{
				var totalData = data.Count();

				double resultToDisplay = 18;
				double pagecount = Math.Ceiling(totalData / resultToDisplay);
				int totalPage = totalData > resultToDisplay ? Convert.ToInt32(pagecount) : 1;
				int actualDisplay = totalData < resultToDisplay ? totalData : Convert.ToInt32(resultToDisplay);

				searchResultsPagination.SearchResult = data.Skip(actualDisplay * pageNumber).Take(actualDisplay);
				searchResultsPagination.TotalPage = totalPage;
			}
			return searchResultsPagination;
		}

		private bool IsAuthorize(string permissonType)
		{
			return _fileManagerAuth.IsAuthorize("Resource", permissonType);
		}
		private bool IsAuthorizeForDownload()
		{
			return _fileManagerAuth.IsAuthorize("Download", "read");
		}
	}
}

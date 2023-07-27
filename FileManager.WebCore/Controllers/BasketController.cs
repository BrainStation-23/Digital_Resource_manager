using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using FileManager.DAL.DataContext;
using FileManager.Model;
using FileManager.Web.ViewModel;
using System.IO;
//using System.IO.Compression;
using System.Net.Http.Headers;
using System.Data;
using FileManager.BusinessFacade;
using Microsoft.AspNetCore.Mvc;

namespace FileManager.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BasketController : ControllerBase
	{
		private FileManagerAuthorizationFacade _fileManagerAuth = new FileManagerAuthorizationFacade();
		private Facade _facade = new Facade();

		[HttpGet]
		// GET api/<controller>
		public BasketCollectionViewModel Get()
		{
			Guid userId = _fileManagerAuth.GetCurrentUserId();
			BasketCollectionViewModel basketCollectionViewModel = new BasketCollectionViewModel();
			List<BasketViewModel> basketViewModelList = new List<BasketViewModel>();

			List<Basket> baskets = _facade.GetBuskets().ToList();
			if (baskets == null || baskets.Count == 0)
				return basketCollectionViewModel;

			foreach (Basket item in baskets)
			{
				ResourceInfo resourceInfo = _facade.GetResourceById(item.ResourceId);

				BasketViewModel basketViewModel = new BasketViewModel();
				basketViewModel.ResourceId = item.ResourceId;
				basketViewModel.ResourceTitle = resourceInfo.Title;
				basketViewModel.ResourceThumbPath = Path.Combine("/Resources/" + resourceInfo.Id.ToString() + "/", resourceInfo.ThumbName);

				basketViewModelList.Add(basketViewModel);
			}

			basketCollectionViewModel.BasketViewModelList = basketViewModelList;
			string zipFolder = "#";
			if (baskets.Count > 0)
				zipFolder = Path.Combine("/Downloadbasket/", userId.ToString() + ".zip");
			basketCollectionViewModel.Zippath = zipFolder;
			return basketCollectionViewModel;
		}

		[HttpPost("{basket?}")]


		// POST api/<controller>
		public bool Post(Basket basket)
		{
			Guid userId = _fileManagerAuth.GetCurrentUserId();
			try
			{
				Basket basketPrevious = _facade.GetBasketByResourceIdAndUserId(basket.ResourceId, userId);
				if (basketPrevious == null)
				{
					basket.UserId = userId;
					return _facade.AddBusket(basket);
				}
			}
			catch (Exception e)
			{

			}
			return false;
		}

		[ActionName("PostDownloadComplete")]
		[HttpPost("{username?}")]
		public string PostDownloadComplete(string username)
		{
			return _facade.DowloadBasketAllFiles();
		}

		[HttpDelete("{id?}")]



		// DELETE api/<controller>/5
		public ActionResult<Basket> Delete(long id)
		{
			var userId = _fileManagerAuth.GetCurrentUserId();
			Basket basket = _facade.GetBasketByResourceIdAndUserId(id, userId);
			if (basket == null)
			{
				return NotFound();
				//return new HttpResponseMessage(HttpStatusCode.NotFound);
				//return Request.CreateResponse(HttpStatusCode.NoContent);
			}

			if (_facade.RemoveBasketResourceByBasket(basket))
				return new OkObjectResult(basket);
			//return new HttpResponseMessage(HttpStatusCode.OK, basket);
			//return Request.CreateResponse(HttpStatusCode.OK, basket);
			else
				return NotFound();
				//return Request.CreateResponse(HttpStatusCode.NotFound);
		}

		/*public HttpResponseMessage Post(string download)
        {
            var path = _facade.DowloadBasketAllFiles();
            path = System.Web.HttpContext.Current.Server.MapPath(path);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new System.IO.FileStream(path, System.IO.FileMode.Open);
            var content = new StreamContent(stream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            response.Content = content;
            return response;
        }*/


	}
}
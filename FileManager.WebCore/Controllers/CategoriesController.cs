using System;
using System.Collections.Generic;
using System.Data;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using FileManager.Model;
using FileManager.DAL.DataContext;
using FileManager.Web.ViewModel;
using FileManager.BusinessFacade;
using Microsoft.AspNetCore.Mvc;

namespace FileManager.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CategoriesController : ControllerBase
	{
		private FileManagerAuthorizationFacade _fileManagerAuth = new FileManagerAuthorizationFacade();
		private Facade _facade = new Facade();

		[HttpGet]
		// GET api/Category
		public IEnumerable<CategoryViewModel> GetCategories()
		{
			return GetCategoryViewModel(_facade.GetCategories());
		}

		/// <summary>
		/// This function is used to create the view model
		/// </summary>
		/// <param name="_categories"></param>
		/// <returns></returns>
		private IEnumerable<CategoryViewModel> GetCategoryViewModel(IEnumerable<Category> _categories)
		{
			List<CategoryViewModel> _categoriesVM = new List<CategoryViewModel>();


			foreach (var item in _categories)
			{
				var category = new CategoryViewModel()
				{
					Id = item.Id,
					Title = item.Title,
					ParentCategory = (item.ParentsCategory == null) ? null : item.ParentsCategory.Title,
					CategoryId = (item.ParentsCategory == null) ? 0 : item.ParentsCategory.Id
				};
				_categoriesVM.Add(category);
			}
			return _categoriesVM;
		}

		[HttpGet("{id?}")]

		// GET api/Category/5
		public ActionResult<Category> GetCategory(int id)
		{
			if (!IsAuthorize("read"))
			{
				return null;
			}
			Category category = _facade.GetCategoryById(id);
			if (category == null)
			{
				//throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
			}

			return category;
		}

		[HttpGet("{categoryTitle?}")]
		public bool GetCategoryExistOrNot(string categoryTitle)
		{
			Category category = _facade.GetCategoryByTitle(categoryTitle);
			if (category == null)
			{
				return false;
			}
			return true;
		}

		[HttpGet("{catid?}")]
		//Maybe unused method
		public List<CategoryViewModel> GetChildrenByParentID(int catid)
		{
			if (!IsAuthorize("read"))
			{
				return null;
			}
			List<CategoryViewModel> categoryViewModelList = new List<CategoryViewModel>();
			CategoryViewModel categoryViewModel = null;



			var categories = _facade.GetChildrenByParentID(catid);

			categories.ForEach(x =>
			{

				categoryViewModelList.Add(new CategoryViewModel
				{
					Id = x.Id,
					Title = x.Title
				});

			});


			return categoryViewModelList;
		}

		[HttpPut("{id?}/{category?}")]


		//changed from HttpResponseMessage return to ActionResult<Category>
		// PUT api/Category/5
		public ActionResult<Category> PutCategory(int id, Category category)
		{
			if (!IsAuthorize("write"))
			{
				return Unauthorized();
				//return Request.CreateResponse(HttpStatusCode.Unauthorized);
			}
			if (ModelState.IsValid && id == category.Id)
			{
				if (_facade.UpdateCategory(category))
					return new OkObjectResult(category);
				//return Request.CreateResponse(HttpStatusCode.OK, category);
				else
					return StatusCode(304);
					//return Request.CreateResponse(HttpStatusCode.NotModified);
			}
			else
			{
				return BadRequest();
				//return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}

		//Changed from HttpResponseMessage to ActionResult<Category>
		[HttpPost("{item?}")]
		// POST api/Category
		public ActionResult<Category> PostCategory(Category item)
		{
			if (!IsAuthorize("write"))
			{
				return Unauthorized();
				//return Request.CreateResponse(HttpStatusCode.Unauthorized);
			}
			if (ModelState.IsValid)
			{
				_facade.AddCategory(item);
				_facade.TagsSaveIfNotExists(item);
				//HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, item);
				//response.Headers.Location = new Uri(Url.Link("DefaultApi", new
				//{
				//	id = item.Id
				//}));
				//return response;
				return new OkObjectResult(item);
			}
			else
			{
				return BadRequest();
				//return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}

		[HttpDelete("{id?}")]

		//Changed from HttpResponseMessage to ActionResult<int>
		// DELETE api/Category/5
		public ActionResult<string> DeleteCategory(int id)
		{
			if (!IsAuthorize("delete"))
			{
				return Unauthorized();
				//return Request.CreateResponse(HttpStatusCode.Unauthorized);
			}

			if (_facade.DeleteCategory(id))
				return Ok(id.ToString());
				//return Request.CreateResponse(HttpStatusCode.OK, id.ToString());
			else
				return NotFound();
				//return Request.CreateResponse(HttpStatusCode.NotFound);

		}

		[HttpGet("{catId?}")]
		//Maybe unused method
		public List<CategoryViewModel> GetCategoriesByParentId(int catId)
		{
			if (!IsAuthorize("read"))
			{
				return null;
			}
			List<CategoryViewModel> categoriesViewModel = new List<CategoryViewModel>();
			var categories = _facade.GetChildrenByParentID(catId);
			if (categories.Count == 0)
			{
				return categoriesViewModel;
			}
			categories.ForEach(x =>
			{
				categoriesViewModel.Add(new CategoryViewModel
				{
					Id = x.Id,
					Title = x.Title
				});
			});
			return categoriesViewModel;
		}

		protected void Dispose(bool disposing)
		{
			//db.Dispose();
			//base.Dispose(disposing);
		}

		private bool IsAuthorize(string permissonType)
		{
			return _fileManagerAuth.IsAuthorize("Category", permissonType);
		}
	}
}
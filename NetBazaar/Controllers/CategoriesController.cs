using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NetBazaar.BusinessLogic;
using NetBazaar.BusinessLogic.Interfaces;
using NetBazaar.Dalc.Dtos;
using NetBazaar.Dalc.Interfaces;
using NetBazaar.ViewModels.ActionResultViewModels;
using NetBazaar.ViewModels.CategoryViewModels;
using NetBazaar.ViewModels.Common;

namespace NetBazaar.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CategoriesController : Controller
    {
        private readonly ICategoriesStore _categoriesStore;
        private readonly IImageHostingService _imageHostingService;
        private readonly NetBazaarDatabase _netBazaarDatabase;

        public CategoriesController(ICategoriesStore categoriesStore, IImageHostingService imageHostingService,
            NetBazaarDatabase netBazaarDatabase)
        {
            _categoriesStore = categoriesStore;
            _imageHostingService = imageHostingService;
            _netBazaarDatabase = netBazaarDatabase;
        }

        // GET: Categories
        public async Task<ActionResult> Index()
        {
            return View("Index");
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View("Create");
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public async Task<ActionResult> CreateCategory(CategoryViewModel createCategoryViewModel)
        {
            await _categoriesStore.CreateAsync(createCategoryViewModel);
            return
                new JsonNetResult(new GetActionResultViewModel<long>
                {
                    IsSuccessful = true,
                    Data = createCategoryViewModel.Id
                });
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public async Task<ActionResult> SaveCategoryImage(long categoryId, HttpPostedFileBase categoryImage)
        {
            var savedCategoryImage = await _imageHostingService.SaveCategoryImage(categoryImage, categoryId);

            return
                new JsonNetResult(new GetActionResultViewModel<ImageViewModel>
                {
                    IsSuccessful = true,
                    Data = new ImageViewModel {Id = savedCategoryImage.Id, Url = savedCategoryImage.Url}
                });
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public async Task<ActionResult> GetCategory(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var category = await _categoriesStore.GetCategoryByIdAsync(id.Value);


            return new JsonNetResult(new GetActionResultViewModel<CategoryViewModel>
            {
                IsSuccessful = true,
                Data = category
            });
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public async Task<ActionResult> GetCategories()
        {
            var categories = await _categoriesStore.GetCategoriesAsync();
            var categoriesList = new List<CategoryViewModel>();

            foreach (var category in categories)
            {
                categoriesList.Add(category);
            }


            return new JsonNetResult(new GetActionResultViewModel<List<CategoryViewModel>>
            {
                IsSuccessful = true,
                Data = categoriesList
            });
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public async Task<ActionResult> SaveCategory(CategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                await _categoriesStore.SaveAsync(category);
            }

            return Json(new BasicActionResultViewModel {IsSuccessful = true});
        }

        // GET: Categories/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            return View("Edit");
        }

        public async Task<ActionResult> DeleteCategory(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var categoryViewModel = await _categoriesStore.GetCategoryByIdAsync(id.Value);

            await _categoriesStore.DeleteAsync(id.Value);

            await _imageHostingService.DeleteImagAsync(categoryViewModel.Image.Id);

            return Json(new BasicActionResultViewModel {IsSuccessful = true});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _netBazaarDatabase.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
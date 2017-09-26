using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using MongoDB.Driver;
using NetBazaar.BusinessLogic;
using NetBazaar.BusinessLogic.Interfaces;
using NetBazaar.Dalc.Dtos;
using NetBazaar.Dalc.Interfaces;
using NetBazaar.ViewModels.ActionResultViewModels;
using NetBazaar.ViewModels.CategoryViewModels;
using NetBazaar.ViewModels.PostingViewModels;

namespace NetBazaar.Controllers
{
    [Authorize(Roles = "Administrator,Member")]
    public class PostingsController : Controller
    {
        private readonly ICategoriesStore _categoriesStore;
        private readonly IImageHostingService _imageHostingService;
        private readonly NetBazaarDatabase _netBazaarDatabase;
        private readonly IPostingsStore _postingsStore;
        private IMongoDatabase _netBazaarMongoDb;

        public PostingsController(IPostingsStore postingsStore, ICategoriesStore categoriesStore,
            IImageHostingService imageHostingService, NetBazaarDatabase netBazaarDatabase,
            IMongoDatabase netBazaarMongoDb)
        {
            _postingsStore = postingsStore;
            _categoriesStore = categoriesStore;
            _imageHostingService = imageHostingService;
            _netBazaarDatabase = netBazaarDatabase;
            _netBazaarMongoDb = netBazaarMongoDb;
        }

        // GET: Postings
        public async Task<ActionResult> Index()
        {
            return View("Categories");
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

        public async Task<ActionResult> CategoryPostings(long categoryId)
        {
            ViewBag.CategoryId = categoryId;
            return View("CategoryPostings");
        }

        public async Task<ActionResult> GetCategoryPostings(long categoryId)
        {
            var category = await _categoriesStore.GetCategoryByIdAsync(categoryId);
            var postingsFromCategory = await _postingsStore.GetPostingsFromCategory(categoryId, 10000, 1);

            return new JsonNetResult(new GetActionResultViewModel<PostingsWithCategoryViewModel>
            {
                IsSuccessful = true,
                Data = new PostingsWithCategoryViewModel {Category = category, Postings = postingsFromCategory}
            });
        }

        // GET: Postings/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var posting = await _netBazaarDatabase.Postings.FindAsync(id);
            if (posting == null)
            {
                return HttpNotFound();
            }
            return View(posting);
        }

        // GET: Postings/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(_netBazaarDatabase.AspNetUsers, "Id", "Hometown");
            ViewBag.CategoryId = new SelectList(_netBazaarDatabase.Categories, "Id", "Name");
            return View();
        }

        // POST: Postings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,UserId,CategoryId,Type,Title,Description,PostingDate,ExpirationDate")] Posting posting)
        {
            if (ModelState.IsValid)
            {
                _netBazaarDatabase.Postings.Add(posting);
                await _netBazaarDatabase.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(_netBazaarDatabase.AspNetUsers, "Id", "Hometown", posting.UserId);
            ViewBag.CategoryId = new SelectList(_netBazaarDatabase.Categories, "Id", "Name", posting.CategoryId);
            return View(posting);
        }

        // GET: Postings/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var posting = await _netBazaarDatabase.Postings.FindAsync(id);
            if (posting == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(_netBazaarDatabase.AspNetUsers, "Id", "Hometown", posting.UserId);
            ViewBag.CategoryId = new SelectList(_netBazaarDatabase.Categories, "Id", "Name", posting.CategoryId);
            return View(posting);
        }

        // POST: Postings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,UserId,CategoryId,Type,Title,Description,PostingDate,ExpirationDate")] Posting posting)
        {
            if (ModelState.IsValid)
            {
                _netBazaarDatabase.Entry(posting).State = EntityState.Modified;
                await _netBazaarDatabase.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(_netBazaarDatabase.AspNetUsers, "Id", "Hometown", posting.UserId);
            ViewBag.CategoryId = new SelectList(_netBazaarDatabase.Categories, "Id", "Name", posting.CategoryId);
            return View(posting);
        }

        // GET: Postings/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var posting = await _netBazaarDatabase.Postings.FindAsync(id);
            if (posting == null)
            {
                return HttpNotFound();
            }
            return View(posting);
        }

        // POST: Postings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            var posting = await _netBazaarDatabase.Postings.FindAsync(id);
            _netBazaarDatabase.Postings.Remove(posting);
            await _netBazaarDatabase.SaveChangesAsync();
            return RedirectToAction("Index");
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
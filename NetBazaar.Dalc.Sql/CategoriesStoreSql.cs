using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetBazaar.Dalc.Dtos;
using NetBazaar.Dalc.Interfaces;
using NetBazaar.ViewModels.CategoryViewModels;

namespace NetBazaar.Dalc.Sql
{
    public class CategoriesStoreSql : BaseStoreSql, ICategoriesStore
    {
        public CategoriesStoreSql(NetBazaarDatabase netBazaarDatabase) : base(netBazaarDatabase)
        {
        }

        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            return _netBazaarDatabase.Categories.ToList().Select(category => category.ToViewModel()).ToList();
        }

        public async Task<CategoryViewModel> GetCategoryByIdAsync(long categoryId)
        {
            return _netBazaarDatabase.Categories.Include("CategoryFields").Include("ImageReference")
                .First(category => category.Id == categoryId).ToViewModel();
        }

        public async Task CreateAsync(CategoryViewModel categoryViewModel)
        {
            var category = _netBazaarDatabase.Categories.Add(new Category(categoryViewModel));

            _netBazaarDatabase.SaveChanges();

            categoryViewModel.Id = category.Id;
        }

        public Task SaveAsync(CategoryViewModel categoryViewModel)
        {
            var category = _netBazaarDatabase.Categories.First(cat => cat.Id == categoryViewModel.Id);

            category.UpdateFromViewModel(categoryViewModel);

            return _netBazaarDatabase.SaveChangesAsync();
        }

        public Task<ImageReference> SaveImage(ImageReference image, long categoryId)
        {
            var categoryById = _netBazaarDatabase.Categories.First(cat => cat.Id == categoryId);
            ImageReference previousImageId = null;

            if (categoryById.ImageReference != null)
            {
                previousImageId = categoryById.ImageReference;
            }

            categoryById.ImageReference = image;

            _netBazaarDatabase.SaveChanges();

            return Task.FromResult(previousImageId);
        }

        public Task DeleteAsync(long categoryId)
        {
            var existingCategory = _netBazaarDatabase.Categories.Include("CategoryFields")
                .First(category => category.Id == categoryId);

            _netBazaarDatabase.CategoryFields.RemoveRange(existingCategory.CategoryFields);
            _netBazaarDatabase.Categories.Remove(existingCategory);

            return _netBazaarDatabase.SaveChangesAsync();
        }
    }
}
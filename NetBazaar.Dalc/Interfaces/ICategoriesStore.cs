using System.Collections.Generic;
using System.Threading.Tasks;
using NetBazaar.Dalc.Dtos;
using NetBazaar.ViewModels.CategoryViewModels;

namespace NetBazaar.Dalc.Interfaces
{
    public interface ICategoriesStore
    {
        Task DeleteAsync(long categoryId);

        Task<List<CategoryViewModel>> GetCategoriesAsync();

        Task<CategoryViewModel> GetCategoryByIdAsync(long categoryId);

        Task CreateAsync(CategoryViewModel categoryViewModel);

        Task SaveAsync(CategoryViewModel categoryViewModel);

        Task<ImageReference> SaveImage(ImageReference image, long categoryId);
    }
}
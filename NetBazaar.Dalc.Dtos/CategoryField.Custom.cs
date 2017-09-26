using NetBazaar.ViewModels.CategoryViewModels;

namespace NetBazaar.Dalc.Dtos
{
    public partial class CategoryField
    {
        public CategoryField(CategoryFieldViewModel categoryFieldViewModel)
        {
            Id = categoryFieldViewModel.Id;
            CategoryId = categoryFieldViewModel.CategoryId;
            Name = categoryFieldViewModel.Name;
            Type = categoryFieldViewModel.Type;
        }
    }
}
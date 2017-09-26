using System.Collections.Generic;
using NetBazaar.ViewModels.Common;

namespace NetBazaar.ViewModels.CategoryViewModels
{
    public class CategoryViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<CategoryFieldViewModel> DeletedFields { get; set; } = new List<CategoryFieldViewModel>();

        public List<CategoryFieldViewModel> Fields { get; set; } = new List<CategoryFieldViewModel>();

        public ImageViewModel Image { get; set; }

        public long Id { get; set; }
    }
}
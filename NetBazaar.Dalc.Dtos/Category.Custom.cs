using System.Collections.Generic;
using System.Linq;
using NetBazaar.ViewModels.CategoryViewModels;
using NetBazaar.ViewModels.Common;

namespace NetBazaar.Dalc.Dtos
{
    public partial class Category
    {
        public Category(CategoryViewModel categoryViewModel)
        {
            Id = categoryViewModel.Id;
            Name = categoryViewModel.Name;
            Description = categoryViewModel.Description;

            CategoryFields = new List<CategoryField>();

            foreach (var categoryFieldViewModel in categoryViewModel.Fields)
            {
                CategoryFields.Add(new CategoryField(categoryFieldViewModel));
            }

            foreach (var deletedCategoryField in categoryViewModel.DeletedFields)
            {
                DeletedCategoryFields.Add(new CategoryField(deletedCategoryField));
            }
        }

        public List<CategoryField> DeletedCategoryFields { get; set; } = new List<CategoryField>();

        public void UpdateFromViewModel(CategoryViewModel categoryViewModel)
        {
            Name = categoryViewModel.Name;
            Description = categoryViewModel.Description;

            foreach (var categoryFieldViewModel in categoryViewModel.Fields)
            {
                var categoryField = CategoryFields.FirstOrDefault(field => field.Id == categoryFieldViewModel.Id);

                if (categoryField != null)
                {
                    categoryField.Name = categoryFieldViewModel.Name;
                    categoryField.Type = categoryFieldViewModel.Type;
                }
                else
                {
                    categoryField = new CategoryField();
                    categoryField.Name = categoryFieldViewModel.Name;
                    categoryField.Type = categoryFieldViewModel.Type;
                    CategoryFields.Add(categoryField);
                }
            }

            var deleteCategoryFields = CategoryFields.Where(
                field => categoryViewModel.DeletedFields.Any(deleted => deleted.Id == field.Id)).ToList();

            DeletedCategoryFields.AddRange(deleteCategoryFields);
        }

        public CategoryViewModel ToViewModel()
        {
            var categoryViewModel = new CategoryViewModel();

            categoryViewModel.Id = Id;
            categoryViewModel.Name = Name;
            categoryViewModel.Description = Description;

            foreach (var categoryField in CategoryFields)
            {
                var categoryFieldViewModel = new CategoryFieldViewModel();
                categoryFieldViewModel.Id = categoryField.Id;
                categoryFieldViewModel.Name = categoryField.Name;
                categoryFieldViewModel.Type = categoryField.Type;

                categoryViewModel.Fields.Add(categoryFieldViewModel);
            }

            categoryViewModel.Image = new ImageViewModel();

            if (ImageReference != null)
            {
                categoryViewModel.Image.Id = ImageReference.Id;
                categoryViewModel.Image.Url = ImageReference.Url;
            }

            return categoryViewModel;
        }
    }
}
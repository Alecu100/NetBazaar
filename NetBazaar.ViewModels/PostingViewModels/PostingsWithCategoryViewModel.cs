using System.Collections.Generic;
using NetBazaar.ViewModels.CategoryViewModels;

namespace NetBazaar.ViewModels.PostingViewModels
{
    public class PostingsWithCategoryViewModel
    {
        public List<PostingInfoViewModel> Postings { get; set; }

        public CategoryViewModel Category { get; set; }
    }
}
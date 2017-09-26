using System;
using System.Collections.Generic;
using NetBazaar.ViewModels.Common;

namespace NetBazaar.ViewModels.PostingViewModels
{
    public class PostingViewModel
    {
        public string Id { get; set; }

        public long CategoryId { get; set; }

        public long UserId { get; set; }

        public List<PostingFieldViewModel> Fields { get; set; } = new List<PostingFieldViewModel>();

        public List<ImageViewModel> Images { get; set; } = new List<ImageViewModel>();

        public ImageViewModel MainImage { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Type { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
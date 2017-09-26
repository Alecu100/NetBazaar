using NetBazaar.ViewModels.Common;

namespace NetBazaar.ViewModels.PostingViewModels
{
    public class PostingInfoViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ImageViewModel MainImage { get; set; }
    }
}
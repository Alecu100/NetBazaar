using System.Collections.Generic;
using System.Threading.Tasks;
using NetBazaar.Dalc.Dtos;
using NetBazaar.ViewModels.PostingViewModels;

namespace NetBazaar.Dalc.Interfaces
{
    public interface IPostingsStore
    {
        Task<List<PostingInfoViewModel>> GetPostingsFromCategory(long categoryId, int pageSize, int pageNumber);

        Task<PostingViewModel> GetPosting(string postingId);

        Task InsertPosting(PostingViewModel postingViewModel);

        Task UpdatePosting(PostingViewModel posting);

        Task DeletePosting(string postingId);

        Task<ImageReference> SaveMainImage(string postingId, ImageReference imageReference);

        Task SaveImage(string postingId, ImageReference imageReference);

        Task DeleteImage(string postingId, ImageReference imageReference);
    }
}
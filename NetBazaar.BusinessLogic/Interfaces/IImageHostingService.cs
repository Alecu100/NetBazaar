using System.Threading.Tasks;
using System.Web;
using NetBazaar.Dalc.Dtos;

namespace NetBazaar.BusinessLogic.Interfaces
{
    public interface IImageHostingService
    {
        Task<ImageReference> SaveCategoryImage(HttpPostedFileBase image, long categoryId);

        Task<ImageReference> SavePostImage(HttpPostedFileBase image);

        Task DeleteImagAsync(long imageReferenceId);
    }
}
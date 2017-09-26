using System.Threading.Tasks;
using NetBazaar.Dalc.Dtos;

namespace NetBazaar.Dalc.Interfaces
{
    public interface IImageReferencesStore
    {
        Task SaveImageReferenceAsync(ImageReference imageReference);

        Task DeleteImageReferenceAsync(ImageReference imageReference);

        Task<ImageReference> GetImageReferenceByIdAsync(long imageReferenceId);
    }
}
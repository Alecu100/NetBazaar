using System.Data.Entity;
using System.Threading.Tasks;
using NetBazaar.Dalc.Dtos;
using NetBazaar.Dalc.Interfaces;

namespace NetBazaar.Dalc.Sql
{
    public class ImageReferencesStoreSql : BaseStoreSql, IImageReferencesStore
    {
        public ImageReferencesStoreSql(NetBazaarDatabase netBazaarDatabase) : base(netBazaarDatabase)
        {
        }

        public Task SaveImageReferenceAsync(ImageReference imageReference)
        {
            _netBazaarDatabase.ImageReferences.Add(imageReference);

            return _netBazaarDatabase.SaveChangesAsync();
        }

        public Task DeleteImageReferenceAsync(ImageReference imageReference)
        {
            _netBazaarDatabase.ImageReferences.Remove(imageReference);

            return _netBazaarDatabase.SaveChangesAsync();
        }

        public Task<ImageReference> GetImageReferenceByIdAsync(long imageReferenceId)
        {
            return _netBazaarDatabase.ImageReferences.FirstAsync(image => image.Id == imageReferenceId);
        }
    }
}
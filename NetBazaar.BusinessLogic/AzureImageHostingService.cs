using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NetBazaar.BusinessLogic.Interfaces;
using NetBazaar.Dalc.Dtos;
using NetBazaar.Dalc.Interfaces;

namespace NetBazaar.BusinessLogic
{
    public class AzureImageHostingService : IImageHostingService
    {
        private readonly ICategoriesStore _categoriesStore;
        private readonly CloudBlobClient _cloudBlobClient;

        private readonly IGeneralSettingsCache _generalSettingsCache;

        private readonly IImageReferencesStore _imageReferencesStore;

        public AzureImageHostingService(IImageReferencesStore imageReferencesStore, ICategoriesStore categoriesStore,
            IGeneralSettingsCache generalSettingsCache)
        {
            var storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);

            _cloudBlobClient = storageAccount.CreateCloudBlobClient();

            _imageReferencesStore = imageReferencesStore;

            _categoriesStore = categoriesStore;

            _generalSettingsCache = generalSettingsCache;
        }

        public async Task<ImageReference> SaveCategoryImage(HttpPostedFileBase image, long categoryId)
        {
            var cloudBlobContainer =
                _cloudBlobClient.GetContainerReference(_generalSettingsCache.AzureCategoriesContainer);

            cloudBlobContainer.CreateIfNotExists();
            cloudBlobContainer.SetPermissions(
                new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});

            var cloudBlobDirectory =
                cloudBlobContainer.GetDirectoryReference(_generalSettingsCache.AzureCategoriesImagesDirectory);
            var blockBlobReference = cloudBlobDirectory.GetBlockBlobReference(image.FileName);
            await blockBlobReference.UploadFromStreamAsync(image.InputStream);

            var imageReference = new ImageReference();
            imageReference.Provider = "Azure";
            imageReference.InternalAddress1 = _generalSettingsCache.AzureCategoriesContainer;
            imageReference.InternalAddress2 = _generalSettingsCache.AzureCategoriesImagesDirectory;
            imageReference.InternalAddress3 = image.FileName;
            imageReference.Url = string.Format(_generalSettingsCache.AzureCategoriesImagesUrlFormat,
                imageReference.InternalAddress1, imageReference.InternalAddress2, imageReference.InternalAddress3);

            await _imageReferencesStore.SaveImageReferenceAsync(imageReference);

            var previousImage = await _categoriesStore.SaveImage(imageReference, categoryId);

            if (previousImage != null)
            {
                await DeleteImageInternal(previousImage);
            }

            return imageReference;
        }

        public Task<ImageReference> SavePostImage(HttpPostedFileBase image)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteImagAsync(long imageReferenceId)
        {
            var imageReference = await _imageReferencesStore.GetImageReferenceByIdAsync(imageReferenceId);
            await DeleteImageInternal(imageReference);
        }

        private async Task DeleteImageInternal(ImageReference imageReference)
        {
            var cloudBlobContainer =
                _cloudBlobClient.GetContainerReference(imageReference.InternalAddress1);


            cloudBlobContainer.CreateIfNotExists();
            cloudBlobContainer.SetPermissions(
                new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});

            var cloudBlobDirectory =
                cloudBlobContainer.GetDirectoryReference(imageReference.InternalAddress2);
            var blockBlobReference = cloudBlobDirectory.GetBlockBlobReference(imageReference.InternalAddress3);
            await blockBlobReference.DeleteAsync();

            await _imageReferencesStore.DeleteImageReferenceAsync(imageReference);
        }
    }
}
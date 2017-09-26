using System.Threading.Tasks;
using System.Web.Mvc;
using NetBazaar.BusinessLogic.Interfaces;
using NetBazaar.Dalc.Interfaces;

namespace NetBazaar.BusinessLogic
{
    public class GeneralSettingsCache : IGeneralSettingsCache
    {
        private string _azureCategoriesImagesUrlFormat;
        private string _azureCategoriesContainer;
        private string _azureCategoriesImagesDirectory;
        

        public string AzureCategoriesImagesUrlFormat
        {
            get { return _azureCategoriesImagesUrlFormat; }
        }

        public string AzureCategoriesContainer
        {
            get { return _azureCategoriesContainer; }
        }

        public string AzureCategoriesImagesDirectory
        {
            get { return _azureCategoriesImagesDirectory; }
        }

        public async Task Refresh()
        {
            var generalSettingsStore =
                (IGeneralSettingsStore) DependencyResolver.Current.GetService(typeof(IGeneralSettingsStore));

            var generalSettings = await generalSettingsStore.GetGeneralSettingsAsync();

            foreach (var generalSetting in generalSettings)
            {
                if (generalSetting.Key == nameof(AzureCategoriesImagesUrlFormat))
                {
                    _azureCategoriesImagesUrlFormat = generalSetting.Value;
                    continue;
                }

                if (generalSetting.Key == nameof(AzureCategoriesContainer))
                {
                    _azureCategoriesContainer = generalSetting.Value;
                    continue;
                }

                if (generalSetting.Key == nameof(AzureCategoriesImagesDirectory))
                {
                    _azureCategoriesImagesDirectory = generalSetting.Value;
                }
            }
        }
    }
}
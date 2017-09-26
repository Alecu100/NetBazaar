using System.Threading.Tasks;

namespace NetBazaar.BusinessLogic.Interfaces
{
    public interface IGeneralSettingsCache
    {
        string AzureCategoriesImagesUrlFormat { get; }

        string AzureCategoriesContainer { get; }

        string AzureCategoriesImagesDirectory { get; }
        Task Refresh();
    }
}
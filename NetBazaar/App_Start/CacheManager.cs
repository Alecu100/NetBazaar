using System.Web.Mvc;
using NetBazaar.BusinessLogic.Interfaces;

namespace NetBazaar.App_Start
{
    public static class CacheManager
    {
        public static void RefreshInitialCache()
        {
            var generalSettingsCache =
                (IGeneralSettingsCache) DependencyResolver.Current.GetService(typeof(IGeneralSettingsCache));
            generalSettingsCache.Refresh();
        }
    }
}
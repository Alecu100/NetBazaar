using System.Collections.Generic;
using System.Threading.Tasks;
using NetBazaar.Dalc.Dtos;

namespace NetBazaar.Dalc.Interfaces
{
    public interface IGeneralSettingsStore
    {
        Task<List<GeneralSetting>> GetGeneralSettingsAsync();

        Task SaveGeneralSettingsAsync(GeneralSetting generalSetting);
    }
}
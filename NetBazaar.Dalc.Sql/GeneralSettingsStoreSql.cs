using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using NetBazaar.Dalc.Dtos;
using NetBazaar.Dalc.Interfaces;

namespace NetBazaar.Dalc.Sql
{
    public class GeneralSettingsStoreSql : BaseStoreSql, IGeneralSettingsStore
    {
        public GeneralSettingsStoreSql(NetBazaarDatabase netBazaarDatabase) : base(netBazaarDatabase)
        {
        }

        public Task<List<GeneralSetting>> GetGeneralSettingsAsync()
        {
            return _netBazaarDatabase.GeneralSettings.ToListAsync();
        }

        public Task SaveGeneralSettingsAsync(GeneralSetting generalSetting)
        {
            _netBazaarDatabase.GeneralSettings.Attach(generalSetting);

            return _netBazaarDatabase.SaveChangesAsync();
        }
    }
}
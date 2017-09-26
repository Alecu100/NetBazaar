using NetBazaar.Dalc.Dtos;

namespace NetBazaar.Dalc.Sql
{
    public class BaseStoreSql
    {
        protected NetBazaarDatabase _netBazaarDatabase;

        public BaseStoreSql(NetBazaarDatabase netBazaarDatabase)
        {
            _netBazaarDatabase = netBazaarDatabase;
        }
    }
}
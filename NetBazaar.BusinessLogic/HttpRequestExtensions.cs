using System.Linq;
using System.Web;

namespace NetBazaar.BusinessLogic
{
    public static class HttpRequestExtensions
    {
        public static string LastPathPart(this HttpRequestBase request)
        {
            var parts = request.Path.Split('/', '\\');

            return parts.LastOrDefault();
        }
    }
}
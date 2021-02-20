using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Hotels.API.Extensions
{
    public static class HttpContextExtensions
    {
        public static RouteValueDictionary ToRouteValues(this IQueryCollection queryCollection)
        {
            var routeValue = new RouteValueDictionary();

            foreach (var item in queryCollection)
            {
                // Model içerisndeki propertylerden filtre uygulanamabilir olanlara göre 
                // işlem yapılabilir 
                // Attribute = CanFilter gibi
                if (!string.IsNullOrEmpty(queryCollection[item.Key]))
                {
                    if (!routeValue.ContainsKey(item.Key))
                        routeValue.Add(item.Key, item.Value);
                }
            }

            return routeValue;
        }
    }
}

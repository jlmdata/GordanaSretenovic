using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using Newtonsoft.Json;


namespace Avensia.Storefront.Developertest
{
    public class DefaultExampleProductRepository : IProductRepository
    {

        private const string CacheKey = "products";

        /// <summary>
        /// Using System.Runtime.Caching reference
        /// Method GetProducts which returns list of products from memory if it is there,
        /// if not read products from json file with
        /// method GetProductsFromJson()
        /// </summary>
        /// <returns>IEnumerable IProductDto</returns>

        public IEnumerable<IProductDto> GetProducts()
        {

            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(CacheKey))
                return (IEnumerable<IProductDto>)cache.Get(CacheKey);
            var products = GetProductsFromJson();

            // Store data in the cache    
            var cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now.AddHours(1.0)
            };
            cache.Add(CacheKey, products, cacheItemPolicy);

            return products;
        }

        /// <summary>
        /// Read products.json 
        /// Deserialize Json to List DefaultProductDto
        /// </summary>
        /// <returns>IEnumerable IProductDto </returns>

        private static IEnumerable<IProductDto> GetProductsFromJson()
        {
            return JsonConvert.DeserializeObject<List<DefaultProductDto>>(File.ReadAllText(Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "products.json")), new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            });
        }
        /// <summary>
        /// Calling GetProducts() for products list
        /// Calling GetPage() for a list of products med page number depends on page size
        /// </summary>
        /// <param name="start"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        public IEnumerable<IProductDto> GetProducts(int start, int pageSize)
        {
            return GetPage(GetProducts(), start, pageSize);
        }

        /// <summary>
        /// returns products by page size and number of page
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private static IEnumerable<IProductDto> GetPage(IEnumerable<IProductDto> list, int pageNumber, int pageSize)
        {
            return list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
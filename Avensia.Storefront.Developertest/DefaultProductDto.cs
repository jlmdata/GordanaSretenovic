using Newtonsoft.Json;

namespace Avensia.Storefront.Developertest
{
    /// <summary>
    /// Class we deserialize Json in,
    /// with add attribute Json property name
    /// </summary>
    public class DefaultProductDto : IProductDto
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("ProductName")]
        public string Name { get; set; }

        [JsonProperty("Price")]
        public decimal Price { get; set; }
    }
}
namespace Avensia.Storefront.Developertest
{
    /// <summary>
    /// Class to be able to add new product prices after currency
    /// so that the IProductDto price does not change
    /// </summary>
    internal class DefaultProduct
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}

using Microsoft.Win32;
using StructureMap;
using Registry = StructureMap.Registry;


namespace Avensia.Storefront.Developertest
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            For<ProductListVisualizer>().Use<ProductListVisualizer>();
            For<IProductRepository>().Use<DefaultExampleProductRepository>();
        }
    }
}
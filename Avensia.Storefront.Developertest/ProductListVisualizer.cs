using System;
using System.Linq;
using System.Threading;

namespace Avensia.Storefront.Developertest
{
    public class ProductListVisualizer
    {
        private readonly IProductRepository _productRepository;

        public ProductListVisualizer(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void OutputAllProduct(string currency)
        {
            // Show products and products price  with currency converter from default currency to the selected currency
            // Add Thread.Sleep(500) so that the list is displayed more slowly (short product list)

            var products = _productRepository.GetProducts();
            foreach (var product in products)
            {
                Console.WriteLine($"{product.Id}\t{product.Name}\t{Math.Round(product.Price * (decimal)CurrencyConverter.GetExchangeRate("usd", currency),2)} {currency.ToUpper()}");
                Thread.Sleep(500);
            }
        }

        public void OutputPaginatedProducts(int start, int pageSize, string currency)
        {
            var products = _productRepository.GetProducts();
            var pageQuantity = products.Count() / pageSize;
            if (products.Count() % pageSize > 0)
                pageQuantity += 1;
            for (var i = start; i <= pageQuantity; i++)
            {
                var paginatedProducts = _productRepository.GetProducts(i,pageSize);
                Console.WriteLine(
                    $"{Environment.NewLine}Start page:{i}\tPage size: {pageSize}{Environment.NewLine} ");


                // Show products with products price with currency converter from default currency to the selected currency
                // Add Thread.Sleep(500) so that the list is displayed more slowly (short product list)

                foreach (var product in paginatedProducts)
                {
                    Console.WriteLine(
                        $"{product.Id}\t{product.Name}\t{Math.Round(product.Price * (decimal)CurrencyConverter.GetExchangeRate("usd", currency), 2)} {currency.ToUpper()}");
                    Thread.Sleep(500);
                }

            }

        }

        public void OutputProductGroupedByPriceSegment(string currency)
        {
            // start for price range
            decimal startPrice = 1;
            var products = _productRepository.GetProducts();
            // create a new list with default product so that the price in the main list remains unchanged
            // and calculate the price in the selected currency
            var productsWithNewPrice = products.Select(productDto => new DefaultProduct() { Id = productDto.Id, Name = productDto.Name, Price = Math.Round(productDto.Price * (decimal)CurrencyConverter.GetExchangeRate("usd", currency), 2) }).ToList();

            // as long as the starting price that is increased by a hundred in the loop is less than the largest price runs while
            while (startPrice < productsWithNewPrice.OrderByDescending(a => a.Price).First().Price)
            {
                //check if we have products with a price within the current range
                if (productsWithNewPrice.Count(a => a.Price >= startPrice && a.Price < startPrice + 100) > 0)
                {

                    //prints price range in the selected currency
                    Console.WriteLine(
                        $"{Environment.NewLine}{startPrice}-{startPrice + 99} {currency.ToUpper()}{Environment.NewLine} ");

                    foreach (var product in productsWithNewPrice.Where(a => a.Price >= startPrice && a.Price < startPrice + 100))
                    {
                        // prints products whose price is in the current price range
                        Console.WriteLine($"{product.Id}\t{product.Name}\t{product.Price} {currency.ToUpper()}");
                        Thread.Sleep(500);
                    }
                }
                // creates a new range by adding existing range for 100
                startPrice += 100;
            }
        }
    }
}
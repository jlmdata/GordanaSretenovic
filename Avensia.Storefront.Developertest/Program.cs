using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Container = StructureMap.Container;

namespace Avensia.Storefront.Developertest
{
    class Program
    {
       
        private static ProductListVisualizer _productListVisualizer;
        static List<Thread> threadsList = new List<Thread>();
       
        static void Main(string[] args)
        {
            //default startPage, default page size
            var _startPage = 1;
            var _pageSize = 10;
            //declares starting currency
            var currency = "usd";

            var childAllProducts = new Thread(() => CallToChildThread(currency));
            var childAllProductsGroupedByPrice = new Thread(() => CallToChildThreadGroupedByPrice(currency));
            var childAllProductsPaginated = new Thread(() => CallToChildThreadPaginated(currency, _startPage, _pageSize));


            //bool that holds info which thread is alive
            bool currencyAll = false, currencyGroupByPrice = false, currencyPaginated = false;
            
            var container = new Container(new DefaultRegistry());
            _productListVisualizer = container.GetInstance<ProductListVisualizer>();
            var shouldRun = true;
            ConsoleKeyInfo input;

            
            threadsList = new List<Thread> {childAllProductsPaginated,childAllProducts,childAllProductsGroupedByPrice};

            DisplayOptions();
           
            while (shouldRun)
            {
               
                //handles input when application is running
                input = Console.ReadKey();
                Console.WriteLine("\n");
           

                // checks whether the input is in connection with currency selection
                if (CurrencyConverter.GetCurrencyTags().Any(c => c.StartsWith(input.Key.ToString().ToUpper())))
                {
                    //reads elements in the list looking for currency by checking which element starts with the user's input
                    currency = CurrencyConverter.GetCurrencyTags()
                        .First(c => c.StartsWith(input.Key.ToString().ToUpper()));
                    _productListVisualizer = container.GetInstance<ProductListVisualizer>();


                    // check for which thread is running so that we manage to show the same list that has prices in the new currency (IsAlive)
                    switch (currencyAll)
                    {
                        
                        case true:
                            childAllProducts.Abort();
                            childAllProducts =
                                new Thread(() => CallToChildThread(currency));
                            // start a new thread with selected currency
                            childAllProducts.Start();
                            break;
                        default:
                        {
                            if (currencyGroupByPrice)
                            {
                                childAllProductsGroupedByPrice.Abort();
                                    childAllProductsGroupedByPrice =
                                        new Thread(() => CallToChildThreadGroupedByPrice(currency));
                                    // start a new thread with selected currency
                                    childAllProductsGroupedByPrice.Start();

                            }
                            else if (currencyPaginated)
                            {
                                childAllProductsPaginated.Abort();
                                    childAllProductsPaginated =
                                        new Thread(() => CallToChildThreadPaginated(currency, _startPage, _pageSize));
                                    // start a new thread with selected currency 
                                    childAllProductsPaginated.Start();
                               
                            }

                            break;
                        }
                    }
                    
                }
                else
                {
                    switch (input.Key)
                    {
                        case ConsoleKey.NumPad1:
                        case ConsoleKey.D1:
                            currencyAll = true;
                            currencyPaginated = currencyGroupByPrice = false;

                            childAllProducts.Abort();
                            childAllProducts =
                                new Thread(() => CallToChildThread(currency));

                            // start thread with default currency usd
                            childAllProducts.Start();
                            
                            break;
                        case ConsoleKey.NumPad2:
                        case ConsoleKey.D2:
                            currencyPaginated = true;
                            currencyAll = currencyGroupByPrice = false;
                            DisplayPaginatedOptionStart();
                            var inputStr = Console.ReadLine();
                            Console.WriteLine("\n");
                            //checks user input if m go to main menu
                            if (!string.IsNullOrEmpty(inputStr) && inputStr.ToUpper() == "M") break;

                            //checks user input for start page while not int call submenu
                            while (!string.IsNullOrEmpty(inputStr) && !int.TryParse(inputStr.TrimStart('D'), out _startPage))
                                DisplayOptionInvalid(true);

                            DisplayPaginatedOptionPageSize();
                            inputStr = Console.ReadLine();
                            Console.WriteLine("\n");
                            //checks user input  if m go to main menu
                            if (!string.IsNullOrEmpty(inputStr) && inputStr.ToUpper() == "M") break;

                            //checks user input for page size while not int call submenu
                            while (!string.IsNullOrEmpty(inputStr) && !int.TryParse(inputStr.TrimStart('D'), out _pageSize))
                                DisplayOptionInvalid(false);

                            childAllProductsPaginated.Abort();
                            childAllProductsPaginated =
                                new Thread(() => CallToChildThreadPaginated(currency, _startPage, _pageSize));

                            // start thread with default currency usd
                            childAllProductsPaginated.Start();
                            
                            break;
                        case ConsoleKey.NumPad3:
                        case ConsoleKey.D3:
                            currencyGroupByPrice = true;
                            currencyPaginated = currencyAll = false;
                            childAllProductsGroupedByPrice.Abort();
                            childAllProductsGroupedByPrice =
                                new Thread(() => CallToChildThreadGroupedByPrice(currency));

                            // start thread with default currency usd
                            childAllProductsGroupedByPrice.Start();
                            break;
                        case ConsoleKey.Q:
                            shouldRun = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option!");
                            currencyAll = currencyPaginated = currencyGroupByPrice = false;
                            break;
                    }

                    
                }
               
            }

            Console.Write("\n\rPress any key to exit!");
            Console.ReadKey();
        }

        /// <summary>
        /// checks if the thread is running otherwise calls the main menu
        /// </summary>
        /// <param name="tList"></param>

        private static void CallMainMenu(IEnumerable<Thread> tList)
        {
            var areDone = tList.All(t => !t.IsAlive);
            if (!areDone) return;
            Console.WriteLine();
            DisplayOptions();
        }


        /// Printing paginated product list
        /// with the default or selected currency,
        /// start page and page size
        /// <param name="currency"></param>
        /// <param name="startPage"></param>
        /// <param name="pageSize"></param>

        private static void CallToChildThreadPaginated(string currency, int startPage, int pageSize)
        {
            
            Console.WriteLine("Printing products paginated");
            _productListVisualizer.OutputPaginatedProducts(startPage, pageSize, currency); 
            CallMainMenu(threadsList);
        }

        /// <summary>
        /// shows an invalid option for a paginated product list
        /// </summary>
        /// <returns>user input</returns>

        private static void DisplayOptionInvalid(bool start)
        {
            Console.WriteLine("Invalid option!");
            Console.WriteLine("\n");
            if (start) DisplayPaginatedOptionStart();
            else DisplayPaginatedOptionPageSize();

        }
        /// <summary>
        /// Selected a page size for a paginated product list
        /// </summary>
        /// <returns>user input</returns>

        private static void DisplayPaginatedOptionPageSize()
        {
            Console.WriteLine("Choose a page size:");
            Console.WriteLine("m - Main Menu");
            //Console.WriteLine("\n");
           
        }
        /// <summary>
        /// Selected a start page for a paginated product list
        /// </summary>
        /// <returns>user input</returns>

        private static void DisplayPaginatedOptionStart()
        {
            Console.WriteLine("Choose a start page:");
            Console.WriteLine("m - Main Menu");
            //Console.WriteLine("\n");
           
        }


        /// <summary>
        /// Printing products grouped by price
        /// with the default or selected currency
        /// </summary>
        /// <param name="currency"></param>
        private static void CallToChildThreadGroupedByPrice(string currency)
        {
            Console.WriteLine("Printing products grouped by price");
            _productListVisualizer.OutputProductGroupedByPriceSegment(currency);
            CallMainMenu(threadsList);

        }

        /// <summary>
        /// Printing all products"
        /// with the default or selected currency
        /// </summary>
        /// <param name="currency"></param>

        private static void CallToChildThread(string currency)
        {
            Console.WriteLine("Printing all products");
            _productListVisualizer.OutputAllProduct(currency);
            CallMainMenu(threadsList);

        }


        private static void DisplayOptions()
        {
            Console.WriteLine("Current price currency is USD.\nTo change:\ne for EUR\nu for USD\nd for DKK\ng for GBP\ns for SEK\nn for NOK\nOnly when application is running!");
            Console.WriteLine("\n");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1 - Print all products");
            Console.WriteLine("2 - Print paginated products");
            Console.WriteLine("3 - Print products grouped by price");
            Console.WriteLine("q - Quit");
            Console.WriteLine("\n");
            Console.Write("Enter an option: ");
            //Console.WriteLine("\n");


        }
    }
}

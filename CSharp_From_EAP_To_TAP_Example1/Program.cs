using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSharp_From_EAP_To_TAP_Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            ThirdParty_Service thirdPartyService = new ThirdParty_Service();

            YourService yourService = new YourService(thirdPartyService);

            Parallel.For(1, 10, async i =>
            {
                MenuData menu = await yourService.GetMenuData(i.ToString());

                Console.WriteLine($"MenuId: {menu.MenuId}, Name: {menu.Name}");

                Console.WriteLine("======================================");
            });



            //Parallel.For(1, 5, async i =>
            //{
            //    IEnumerable<CategoryData> categories = await yourService.GetCategories(i.ToString());

            //    foreach (CategoryData item in categories)
            //    {
            //        Console.WriteLine($"MenuId: {item.MenuId}, Name: {item.Name}");
            //    }
            //    Console.WriteLine("======================================");
            //});


            Console.WriteLine("Hello World.");

            Console.ReadKey();
        }
    }

    class YourService
    {
        private readonly ThirdParty_Service _thirdPartyService;

        public YourService(ThirdParty_Service thirdPartyService)
        {
            _thirdPartyService = thirdPartyService;
        }

        public Task<MenuData> GetMenuData(string menuId)
        {
            TaskCompletionSource<MenuData> tcs = new TaskCompletionSource<MenuData>();

            _thirdPartyService.GetMenuData(menuId, (md) => tcs.SetResult(md));

            return tcs.Task;
        }

        public Task<IEnumerable<CategoryData>> GetCategories(string menuID)
        {
            var tcs = new TaskCompletionSource<IEnumerable<CategoryData>>();

            _thirdPartyService.GetCategories(menuID, (IEnumerable<CategoryData> categories) => tcs.SetResult(categories));

            return tcs.Task;
        }
    }


    /// <summary>
    /// ThirdParty Event-based Asynchronous Pattern Service
    /// </summary>
    class ThirdParty_Service
    {
        private Random random = new Random();

        public void GetMenuData(string menuId, Action<MenuData> getMenuDataCompleted)
        {
            Task.Run(() =>
            {
                Thread.Sleep(1000 + random.Next(1000, 9000));

                getMenuDataCompleted?.Invoke(new MenuData { MenuId = menuId, MenuText = $"MenuText_{menuId}" });
            });
        }

        public void GetCategories(string menuId, Action<IEnumerable<CategoryData>> getMenuDataCompleted)
        {
            int randomMilliseconds = 1000 + random.Next(1000, 9000);

            Task.Delay(randomMilliseconds)
                .ContinueWith(t =>
                {
                    IEnumerable<CategoryData> categories = Enumerable.Range(1, new Random().Next(1, 10)).Select(i => new CategoryData { CategoryId = i.ToString(), MenuId = menuId, Name = $"Category_{i}" });

                    getMenuDataCompleted?.Invoke(categories);
                });
        }
    }

    class CategoryData
    {
        public string CategoryId { get; set; }
        public string MenuId { get; set; }
        public string Name { get; set; }
    }

    class MenuData
    {
        public string MenuId { get; set; }
        public string MenuText { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public List<MenuData> SubMenus { get; set; }
    }
}
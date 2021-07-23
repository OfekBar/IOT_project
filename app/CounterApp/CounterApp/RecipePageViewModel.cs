/**
 * food page logic
**/
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace CounterApp
{
    public class RecipePageViewModel : INotifyPropertyChanged
    {
        public ICommand GetRecipeCommand { get; private set; }
        static HttpClient client = new HttpClient();
        static readonly string getProductsURL = "https://functionappinportal.azurewebsites.net/api/GetInventory";
        public event PropertyChangedEventHandler PropertyChanged;
        private List<Product> _products = new List<Product>(); //products in inventory

        private HashSet<string> checkedList = new HashSet<string>(); //products that are checked
        private List<RecipeUrl> recipiesUrls = new List<RecipeUrl>(); //products that are checked

        public List<Product> Products //products in inventory
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        // constructor
        public RecipePageViewModel()
        {
            // callcack for clicking get recepies button
            GetRecipeCommand = new Command<object>(async (object o) => {
                Console.WriteLine("hello1");
                // in task to not freaze app
                await Task.Run(async () => {

                    string url = "";
                    foreach(string name in checkedList)
                    {
  
                        url += name;
                        url += ",";
                    }
                    url = url.Remove(url.Length - 1);
                    Console.WriteLine(url);
                    //HttpResponseMessage result = CallUrl(url).Result;
                    //update recipies url list


                });

                recipiesUrls = new List<RecipeUrl>(); //products that are checked
                recipiesUrls.Add(new RecipeUrl("testURL"));
                recipiesUrls.Add(new RecipeUrl("abcd"));

                await Application.Current.MainPage.Navigation.PushAsync(new RecipesUrlsPage(recipiesUrls));

            });

            // build list of food from in fridge from hhtp call of data base
            var task = Task.Run(async () =>
            {
                try
                {
                     this.Products = ReadProducts().Result;
                }

                catch (Exception ex)
                {
                   //Console.WriteLine(ex);
                }
            });
        }

        // calls the url in the input
        public async Task<HttpResponseMessage> CallUrl(string url)
        {
            //Console.WriteLine(url);
            return await client.GetAsync(url);
        }


        //reads the products via http call to cloud function
        //returns list of products from azure table starage
        public async Task<List<Product>> ReadProducts()
        {
            //List<Product> pr = new List<Product>{ new Product("Milk", 30) , new Product("eggs", 2), new Product("cheese", 1) };
            HttpResponseMessage result = CallUrl(getProductsURL).Result;
            string jsonData = await result.Content.ReadAsStringAsync();
            Console.WriteLine(jsonData);
            List<Product> pr = JsonSerializer.Deserialize<List<Product>>(jsonData);
            Console.WriteLine(result.StatusCode);
            return pr;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void UpdateChecked(string Name)
        {
            if (!checkedList.Contains(Name))
            {
                checkedList.Add(Name);
            } else {
                checkedList.Remove(Name);
            }
            Console.WriteLine(checkedList.Count);
        }


    }
}

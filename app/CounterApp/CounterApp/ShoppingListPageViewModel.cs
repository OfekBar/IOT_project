/**
 * logic of shopping list page 
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


    public class ProductName
    {

        //what we show for each product in the shopping list
        public ProductName(string name)
        {
            this.Name = name;
          
        }

        [JsonPropertyName("RowKey")]
        public string Name { get; set; }
    }


    public class ShoppingListPageViewModel : INotifyPropertyChanged
    {

        //binding commands of buttons to add/remove products
        public ICommand removeProdClicked { get; private set; }
        public ICommand AddCommand { get; }

        static HttpClient client = new HttpClient();
        static readonly string getListURL = "https://functionappinportal.azurewebsites.net/api/getShoppingList";
        static readonly string addURL = "https://functionappinportal.azurewebsites.net/api/AddItemToShoppingList?name_to_add=";
        static readonly string removeURL = "https://functionappinportal.azurewebsites.net/api/RemoveProdFromShoppingList?to_romove_name=";

        public event PropertyChangedEventHandler PropertyChanged;

        //our shopping list
        private List<ProductName> _shoppingList = new List<ProductName>();

        public List<ProductName> ShoppingList
        {
            get => _shoppingList;
            set => SetProperty(ref _shoppingList, value);
        }

        //constructor
        public ShoppingListPageViewModel()
        {

            //button handler for removing items from list- in task to not freeze app
            removeProdClicked = new Command<object>(async (object o) => {

                ProductName p1 = (ProductName)o;

                await Task.Run(async () => {
                    var url = removeURL + p1.Name;
                    //Console.WriteLine(p1.Name);
                    HttpResponseMessage result = CallUrl(url).Result;
                    //Console.WriteLine(result.StatusCode);
                    this.ShoppingList = ReadList().Result;
                });
            });

            //button hanlder for adding new item to list
            AddCommand = new Command<string>(AddFunc);

            //build list of food from in fridge from hhtp call of database
            var task = Task.Run(async () =>
            {
                //Console.WriteLine("hello");
                try
                {
                     this.ShoppingList = ReadList().Result;
                }
                catch (Exception ex)
                {
                   //Console.WriteLine(ex);
                }
            });
        }

        //calls the url in the input
        public async Task<HttpResponseMessage> CallUrl(string url){
            //Console.WriteLine(url);
            return await client.GetAsync(url);
        }


        // reads the products via http call to cloud function
        // returns list of products from azure table starage
        public async Task<List<ProductName>> ReadList()
        {
            //List<Product> pr = new List<Product>{ new Product("Milk", 30) , new Product("eggs", 2), new Product("cheese", 1) };
            HttpResponseMessage result = CallUrl(getListURL).Result;
            string jsonData = await result.Content.ReadAsStringAsync();
            //Console.WriteLine(jsonData);
            List<ProductName> pr = JsonSerializer.Deserialize<List<ProductName>>(jsonData);
            //Console.WriteLine(result.StatusCode);
            //Console.WriteLine(pr[0].Name);    
            return pr;
        }


        // button hanlder for adding new item to list
        private void AddFunc(string parameter)
        {
            //Console.WriteLine("Add " + parameter);
            string result;
            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                result = await App.Current.MainPage.DisplayPromptAsync("Adding", "What do you want do add?");
                //Console.WriteLine(result);
                //task to not freeze app
                var task = Task.Run(async () =>
                {           
                    try
                    {
                        var URL = addURL;
                        URL += result;
                        Console.WriteLine(URL);
                        HttpResponseMessage respsone = CallUrl(URL).Result;
                        string jsonData = await respsone.Content.ReadAsStringAsync();
                        bool added = JsonSerializer.Deserialize<bool>(jsonData);
                        //Console.WriteLine(added);
                        if (added)
                        {
                            this.ShoppingList = ReadList().Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex);
                    }          
                });
            });


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
    }
}
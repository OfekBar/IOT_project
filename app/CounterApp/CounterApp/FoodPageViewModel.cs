/**
 * logic of inventory page, show products and amounts
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


namespace CounterApp
{
    public class Product
    {
        //product class
        public Product(string name, int amount, string barcode)
        {
            this.Name = name;
            this.Amount = amount;
            this.Barcode = barcode;
            
        }
        //for desirializing json from http returns
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }
    }

    public class FoodPageViewModel : INotifyPropertyChanged {

        public ICommand IncProdAmountClicked { get; private set; } //command for pressing + to inc products amount
        public ICommand DecProdAmountClicked { get; private set; } //command for pressing - to dec products amount
        //variables for using our cloud http trigger functions
        static HttpClient client = new HttpClient();
        static readonly string getProductsURL = "https://functionappinportal.azurewebsites.net/api/GetInventory";
        static readonly string incProdURL = "https://functionappinportal.azurewebsites.net/api/incProduct?barcode_input_number=";
        static readonly string decProdURL = "https://functionappinportal.azurewebsites.net/api/decProduct?barcode_input_number=";

        public event PropertyChangedEventHandler PropertyChanged;
        private List<Product> _products = new List<Product>(); //products in inventory
        public List<Product> Products //products in inventory
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        //constructor of food page
        public FoodPageViewModel(){

            //callcack function for clicking + button
            IncProdAmountClicked = new Command<object>(async (object o) => {
                Product p1 = (Product)o;
                //in task to not freaze app
                await Task.Run(async () => {
                    //Console.WriteLine(p1.Name);
                    //Console.WriteLine(p1.Barcode);
                    var url = incProdURL + p1.Barcode;
                    HttpResponseMessage result = CallUrl(url).Result;
                    //Console.WriteLine(result.StatusCode);
                    this.Products = ReadProducts().Result; //relodes screen
                });                
            });

            //callcack for clicking - button
            DecProdAmountClicked = new Command<object>(async (object o) => {
                Product p1 = (Product)o;
                //in task to not freaze app
                await Task.Run(async () => {
                    //Console.WriteLine(p1.Name);
                    //Console.WriteLine(p1.Barcode);
                    var url = decProdURL + p1.Barcode;
                    HttpResponseMessage result = CallUrl(url).Result;
                    //Console.WriteLine(result.StatusCode);
                    this.Products = ReadProducts().Result;  //relodes screen
                });       
            });

            //build list of inventory from azure table database
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

        //calls the url in the input- helper function
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
            //Console.WriteLine(result.StatusCode);
            //Console.WriteLine(pr[0].Name);
            //Console.WriteLine(pr[0].Barcode);
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

    }
}
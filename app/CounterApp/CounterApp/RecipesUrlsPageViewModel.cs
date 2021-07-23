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

    public class RecipeUrl
    {
        
        //what we show for each product
        public RecipeUrl(string name)
        {
            this.Name = name;
            
        }
        [JsonPropertyName("name")]
        public string Name { get; set; }

    }

    public class RecipesUrlsPageViewModel : INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<RecipeUrl> _urlList = new List<RecipeUrl>(); //products in inventory

        public List<RecipeUrl> UrlList
        {
            get => _urlList;
            set => SetProperty(ref _urlList, value);
        }

        // constructor
        public RecipesUrlsPageViewModel(List<RecipeUrl> urlList)
        {
                   
                    this.UrlList = urlList;   
                    Console.WriteLine(UrlList[0].Name);
                    Console.WriteLine(UrlList[1].Name);
                    Console.WriteLine("done1");

        }

        // constructor
        public RecipesUrlsPageViewModel()
        {
            //UrlList.Add(new RecipeUrl("from defult"));
            Console.WriteLine("done2");
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

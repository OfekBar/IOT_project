using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using ZXing;
using Xamarin.Essentials;
using System.Collections.Generic;

//ui for food page
namespace CounterApp
{
    public partial class RecipesUrlsPage : ContentPage
    {
       

        public RecipesUrlsPage(System.Collections.Generic.List<RecipeUrl> recipiesUrls)
        {
            InitializeComponent();
            this.BindingContext = new RecipesUrlsPageViewModel(recipiesUrls);
        }


    }

}

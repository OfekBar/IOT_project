using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using ZXing;
using Xamarin.Essentials;
using System.Collections.Generic;

//ui for recipies we get page
namespace CounterApp
{
    public partial class RecipesUrlsPage : ContentPage
    {
      
        public RecipesUrlsPage(List<RecipeUrl> recipiesUrls)
        {
            InitializeComponent();
            this.BindingContext = new RecipesUrlsPageViewModel(recipiesUrls);
        }


    }

}

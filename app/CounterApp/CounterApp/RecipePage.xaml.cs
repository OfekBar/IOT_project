using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using ZXing;
using Xamarin.Essentials;

//ui for food page
namespace CounterApp
{
    public partial class RecipePage : ContentPage
    {
        public RecipePage()
        {
            InitializeComponent();   

        }


        public void OnCheckClick(object sender, CheckedChangedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            string name = (string) cb.BindingContext;
            Console.WriteLine(name);
            RecipePageViewModel myModel = (RecipePageViewModel)this.BindingContext;
            myModel.UpdateChecked(name);
        }





    }

}

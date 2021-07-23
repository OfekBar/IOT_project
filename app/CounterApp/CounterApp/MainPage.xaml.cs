using System;
using Xamarin.Forms;
using Xamarin.Essentials;

//ui for ain page
namespace CounterApp
{
    public partial class MainPage : ContentPage
    {
      
        public MainPage()
        {
            InitializeComponent();
        }

        //scan button handler
        public void OnScanBarcodeClicked(object sender, EventArgs e)
        {
            //showing cammera
            MainLayout.IsVisible = false;
            BarcodeScanView.IsVisible = true;
            BarcodeScanView.IsScanning = true;
        }

        //food button handler- go to food page
        async public void OnFoodButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FoodPage());
        }

        //shopping list button handler- go to shopping list page
        async public void OnShoppingListButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ShoppingListPage());
        }

        //shopping list button handler- go to shopping list page
        async public void OnRecepiesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecipePage());
        }



        //call back- called when scanner found scan, adds product to inventory
        void OnScanResult(ZXing.Result result)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                BarcodeScanView.IsScanning = false;
                MainLayout.IsVisible = true;
                BarcodeScanView.IsVisible = false;
            });

            mainViewModel.ProcessBarcodeResult(result);
        }
    }

}

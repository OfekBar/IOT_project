using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZXing;
using Xamarin.Essentials;



namespace CounterApp
{

    /**
    public class CountersData
    {
        public int v1 { get; set; }
        public int v2 { get; set; }
    }**/

    public class MainViewModel : INotifyPropertyChanged
    {
        static HttpClient client = new HttpClient();

        /**static string updateURL = "https://functionappinportal.azurewebsites.net/api/UpdateCounter?";
        static string readCounterUrl = "https://functionappinportal.azurewebsites.net/api/getCounterHTTP";
        HubConnection connection;

        private int _counter1;
        public int Counter1
        {
            get => _counter1;
            set => SetProperty(ref _counter1, value);
        }

        private int _counter2;
        public int Counter2
        {
            get => _counter2;
            set => SetProperty(ref _counter2, value);
        }

        public ICommand IncrementCounterCommand { get; }

        **/

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            /**
            IncrementCounterCommand = new Command<string>(IncrementCounterById);

            Console.WriteLine("Connecting...");

            connection = new HubConnectionBuilder()
                .WithUrl("https://functionappinportal.azurewebsites.net/api")
                .Build();

            Console.WriteLine("Connection: " + connection.ToString());
            Console.WriteLine("Connection: " + connection.State + ", " + connection.ConnectionId);

            connection.Closed += async (error) =>
            {
                Console.WriteLine("Connection Closed");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync(); //TODO make this not in call back function
            };


            connection.On<string, string>("gotUpdate", (user, message) =>
            {
                Console.WriteLine("got signalR update: " + message);
                //from message read what counter has to be updated
                //and then update the vaulue from the vaulue in the message
            });

            var task = Task.Run(async () =>
            {
                Console.WriteLine("hello");
                try
                {
                    CountersData data = ReadCountes().Result;

                    //call getCounter HTTP to start counter1 and counter2 to value in table,
                    //give this device a certain ID so we can get the correct counters?

                    Counter1 = data.v1;
                    Counter2 = data.v2;

                    await connection.StartAsync(); //TODO make this not in call back function

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Console.WriteLine("Connection: " + connection.State + ", " + connection.ConnectionId);
            });

        }

        public async Task<CountersData> ReadCountes()
        {
            HttpResponseMessage result = CallUrl(readCounterUrl).Result;
            Console.WriteLine(result.StatusCode);
            // todo: throw error if statusCode != 200

            string jsonData = await result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CountersData>(jsonData);
        **/
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /**
        private void IncrementCounterById(string counterId)
        {
            Console.WriteLine("IncrementById: " + counterId);


            //call HTTP triggered read and update with counter ID and the new value it should get

            int counterVal = 0;

            if (counterId == "0")
                counterVal = Counter1 + 1;
            else if (counterId == "1")
                counterVal = Counter2 + 1;

            string myUrl = updateURL + "counterID=" + counterId + "&" + "newValue=" + counterVal.ToString() + "&deviceName=rowKey";


            var task = Task.Run(async () =>
            {
                HttpResponseMessage result = CallUrl(myUrl).Result;
                Console.WriteLine(result.StatusCode);

                //TODO: delete increments over here
                if (counterId == "0")
                    Counter1++;
                else if (counterId == "1")
                    Counter2++;
            });
        }
        **/

        public async Task<HttpResponseMessage> CallUrl(string url)
        {
            Console.WriteLine(url);
            return await client.GetAsync(url);
        }

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        static string BaseURL = "https://functionappinportal.azurewebsites.net/api/Add_product_to_list?barcode_input_number=";

        //callback function- when scanner retunrs result returns it to here
        public void ProcessBarcodeResult(ZXing.Result result)
        {
            Console.WriteLine(result.Text); // decoded text

            string myUrl = BaseURL + result.Text;
            var task = Task.Run(async () =>
            {
                HttpResponseMessage httpResult = CallUrl(myUrl).Result;
                Console.WriteLine(httpResult.StatusCode);

                string jsonData = await httpResult.Content.ReadAsStringAsync();
                Console.WriteLine(JsonSerializer.Deserialize<string>(jsonData));
            });
          
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AviaTickets.Models;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Net;
using System.Net.Http;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AviaTickets
{

    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ItemsPage : ContentPage
    {

        public List<Items> ItemsList { get; set; }

        string json = ""; // "{\"origin\":{\"iata\":\"LED\",\"name\":\"Санкт-Петербург\",\"country\":\"RU\",\"coordinates\":[30.315785,59.939039]},\"directions\":[{\"direct\":false,\"iata\":\"PMC\",\"name\":\"Пуэрто-Монт\",\"country\":\"CL\",\"country_name\":\"Чили\",\"coordinates\":[-73.09831,-41.433727],\"weight\":0,\"weather\":{\"weathertype\":null,\"temp_min\":null,\"temp_max\":null}},{\"direct\":false,\"iata\":\"LCH\",\"name\":\"Лейк-Чарльз\",\"country\":\"US\",\"country_name\":\"США\",\"coordinates\":[-93.2173758,30.2265949],\"weight\":0,\"weather\":{\"weathertype\":null,\"temp_min\":null,\"temp_max\":null}},{\"direct\":false,\"iata\":\"HJR\",\"name\":\"Кхаджурахо\",\"country\":\"IN\",\"country_name\":\"Индия\",\"coordinates\":[79.91642,24.818747],\"weight\":0,\"weather\":{\"weathertype\":null,\"temp_min\":null,\"temp_max\":null}},{\"direct\":false,\"iata\":\"BRA\",\"name\":\"Баррейрас\",\"country\":\"BR\",\"country_name\":\"Бразилия\",\"coordinates\":[-45.00833,-12.073056],\"weight\":0,\"weather\":{\"weathertype\":null,\"temp_min\":null,\"temp_max\":null}},{\"direct\":false,\"iata\":\"AMQ\",\"name\":\"Амбон\",\"country\":\"ID\",\"country_name\":\"Индонезия\",\"coordinates\":[128.08888,-3.704996],\"weight\":0,\"weather\":{\"weathertype\":null,\"temp_min\":null,\"temp_max\":null}}    ]}";


        public ItemsPage()
        {
            InitializeComponent();
        }


        private async void BtnSearch_Clicked(object sender, EventArgs e)
        {

            itemsList.ItemsSource = "";
            GlobalsVars.OriginIATA = TxtIATA.Text;
            TxtIATA.Text = "";

            Task GetItemJsonTask = new Task(GetItemJson);
            GetItemJsonTask.Start();

            HttpClient client = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();
            try {
                response = await client.GetAsync("http://map.aviasales.ru/supported_directions.json?origin_iata=" + GlobalsVars.OriginIATA + "&one_way=false&locale=ru");
            }
            catch {
                await DisplayAlert("Warning ", "No Connection", "OK");
                return;
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                HttpContent responseContent = response.Content;
                json = await responseContent.ReadAsStringAsync();

                JObject ObjJSON = JObject.Parse(json);
                GlobalsVars.OriginName = (string)ObjJSON.SelectToken("origin.name");

                Dictionary<string, object> ParseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                var ObjDirections = ParseDict["directions"];
                var Directions = JArray.Parse(ObjDirections.ToString());

                ItemsList = new List<Items>();
                ItemsList = Directions.Select(p => new Items
                {
                    iata = (string)p["iata"],
                    name = (string)p["name"],
                    country = (string)p["country"],
                    country_name = (string)p["country_name"]
                })
                .OrderBy(p => p.name)
                .ToList();

                itemsList.ItemsSource = ItemsList;
                
                this.BindingContext = this;

                Title = GlobalsVars.OriginName + "  ("+ GlobalsVars.OriginIATA+")";

            }
            else
            {
                itemsList.ItemsSource = "";
                Title = "";
                await DisplayAlert("Warning", "Wrong IATA Code", "OK");
            }
        }

            
            
        private async void GetItemJson()
        {
            HttpClient TaskClient = new HttpClient();
            HttpResponseMessage TaskResponse = new HttpResponseMessage();
            try
            {
                TaskResponse = await TaskClient.GetAsync("http://map.aviasales.ru/prices.json?origin_iata=" + GlobalsVars.OriginIATA + "&period=2020-05-01:season&direct=true&one_way=true&price=1000000&no_visa=false&schengen=true&need_visa=true&locale=ru&min_trip_duration_in_days=&max_trip_duration_in_days=365");
            }
            catch {
                return;
            }

            if (TaskResponse.StatusCode == HttpStatusCode.OK)
            {
                HttpContent TaskResponseContent = TaskResponse.Content;
                GlobalsVars.ItemInfoJson = await TaskResponseContent.ReadAsStringAsync();
            }
        }

        private void TxtIATA_TextChanged(object sender, TextChangedEventArgs e)
        {
            TxtIATA.Text = TxtIATA.Text.Trim();
            TxtIATA.Text = TxtIATA.Text.ToUpper();
        }

        //private async void ItemsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    Items selectedItem = e.SelectedItem as Items;
        //    await Navigation.PushAsync(new ItemInfoPage(selectedItem.iata, GlobalsVars.OriginName, selectedItem.name));
        //}

        public async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            Items selectedItem = e.Item as Items;
            if (selectedItem != null)
                await Navigation.PushAsync(new ItemInfoPage(selectedItem.iata, GlobalsVars.OriginName, selectedItem.name));
        }

    }
}
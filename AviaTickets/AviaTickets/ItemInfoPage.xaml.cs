using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AviaTickets.Models;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AviaTickets
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemInfoPage : ContentPage
	{

        public List<Item> ItemInfoList { get; set; }

        public ItemInfoPage (string IATA, string CityFrom, string CityTo)
		{
			InitializeComponent ();
            var Prices = JArray.Parse(GlobalsVars.ItemInfoJson);

            ItemInfoList = new List<Item>();
            ItemInfoList = Prices.Select(p => new Item
            {
                value = (float)p["value"],
                name = (string)p["name"],
                destination = (string)p["destination"],
                depart_date = (string)p["depart_date"],
                actual = (bool)p["actual"]
            }
            )
            .Where(p => p.destination == IATA.ToString() && p.actual == true).ToList();

            if (ItemInfoList.Count == 0)
            {
                LblNoTickets.Text = "There are no Tickets";
            }
            else
            {
                LblNoTickets.Text = "";
            }

            this.BindingContext = this;

            Title = CityFrom + "  -  " + CityTo ;

        }

        public async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            Item selectedItem = e.Item as Item;
            if (selectedItem != null)
                await DisplayAlert("Date : " + selectedItem.depart_date, "Price : " + selectedItem.value, "OK");
        }

    }
}
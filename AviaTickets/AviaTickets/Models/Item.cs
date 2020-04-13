using System;
using System.Collections.Generic;
using System.Text;

namespace AviaTickets.Models
{
    public class Item
    {
        public float value { get; set; }
        public string destination { get; set; }
        public string name { get; set; }
        public string depart_date { get; set; }
        public bool actual { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidityTakeHome.Models
{
    public class Person
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string company { get; set; }
        public string email { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public int zip { get; set; }
        public string city { get; set; }
        public string state_long { get; set; }
        public string state { get; set; }
        public string phone { get; set; }
    }
}

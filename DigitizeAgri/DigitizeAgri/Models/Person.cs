using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizeAgri.Models
{

    public class Person
    {
        [JsonIgnore]
        public int id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string income { get; set; }
    }

    public class RootObject
    {
        public List<Person> persons { get; set; }
    }


    
}

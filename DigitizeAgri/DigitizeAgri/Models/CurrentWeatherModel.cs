using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizeAgri.Models
{
   public class CurrentWeatherModel
    {
        //public string base { get; set; }
        public coord coord { get; set; }

        public clouds clouds { get; set; }
        public int cod { get; set; }

        public int dt { get; set; }

        public int id { get; set; }

        public cmain main { get; set; }

        public string name { get; set; }

        public List<weather> weather { get; set; }


        public wind wind { get; set; }

    }
    public class cmain
    {
        public float grnd_level { get; set; }
        public float humidity { get; set; }

        public float pressure { get; set; }

        public float sea_level { get; set; }

        public float temp { get; set; }



        public float temp_max { get; set; }

        public float temp_min { get; set; }
    }
    public class csys
    {
        public string country { get; set; }

        public float message { get; set; }

        public int sunrise { get; set; }

        public int sunset { get; set; }
    }
}

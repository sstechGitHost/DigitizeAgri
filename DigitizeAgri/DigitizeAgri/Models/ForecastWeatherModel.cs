using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizeAgri.Models
{
    public class ForecastWeatherModel
    {
        public city city { get; set; }
        public int cnt { get; set; }
        public int cod { get; set; }

        public float message { get; set; }

        public List<item> list { get; set; }
    }

    public class item
    {
        public int dt { get; set; }
        public string dt_txt { get; set; }
        public clouds clouds { get; set; }
        public main main { get; set; }
        public rain rain { get; set; }

        public sys sys { get; set; }
        public List<weather> weather { get; set; }

        public wind wind { get; set; }


    }
    public class wind
    {
        public float deg { get; set; }

        public float speed { get; set; }
    }
    public class weather
    {
        public string description { get; set; }

        public string icon { get; set; }

        public int id { get; set; }

        public string main { get; set; }
    }
    public class rain
    { }
    public class sys
    {
        public string pod { get; set; }
    }
    public class main
    {
        public float grnd_level { get; set; }
        public float humidity { get; set; }

        public float pressure { get; set; }

        public float sea_level { get; set; }

        public float temp { get; set; }

        public float temp_kf { get; set; }

        public float temp_max { get; set; }

        public float temp_min { get; set; }
    }
    public class clouds
    {
        public int all { get; set; }
    }
    public class city
    {
        public coord coord { get; set; }
        public string country { get; set; }
        public int id { get; set; }

        public string name { get; set; }

        public ssys sys { get; set; }



    }
    public class ssys
    {
        public int population { get; set; }
    }
    public class coord
    {
        public float lat { get; set; }
        public float lon { get; set; }
    }
}

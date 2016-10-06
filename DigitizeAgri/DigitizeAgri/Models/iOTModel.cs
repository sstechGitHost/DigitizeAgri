using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizeAgri.Models
{
    public class iOTModel
    {
        public Sensor_Datas[] sensor_datas { get; set; }
    }

    public class Sensor_Datas
    {
        public string sensor_date { get; set; }
        public string sensor_id { get; set; }
        public float temp { get; set; }
        public float humidity { get; set; }
        public float moisture { get; set; }
    }

}

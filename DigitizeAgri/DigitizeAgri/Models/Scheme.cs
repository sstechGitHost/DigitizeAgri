using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizeAgri.Models
{
    public class SchemeRootobject
    {
        public List<Scheme> schemes { get; set; }
    }

    public class Scheme
    {
        public int id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public string type { get; set; }
        public string description { get; set; }
    }
}

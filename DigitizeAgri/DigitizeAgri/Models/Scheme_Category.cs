using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizeAgri.Models
{
    public class SchemeCategorysRootobject
    {
        public List<Scheme_Category> scheme_categorys { get; set; }
    }

    public class Scheme_Category
    {
        public string category { get; set; }
        public string description { get; set; }
    }
}

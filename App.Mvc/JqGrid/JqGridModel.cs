using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Mvc.JqGrid
{
    public class JqGridModel
    {
        public int total { get; set; }
        public int page { get; set; }
        public int records { get; set; }
        public Array rows { get; set; }
    }
}

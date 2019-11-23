using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class PeriodReport
    {
        public double V1 { get; set; }
        public double V2 { get; set; }
    }

    public class GeoReport
    {
        public int GeoAreaID { get; set; }
        public string GeoAreaName { get; set; }
        public double V { get; set; }
    }
}
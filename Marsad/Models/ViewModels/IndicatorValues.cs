using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marsad.Models.ViewModels
{
    public class IndicatorValues
    {
        public int IndicatorID { get; set; }
        public string IndicatorName { get; set; }
        public int BundleID { get; set; }
        public string BundleName { get; set; }
        public Dictionary<int,double> Values { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class IndicatorLimit
    {
        [Key]
        public int ID { get; set; }
        public int IndicatorID { get; set; }
        public int Year { get; set; }
        public double? IntHigh { get; set; }
        public double? IntLow { get; set; }
        public double? LocHigh { get; set; }
        public double? LocLow { get; set; }

        public Indicator Indicator { get; set; }
    }
}
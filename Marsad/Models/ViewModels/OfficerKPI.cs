using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marsad.Models.ViewModels
{
    public class OfficerKPI
    {
        public ApplicationUser ApplicationUser { get; set; }
        public int  Count { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
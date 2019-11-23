using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marsad.Models.Controls
{
    public class GeoAreasModel
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public List<GeoArea> GovAreas { get; set; }
    }
}
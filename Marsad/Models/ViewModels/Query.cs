using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marsad.Models.ViewModels
{
    public class IndicatorQuery
    {
        public int ID { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public string BundleName { get; set; }
        public bool HasParent { get; set; }
        public int IndicatorsCount { get; set; }
    }

    public class DataSourceDetails
    {
        public int ID { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public string DataSourceTypeName { get; set; }
        public string AuthorName { get; set; }
        public DateTime PublishDate { get; set; }
        public string PublishNumber { get; set; }
        public string PeriodicString { get; set; }
        public string PublisherName{get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marsad.Models.ViewModels
{
    public class OfficerKPI
    {                
        public DateTime LastUpdated { get; set; }
        public int TotalEditSum { get; internal set; }
        public int LastEditCount { get; internal set; }
        public DateTime LastEdit { get; internal set; }
        public int TotalAddSum { get; internal set; }
        public int LastAddCount { get; internal set; }
        public DateTime LastAdd { get; internal set; }
        public string EntityName { get; internal set; }
        public string ApplicationUserId { get; internal set; }
        public string Log { get; internal set; }
        public int Year { get; internal set; }
    }
}
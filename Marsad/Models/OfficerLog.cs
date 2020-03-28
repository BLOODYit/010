using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class OfficerLog
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string EntityName { get; set; }
        public DateTime ActionDate { get; set; }
        public int ValuesCount { get; set; }
        public string Notes { get; set; }
        public string ApplicationUserId { get; internal set; }
        public int Year { get; internal set; }
    }
}
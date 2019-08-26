using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class MyClaim
    {
        public MyClaim()
        {
        }

        [Key]
        public int ID { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        //public string Type { get; set; }
        //public string Value { get; set; }
        public virtual List<UserGroup> UserGroups { get; set; }
    }
}
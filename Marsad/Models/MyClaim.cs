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
        public int Key { get; set; }
        public int Name { get; set; }
        public List<UserGroup> UserGroups { get; set; }
    }
}


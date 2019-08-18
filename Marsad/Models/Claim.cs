using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class Claim
    {
        public Claim()
        {
        }

        [Key]
        public int ID { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }

        public int UserGroupID { get; set; }
        public virtual UserGroup UserGroup { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class SystemLog
    {
        [Key]
        public int ID { get; set; }
    }

    public class PendingLog
    {
        [Key]
        public int ID { get; set; }
    }

    public class UpdateLog
    {
        [Key]
        public int ID { get; set; }
    }
}
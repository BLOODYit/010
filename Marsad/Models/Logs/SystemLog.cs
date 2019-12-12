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
        public string Log { get; set; }
        [MaxLength(1000)]
        public string UserName { get; set; }
        public DateTime ActionDate { get; set; }
        public string Details { get; set; }
    }


    public class UpdateLog
    {
        [Key]
        public int ID { get; set; }
        public string Log { get; set; }
        [MaxLength(1000)]
        public string UserName { get; set; }
        public DateTime ActionDate { get; set; }
        public string Details { get; set; }
    }

    public class PendingLog
    {
        [Key]
        public int ID { get; set; }
        public int GeoAreaID { get; set; }
        public int EquationYearID { get; set; }
        public string Log { get; set; }
        [MaxLength(1000)]
        public string UserName { get; set; }
        public DateTime ActionDate { get; set; }
        public string Details { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class CaseYearIndicator
    {

        [Key]
        public int ID { get; set; }

        [Required]
        public int CaseYearID { get; set; }
        public CaseYear CaseYear { get; set; }

        [Required]
        public int IndicatorID { get; set; }
        public Indicator Indicator { get; set; }

        [Required]
        public short IndicatorType { get; set; }

        public string Strategy { get; set; }

    }
}
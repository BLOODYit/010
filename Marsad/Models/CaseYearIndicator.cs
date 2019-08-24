using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class CaseYearIndicator
    {
        public CaseYearIndicator()
        {
            this.CaseYearIndicatorValues = new List<CaseYearIndicatorValue>();
        }

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

        public virtual List<CaseYearIndicatorValue> CaseYearIndicatorValues { get; set; }

    }
}
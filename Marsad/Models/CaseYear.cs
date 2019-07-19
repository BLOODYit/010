using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class CaseYear
    {
        public CaseYear()
        {
            this.CaseYearIndicators = new List<CaseYearIndicator>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        public int CaseID { get; set; }
        public Case Case { get; set; }

        [Required]
        public int Year { get; set; }

        public string Description { get; set; }

        public List<CaseYearIndicator> CaseYearIndicators { get; set; }

    }
}
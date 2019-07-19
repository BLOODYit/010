using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class Case
    {
        public Case()
        {
            this.CaseYears = new List<CaseYear>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(255)]        
        public string Name { get; set; }

        [Required]
        public int Year { get; set; }

        public string Description { get; set; }

        public List<CaseYear> CaseYears{ get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Marsad.Models
{
    public class Indicator
    {

        public Indicator()
        {
            this.CaseYearIndicators = new List<CaseYearIndicator>();
            this.IndicatorGroups = new List<IndicatorGroup>();
            this.Equations = new List<Equation>();
            this.Indicators = new List<Indicator>();
        }

        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Code { get; set; }

        [Required]
        [MaxLength(1024)]        
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int BundleID { get; set; }
        public Bundle Bundle { get; set; }

        [Required]
        public uint ElementCount { get; set; }

        public string MeasureUnit { get; set; }
        
        public int? IndicatorID { get; set; }
        public Indicator ParentIndicator { get; set; }

        [Required]
        public int IndicatorTypeID { get; set; }
        public IndicatorType IndicatorType  { get; set; }

        public string TargetMillCorrelation { get; set; }
        public string Importance { get; set; }
        public string ApplyLevel { get; set; }
        public string GenderCorrelation { get; set; }
        public string EvaluationInDevAxis { get; set; }
        public string Links { get; set; }
        public string RefreshMethod { get; set; }
        public string References { get; set; }
        public string CalculationMethod { get; set; }


        public List<CaseYearIndicator> CaseYearIndicators { get; set; }
        public List<IndicatorGroup> IndicatorGroups { get; set; }
        public List<Equation> Equations { get; set; }
        public List<Indicator> Indicators { get; set; }
    }
}
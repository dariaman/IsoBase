using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IsoBase.Models
{
    [Table("PlanMaster")]
    public class PlanMasterModel
    {
        [Key]
        public int PlanID { get; set; }
        [Required]
        public string ClientID { get; set; }
        public string PolicyNo { get; set; }

        public string PlanCode { get; set; }
        public string ShortName { get; set; }

        /*
        public string LongName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public int FrequencyCode { get; set; }
        public int FrequencyLimit { get; set; }
        public int MaxLimitCode { get; set; }
        public decimal MaxValueLimit { get; set; }
        */

        public Boolean IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}

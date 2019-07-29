using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("PlanLimit")]
    public class PlanLimitModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int ClientID { get; set; }
        public string PolicyNo { get; set; }
        public string PlanCode { get; set; }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        
        public int? FrequencyCodeID { get; set; }
        public int? LimitCodeID { get; set; }
        public decimal? MaxLimitValue { get; set; }

        public Boolean IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}

﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("BenefitUpload", Schema = "stg")]
    public class BenefitUploadModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int EnrollmentHdrID { get; set; }
        public string PlanId { get; set; }
        public string errPlanId { get; set; }
        public string CorpCode { get; set; }
        public string errCorpCode { get; set; }
        public string CoverageCode { get; set; }
        public string errCoverageCode { get; set; }
        public string BenefitCode { get; set; }
        public string errBenefitCode { get; set; }
        public string ActiveFlag { get; set; }
        public string errActiveFlag { get; set; }
        public string ConditionDescription { get; set; }
        public string errConditionDescription { get; set; }
        public string LOA_Description { get; set; }
        public string errLOA_Description { get; set; }
        public string ClientBenefitcode { get; set; }
        public string errClientBenefitcode { get; set; }
        public string MaxValue { get; set; }
        public string errMaxValue { get; set; }
        public string LimitCode { get; set; }
        public string errLimitCode { get; set; }
        public string FrequencyCode { get; set; }
        public string errFrequencyCode { get; set; }
        public string MultipleCondition { get; set; }
        public string errMultipleCondition { get; set; }

        public DateTime? UploadDate { get; set; }
        public string UserUpload { get; set; }
    }
}

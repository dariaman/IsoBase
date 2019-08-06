﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("CoverageUpload")]
    public class CoverageUploadModel
    {
        public CoverageUploadModel()
        {
            RecType = "2";
        }
        [Key]
        public int ID { get; set; }
        public string RecType { get; set; }
        public string PlanId { get; set; }
        public string CorpCode { get; set; }
        public string CoverageCode { get; set; }
        public string ActiveFlag { get; set; }
        public string ClientCoverageCode { get; set; }
        public string LimitCode { get; set; }
        public string FrequencyCode { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string FamilyValue { get; set; }

        public DateTime? UploadDate { get; set; }
        public string UserUpload { get; set; }
        public string ErrorMessage { get; set; }
    }
}

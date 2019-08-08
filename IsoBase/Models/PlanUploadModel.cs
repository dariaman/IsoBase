﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("PlanUpload",Schema ="stg")]
    public class PlanUploadModel
    {
        public PlanUploadModel()
        {
            RecType = "1";
        }
        public PlanUploadModel(int _clientID)
        {
            RecType = "1";
            ClientID = _clientID;
        }
        [Key]
        public int ID { get; set; }
        [Required]
        public int ClientID { get; set; }
        public string RecType { get; set; }
        public string PayorCode { get; set; }
        public string PlanId { get; set; }
        public string CorpCode { get; set; }
        public string EffectiveDate { get; set; }
        public string TerminationDate { get; set; }
        public string ActiveFlag { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Remarks { get; set; }
        public string PolicyNo { get; set; }
        public string FrequencyCode { get; set; }
        public string LimitCode { get; set; }
        public string MaxValue { get; set; }
        public string FamilyMaxValue { get; set; }
        public string PrintText { get; set; }
        public DateTime? UploadDate { get; set; }
        public string UserUpload { get; set; }
        public string ErrorMessage { get; set; }
    }
}
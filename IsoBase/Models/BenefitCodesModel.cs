﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("BenefitCodes")]
    public class BenefitCodesModel
    {
        [Key]
        public int Code { get; set; }
        [Required]
        public string Loa{ get; set; }
        public string Description { get; set; }

        public Boolean IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
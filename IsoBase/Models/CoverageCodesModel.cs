﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("CoverageCodes")]
    public class CoverageCodesModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string ShortName { get; set; }
        public string Description { get; set; }

        public string IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}

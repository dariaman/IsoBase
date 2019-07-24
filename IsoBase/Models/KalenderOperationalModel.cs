using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoBase.Models
{
    [Table("KalenderOperational")]
    public class KalenderOperationalModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Tgl { get; set; }
        public int DayNumMonth { get; set; }
        public int DayNumYear { get; set; }
        public int DayNameEn { get; set; }
        public string DayNameInd { get; set; }
        public int MonthYear { get; set; }
        public string MonthNameEn { get; set; }
        public string MonthNameInd { get; set; }
        public int YearNumber { get; set; }

        public Boolean IsHoliday { get; set; }

        public string UserCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}

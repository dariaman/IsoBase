using System;
using System.ComponentModel.DataAnnotations;

namespace IsoBase.ViewModels
{
    public class KalenderVM
    {
        [Key]
        public int ID { get; set; }
        public string Tgl { get; set; }
        public int DayNumMonth { get; set; }
        public int DayNumYear { get; set; }
        public string DayNameEn { get; set; }
        public string DayNameInd { get; set; }
        public int MonthYear { get; set; }
        public string MonthNameEn { get; set; }
        public string MonthNameInd { get; set; }
        public int YearNumber { get; set; }
        public string IsHoliday { get; set; }
        public string UserUpdate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}

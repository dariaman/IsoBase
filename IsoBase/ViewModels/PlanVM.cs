using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsoBase.ViewModels
{
    public class PlanVM
    {
        public string ID { get; set; }
        public string ClientID { get; set; }
        public string PolicyNo { get; set; }
        public string ClientPlanID { get; set; }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Frequency { get; set; }
        public string Limit { get; set; }
        public string MaxLimitValue { get; set; }

        public string IsActive { get; set; }
        public string UserCreate { get; set; }
        public string DateCreate { get; set; }
    }
}

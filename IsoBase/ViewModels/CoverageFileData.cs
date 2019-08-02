using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsoBase.ViewModels
{
    public class CoverageFileData
    {
        public string PlanID { get; set; }
        public string CorpCode { get; set; }
        public string CoverageCode { get; set; }
        public string ActiveFlag { get; set; }
        public string ClientCoverage { get; set; }
        public string LimitCode { get; set; }
        public string FrequencyCode { get; set; }
        public string MaxValue { get; set; }
        public string FamilyLimit { get; set; }
        public string ErrorMessage { get; set; }

    }
}

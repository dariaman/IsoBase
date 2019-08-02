using System.Collections.Generic;

namespace IsoBase.ViewModels
{
    public class EnrollPlanFileExcelDataVM
    {
        public List<PlanFileData> PlanData { get; set; }
        public List<CoverageFileData> CoverageData { get; set; }
        public List<BenefitFileData> BenefitData { get; set; }

    }
}

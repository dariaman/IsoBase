using IsoBase.Models;
using System.Collections.Generic;

namespace IsoBase.ViewModels
{
    public class EnrollPlanFileExcelDataVM
    {
        public List<PlanUploadModel> PlanData { get; set; }
        public List<CoverageUploadModel> CoverageUploadModel { get; set; }
        public List<BenefitFileData> BenefitData { get; set; }

    }
}

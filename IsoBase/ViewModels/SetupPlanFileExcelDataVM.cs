using IsoBase.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IsoBase.ViewModels
{
    public class EnrollPlanFileExcelDataVM
    {
        public EnrollPlanFileExcelDataVM(int _clnt)
        {
            ClientID = _clnt;
        }

        [Key]
        public int ClientID { get; set; }
        public List<PlanUploadModel> PlanUploadModel { get; set; }
        public List<CoverageUploadModel> CoverageUploadModel { get; set; }
        public List<BenefitUploadModel> BenefitUploadModel { get; set; }

    }
}

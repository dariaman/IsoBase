
namespace IsoBase.ViewModels
{
    public class BenefitUploadVM
    {
        public int ID { get; set; }
        public string PlanId { get; set; }
        public string CorpCode { get; set; }
        public string CoverageCode { get; set; }
        public string BenefitCode { get; set; }
        public string BenefitShortName { get; set; }
        public string BenefitDescription { get; set; }
        public string ActiveFlag { get; set; }
        public string ConditionDescription { get; set; }
        public string LOA_Description { get; set; }
        public string ClientBenefitcode { get; set; }
        public string LimitCode { get; set; }
        public string FrequencyCode { get; set; }
        public string MaxValue { get; set; }
        public string MultipleCondition { get; set; }
        public string UploadDate { get; set; }
        public string UserUpload { get; set; }
        public string ErrorMessage { get; set; }
    }
}

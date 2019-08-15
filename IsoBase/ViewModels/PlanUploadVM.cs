
namespace IsoBase.ViewModels
{
    public class PlanUploadVM
    {
        public int ID { get; set; }
        public string PayorCode { get; set; }
        public string PlanId { get; set; }
        public string CorpCode { get; set; }
        public string EffectiveDate { get; set; }
        public string TerminationDate { get; set; }
        public string ActiveFlag { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Remarks { get; set; }
        public string PolicyNo { get; set; }
        public string FrequencyCode { get; set; }
        public string LimitCode { get; set; }
        public string MaxValue { get; set; }
        public string FamilyMaxValue { get; set; }
        public string PrintText { get; set; }
        public string UploadDate { get; set; }
        public string UserUpload { get; set; }
        public string ErrorMessage { get; set; }
    }
}

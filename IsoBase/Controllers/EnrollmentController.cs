using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataTables.AspNetCore.Mvc.Binder;
using IsoBase.Data;
using IsoBase.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;
using IsoBase.Models;
using System.IO;
using IsoBase.Extension;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace IsoBase.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlashMessage flashMessage;
        private static string paternAngka { get; } = @"[^0-9]";

        public EnrollmentController(ApplicationDbContext context, IFlashMessage flash)
        {
            _context = context;
            flashMessage = flash;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Plan(string ClientID)
        {
            string _urlBack = (Request.Headers["Referer"].ToString() == "" ? "Index" : Request.Headers["Referer"].ToString());
            if (ClientID.Trim() != Regex.Replace(ClientID.Trim(), paternAngka, ""))
            {
                flashMessage.Danger("Invalid ClientID");
                return Redirect(_urlBack);
            }

            var _client = ClientFindByID(Convert.ToInt32(ClientID));

            if (_client == null)
            {
                flashMessage.Danger("Client Not Found");
                return Redirect(_urlBack);
            }
            // ==============End Validation========================
            LastEnrollVM _lastEnroll = new LastEnrollVM();
            _lastEnroll._clientModel = _client;
            _lastEnroll.EnrollmentHdrModel = _context.EnrollmentHdrModel.Where(x=>x.ClientID==_client.ID).OrderByDescending(x=>x.ID).FirstOrDefault();

            return View(_lastEnroll);
        }

        public async Task<IActionResult> UploadFilePlan([Bind("ClientID,Fileupload")] UploadFilePlanVM ClientfilePlan)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string _urlBack = (Request.Headers["Referer"].ToString() == "" ? "Index" : Request.Headers["Referer"].ToString());

            try
            {
                if (ClientfilePlan.Fileupload == null || ClientfilePlan.Fileupload.Length == 0)
                {
                    flashMessage.Danger("File Not Selected");
                    throw new Exception();
                }

                FileUploadExt uplExt = new FileUploadExt();
                FileStream Filestrm;
                ExcelExt excelRead;
                EnrollmentHdrModel enrollH = new EnrollmentHdrModel();

                // upload file ke server
                // Param 1 untuk file Plan excel 
                try
                {
                    Filestrm = await uplExt.BackupFile(1, ClientfilePlan.Fileupload);

                }
                catch (Exception ex) { throw new Exception(ex.Message); }

                //// Copy File To Server
                try { excelRead = new ExcelExt(Filestrm, ClientfilePlan.ClientID); }
                catch (Exception ex) { throw new Exception("Error Send File : " + ex.Message); }

                //// Read Cell Value to VM
                EnrollPlanFileExcelDataVM EnrolPlan = new EnrollPlanFileExcelDataVM();
                EnrolPlan.ClientID = ClientfilePlan.ClientID;
                try { excelRead.ReadExcelEnrollPlan(ref EnrolPlan); }
                catch (Exception ex) { throw new Exception("Error Read Excel : " + ex.Message); }

                // Proses pembersihan data dan generate error Upload 
                ValidasiFileUploadPlan(ref EnrolPlan);

                // Save To DB
                try
                {
                    enrollH.ClientID = ClientfilePlan.ClientID;
                    enrollH.FileUploadName = ClientfilePlan.Fileupload.FileName;

                    await SaveProductToDB(EnrolPlan, enrollH);
                }
                catch (Exception ex) { throw new Exception("Error Bulk Insert : " + ex.Message); }

                flashMessage.Confirmation("Upload Success");
            }
            catch (Exception ex)
            {
                flashMessage.Danger(ex.Message);
            }
            return Redirect(_urlBack);
        }

        public IActionResult Member()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientActiveAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var pgData = new PageData(dataRequest, _context)
            {
                select = @"SELECT cl.ID ClientID,cl.ClientCode,cl.Name ClientName,ct.Name ClientTypeName,cl.Building,cl.Address,cl.ClientTypeID,cl.ID ",
                Tabel = @" FROM dbo.Client cl
                            INNER JOIN dbo.ClientType ct ON ct.ID = cl.ClientTypeID
                            WHERE cl.IsActive=1 ",
            };

            //defenisikan Where condition
            foreach (var req in dataRequest.Columns)
            {
                if (string.IsNullOrEmpty(req.SearchValue)) continue;
                else if (req.Data == "clientID") pgData.AddWhereRegex(pgData.paternAngkaLike, req.SearchValue, "cl.ID");
                else if (req.Data == "clientCode") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "cl.ClientCode");
                else if (req.Data == "clientName") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "cl.Name");
                else if (req.Data == "clientName") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "Name");
                else if (req.Data == "clientName") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "Name");
            }
            List<EnrollmentVM> ls = new List<EnrollmentVM>();

            try
            {
                pgData.CountData(); // hitung jlh total data dan total dataFilter
                ls = await _context.Set<EnrollmentVM>().FromSql(pgData.GenerateQueryString()).ToListAsync();
            }
            catch (Exception ex) { flashMessage.Info("Error Client Enroll : " + ex.Message); }

            return Json(ls.ToDataTablesResponse(dataRequest, pgData.recordsTotal, pgData.recordsFilterd));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlanUploadListByClient([DataTablesRequest] DataTablesRequest dataRequest, string EnrollmentHdrID)
        {
            var _clntID = Regex.Replace(EnrollmentHdrID.Trim(), paternAngka, "");
            var query = string.Format(@"SELECT pu.ID,pu.PayorCode,pu.PlanId,pu.CorpCode,pu.EffectiveDate,pu.TerminationDate,pu.ActiveFlag,
                            pu.ShortName,pu.LongName,pu.Remarks,pu.PolicyNo,fc.Description FrequencyCode,lc.Description LimitCode,pu.MaxValue,
                            pu.FamilyMaxValue,pu.PrintText,CONCAT(CONVERT(VARCHAR(30),pu.UploadDate,106),' ',CONVERT(VARCHAR(30),pu.UploadDate,108)) UploadDate,
                            pu.UserUpload,pu.ErrorMessage 
                        FROM stg.PlanUpload pu
                            LEFT JOIN FrequencyCodes fc ON fc.ID = pu.FrequencyCode
                            LEFT JOIN LimitCodes lc ON lc.ID = pu.LimitCode
                            WHERE pu.EnrollmentHdrID={0}  ", _clntID);

            var ls = new List<PlanUploadVM>();
            try
            {
                ls = await _context.Set<PlanUploadVM>().FromSql(query).ToListAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CoverageUploadListByClient([DataTablesRequest] DataTablesRequest dataRequest, string EnrollmentHdrID)
        {
            var _clntID = Regex.Replace(EnrollmentHdrID.Trim(), paternAngka, "");
            var query = string.Format(@"SELECT cu.ID,cu.PlanId,cu.CorpCode,cu.CoverageCode,cu.ActiveFlag,
                                        cu.ClientCoverageCode,lc.Description LimitCode,fc.Description FrequencyCode,cu.MinValue,cu.MaxValue,
                                        cu.FamilyValue,CONCAT(CONVERT(VARCHAR(30),cu.UploadDate,106),' ',CONVERT(VARCHAR(30),cu.UploadDate,108)) UploadDate,
                                        cu.UserUpload,cu.ErrorMessage 
                                    FROM stg.CoverageUpload cu
                                    LEFT JOIN FrequencyCodes fc ON fc.ID = cu.FrequencyCode
                                    LEFT JOIN LimitCodes lc ON lc.ID = Cu.LimitCode
                                    WHERE cu.EnrollmentHdrID={0} ", _clntID);

            var ls = new List<CoverageUploadVM>();
            try
            {
                ls = await _context.Set<CoverageUploadVM>().FromSql(query).ToListAsync();
            }
            catch (Exception ex)
            {
                flashMessage.Danger("Error Paging Coverage : " + ex.Message);
            }


            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BenefitUploadListByClient([DataTablesRequest] DataTablesRequest dataRequest, string EnrollmentHdrID)
        {
            var _clntID = Regex.Replace(EnrollmentHdrID.Trim(), paternAngka, "");
            var query = string.Format(@"SELECT bu.ID,bu.PlanId,bu.CorpCode,cc.ShortName CoverageCode,bu.BenefitCode,bc.Code BenefitShortName,
                                    bc.Description BenefitDescription,bu.ActiveFlag,bu.ConditionDescription,bu.LOA_Description,bu.ClientBenefitcode,
                                    lc.Description LimitCode,fc.Description FrequencyCode,bu.MaxValue,bu.MultipleCondition,
                                    CONCAT(CONVERT(VARCHAR(30),bu.UploadDate,106),' ',CONVERT(VARCHAR(30),bu.UploadDate,108)) UploadDate,bu.UserUpload,
                                    bu.ErrorMessage
                                FROM stg.BenefitUpload bu
                                LEFT JOIN dbo.CoverageCodes cc ON cc.ID = bu.CoverageCode
                                LEFT JOIN dbo.BenefitCodes bc ON bc.ID = bu.BenefitCode
                                LEFT JOIN FrequencyCodes fc ON fc.ID = bu.FrequencyCode
                                LEFT JOIN LimitCodes lc ON lc.ID = bu.LimitCode
                                    WHERE bu.EnrollmentHdrID={0} ", _clntID);
            var ls = new List<BenefitUploadVM>();
            try
            {
                ls = await _context.Set<BenefitUploadVM>().FromSql(query).ToListAsync();
            }
            catch (Exception ex)
            {
                flashMessage.Danger("Error Paging benefit : " + ex.Message);
                throw new Exception();
            }

            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
        }

        private ClientModel ClientFindByID(int id)
        {
            return _context.ClientModel.Where(x => x.ID == id).FirstOrDefault();
        }

        private void ValidasiFileUploadPlan(ref EnrollPlanFileExcelDataVM pln)
        {
            var CleanPlan = new EnrollPlanFileExcelDataVM();
            CleanPlan.ClientID = pln.ClientID;
            CleanPlan.PlanUploadModel = new List<PlanUploadModel>();
            CleanPlan.CoverageUploadModel = new List<CoverageUploadModel>();
            CleanPlan.BenefitUploadModel = new List<BenefitUploadModel>();

            foreach (var item in pln.PlanUploadModel)
            {
                var newItem = new PlanUploadModel();
                newItem.PayorCode = (item.PayorCode ?? "").Trim();
                newItem.PlanId = (item.PlanId ?? "").Trim();
                newItem.CorpCode = (item.CorpCode ?? "").Trim();
                newItem.EffectiveDate = (item.EffectiveDate ?? "").Trim();
                newItem.TerminationDate = (item.TerminationDate ?? "").Trim();
                newItem.ActiveFlag = (item.ActiveFlag ?? "").Trim();
                newItem.ShortName = (item.ShortName ?? "").Trim();
                newItem.LongName = (item.LongName ?? "").Trim();
                newItem.Remarks = (item.Remarks ?? "").Trim();
                newItem.PolicyNo = (item.PolicyNo ?? "").Trim();
                newItem.FrequencyCode = (item.FrequencyCode ?? "").Trim();
                newItem.LimitCode = (item.LimitCode ?? "").Trim();
                newItem.MaxValue = (item.MaxValue ?? "").Trim();
                newItem.FamilyMaxValue = (item.FamilyMaxValue ?? "").Trim();
                newItem.PrintText = (item.PrintText ?? "").Trim();
                newItem.UploadDate = item.UploadDate;
                newItem.UserUpload = item.UserUpload;
                newItem.ErrorMessage = item.ErrorMessage;
                CleanPlan.PlanUploadModel.Add(newItem);
            }
            pln.PlanUploadModel.Clear();

            foreach (var item in pln.CoverageUploadModel)
            {
                var newItem = new CoverageUploadModel();
                newItem.PlanId = (item.PlanId ?? "").Trim();
                newItem.CorpCode = (item.CorpCode ?? "").Trim();
                newItem.CoverageCode = (item.CoverageCode ?? "").Trim();
                newItem.ActiveFlag = (item.ActiveFlag ?? "").Trim();
                newItem.ClientCoverageCode = (item.ClientCoverageCode ?? "").Trim();
                newItem.LimitCode = (item.LimitCode ?? "").Trim();
                newItem.FrequencyCode = (item.FrequencyCode ?? "").Trim();
                newItem.MinValue = (item.MinValue ?? "").Trim();
                newItem.MaxValue = (item.MaxValue ?? "").Trim();
                newItem.FamilyValue = (item.FamilyValue ?? "").Trim();
                newItem.UploadDate = item.UploadDate;
                newItem.UserUpload = item.UserUpload;
                newItem.ErrorMessage = item.ErrorMessage;
                CleanPlan.CoverageUploadModel.Add(newItem);
            }
            pln.CoverageUploadModel.Clear();

            foreach (var item in pln.BenefitUploadModel)
            {
                var newItem = new BenefitUploadModel();

                newItem.PlanId = (item.PlanId ?? "").Trim();
                newItem.CorpCode = (item.CorpCode ?? "").Trim();
                newItem.CoverageCode = (item.CoverageCode ?? "").Trim();
                newItem.BenefitCode = (item.BenefitCode ?? "").Trim();
                newItem.ActiveFlag = (item.ActiveFlag ?? "").Trim();
                newItem.ConditionDescription = (item.ConditionDescription ?? "").Trim();
                newItem.LOA_Description = (item.LOA_Description ?? "").Trim();
                newItem.ClientBenefitcode = (item.ClientBenefitcode ?? "").Trim();
                newItem.MaxValue = (item.MaxValue ?? "").Trim();
                newItem.LimitCode = (item.LimitCode ?? "").Trim();
                newItem.FrequencyCode = (item.FrequencyCode ?? "").Trim();
                newItem.MultipleCondition = (item.MultipleCondition ?? "").Trim();
                newItem.UploadDate = item.UploadDate;
                newItem.UserUpload = item.UserUpload;
                newItem.ErrorMessage = item.ErrorMessage;
                CleanPlan.BenefitUploadModel.Add(newItem);
            }
            pln.BenefitUploadModel.Clear();

            pln = CleanPlan;
        }

        private async Task SaveProductToDB(EnrollPlanFileExcelDataVM clPrd, EnrollmentHdrModel _EnrolH)
        {
            ///// Save Data To Database

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Save Enroll Header untuk mendapatkan ID Header
                    _context.Add(_EnrolH);
                    await _context.SaveChangesAsync();

                    clPrd.PlanUploadModel.ForEach(x => x.EnrollmentHdrID = _EnrolH.ID);
                    clPrd.CoverageUploadModel.ForEach(x => x.EnrollmentHdrID = _EnrolH.ID);
                    clPrd.BenefitUploadModel.ForEach(x => x.EnrollmentHdrID = _EnrolH.ID);

                    _context.BulkInsert(clPrd.PlanUploadModel);
                    _context.BulkInsert(clPrd.CoverageUploadModel);
                    _context.BulkInsert(clPrd.BenefitUploadModel);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    flashMessage.Danger(ex.Message);
                    throw new Exception();
                }
            }
        }
    }
}

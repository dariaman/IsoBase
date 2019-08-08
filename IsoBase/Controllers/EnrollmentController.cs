using System;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataTables.AspNetCore.Mvc.Binder;
using IsoBase.Data;
using IsoBase.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;
using IsoBase.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using IsoBase.Extension;
using OfficeOpenXml;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
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


            return View(_client);
        }

        public async Task<IActionResult> UploadFilePlan([Bind("ID,Fileupload")] UploadFilePlanVM ClientfilePlan)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string _urlBack = (Request.Headers["Referer"].ToString() == "" ? "Index" : Request.Headers["Referer"].ToString());

            try
            {
                if (ClientfilePlan.Fileupload == null || ClientfilePlan.Fileupload.Length == 0)
                    throw new Exception("File Not Selected");

                FileUploadExt uplExt = new FileUploadExt();
                FileStream Filestrm;
                ExcelExt excelRead;

                // upload file ke server
                // Param 1 untuk file Plan excel 
                try { Filestrm = await uplExt.BackupFile(1, ClientfilePlan.Fileupload); }
                catch (Exception ex) { throw new Exception(ex.Message); }

                //// Copy File To Server
                try { excelRead = new ExcelExt(Filestrm, ClientfilePlan.ID); }
                catch (Exception ex) { throw new Exception("Error Send File : " + ex.Message); }

                //// Read Cell Value to VM
                var EnrolPlan = new EnrollPlanFileExcelDataVM(ClientfilePlan.ID);
                try { excelRead.ReadExcelEnrollPlan(ref EnrolPlan); }
                catch (Exception ex) { throw new Exception("Error Read Excel : " + ex.Message); }

                // Proses pembersihan data dan generate error Upload 
                ValidasiFileUploadPlan(ref EnrolPlan);

                // Save To DB
                try { DeleteOld_SaveNewPlanToDB(EnrolPlan); }
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
        public IActionResult ClientActiveAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var pgData = new PageData(dataRequest, _context)
            {
                select = @"SELECT ID,ClientCode,Name ",
                Tabel = @" FROM dbo.Client WHERE IsActive=1 ",
            };

            //defenisikan Where condition
            foreach (var req in dataRequest.Columns)
            {
                if (string.IsNullOrEmpty(req.SearchValue)) continue;
                //else if (req.Data == "clientID") pgData.AddWhereRegex(pgData.paternAngkaLike, req.SearchValue, "ID");
                else if (req.Data == "clientCode") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "ClientCode");
                else if (req.Data == "clientName") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "Name");
            }
            pgData.CountData(); // hitung jlh total data dan total dataFilter

            DataTable _dt = new DataTable();
            try
            {
                _dt = pgData.ListData();
            }
            catch (Exception ex) { flashMessage.Info(ex.Message); }

            List<EnrollmentVM> ls = new List<EnrollmentVM>();
            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    ls.Add(new EnrollmentVM
                    {
                        ClientID = row["ID"].ToString(),
                        ClientCode = row["ClientCode"].ToString(),
                        ClientName = row["Name"].ToString(),
                    });
                };
            }
            catch (Exception ex)
            {
                flashMessage.Info(ex.Message);
                //throw new Exception(ex.Message);
            }
            return Json(ls.ToDataTablesResponse(dataRequest, pgData.recordsTotal, pgData.recordsFilterd));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlanUploadListByClient([DataTablesRequest] DataTablesRequest dataRequest, string clientID)
        {
            var _clntID = Regex.Replace(clientID.Trim(), paternAngka, "");
            var query = string.Format(@"SELECT pu.ID,pu.ClientID,pu.RecType,pu.PayorCode,pu.PlanId,pu.CorpCode,pu.EffectiveDate,pu.TerminationDate,pu.ActiveFlag,
                            pu.ShortName,pu.LongName,pu.Remarks,pu.PolicyNo,fc.Description FrequencyCode,lc.Description LimitCode,pu.MaxValue,
                            pu.FamilyMaxValue,pu.PrintText,pu.UploadDate,pu.UserUpload,pu.ErrorMessage 
                        FROM stg.PlanUpload pu
                            LEFT JOIN FrequencyCodes fc ON fc.ID = pu.FrequencyCode
                            LEFT JOIN LimitCodes lc ON lc.ID = pu.LimitCode
                            WHERE pu.ClientID={0}  ", _clntID);

            var ls = new List<PlanUploadVM>();
            try
            {
                ls = await _context.Set<PlanUploadVM>().FromSql(query).ToListAsync();
            }
            catch (Exception ex)
            {
                flashMessage.Danger("Error Paging Plan : " + ex.Message);
            }

            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CoverageUploadListByClient([DataTablesRequest] DataTablesRequest dataRequest, string clientID)
        {
            var _clntID = Regex.Replace(clientID.Trim(), paternAngka, "");
            var query = string.Format(@"SELECT cu.ID,cu.ClientID,cu.RecType,cu.PlanId,cu.CorpCode,cu.CoverageCode,cu.ActiveFlag,
                                        cu.ClientCoverageCode,lc.Description LimitCode,fc.Description FrequencyCode,cu.MinValue,cu.MaxValue,
                                        cu.FamilyValue,cu.UploadDate,cu.UserUpload,cu.ErrorMessage 
                                    FROM stg.CoverageUpload cu
                                    LEFT JOIN FrequencyCodes fc ON fc.ID = cu.FrequencyCode
                                    LEFT JOIN LimitCodes lc ON lc.ID = Cu.LimitCode
                                    WHERE cu.ClientID={0} ", _clntID);

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
        public async Task<IActionResult> BenefitUploadListByClient([DataTablesRequest] DataTablesRequest dataRequest, string clientID)
        {
            var _clntID = Regex.Replace(clientID.Trim(), paternAngka, "");
            var query = string.Format(@"SELECT bu.ID,bu.ClientID,bu.RecType,bu.PlanId,bu.CorpCode,cc.ShortName CoverageCode,bu.BenefitCode,bc.Code BenefitShortName,
                                    bc.Description BenefitDescription,bu.ActiveFlag,bu.ConditionDescription,bu.LOA_Description,bu.ClientBenefitcode,
                                    lc.Description LimitCode,fc.Description FrequencyCode,bu.MaxValue,bu.MultipleCondition,bu.UploadDate,bu.UserUpload,bu.ErrorMessage
                                FROM stg.BenefitUpload bu
                                LEFT JOIN dbo.CoverageCodes cc ON cc.ID = bu.CoverageCode
                                LEFT JOIN dbo.BenefitCodes bc ON bc.ID = bu.BenefitCode
                                LEFT JOIN FrequencyCodes fc ON fc.ID = bu.FrequencyCode
                                LEFT JOIN LimitCodes lc ON lc.ID = bu.LimitCode
                                    WHERE bu.ClientID={0} ", _clntID);
            var ls = new List<BenefitUploadVM>();
            try
            {
                ls = await _context.Set<BenefitUploadVM>().FromSql(query).ToListAsync();
            }
            catch (Exception ex)
            {
                flashMessage.Danger("Error Paging benefit : " + ex.Message);
            }

            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
        }


        private ClientModel ClientFindByID(int id)
        {
            return _context.ClientModel.Where(x => x.ID == id).FirstOrDefault();
        }

        private void ValidasiFileUploadPlan(ref EnrollPlanFileExcelDataVM pln)
        {
            var CleanPlan = new EnrollPlanFileExcelDataVM(pln.ClientID);
            CleanPlan.PlanUploadModel = new List<PlanUploadModel>();
            CleanPlan.CoverageUploadModel = new List<CoverageUploadModel>();
            CleanPlan.BenefitUploadModel = new List<BenefitUploadModel>();

            foreach (var item in pln.PlanUploadModel)
            {
                var newItem = new PlanUploadModel(pln.ClientID);
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
                var newItem = new CoverageUploadModel(pln.ClientID);
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
                var newItem = new BenefitUploadModel(pln.ClientID);

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

        private void DeleteOld_SaveNewPlanToDB(EnrollPlanFileExcelDataVM clPrd)
        {
            ///// Save Data To Database
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.PlanUploadModel.Where(a => a.ClientID == clPrd.ClientID).BatchDelete();
                    _context.CoverageUploadModel.Where(a => a.ClientID == clPrd.ClientID).BatchDelete();
                    _context.BenefitUploadModel.Where(a => a.ClientID == clPrd.ClientID).BatchDelete();

                    _context.BulkInsert(clPrd.PlanUploadModel);
                    _context.BulkInsert(clPrd.CoverageUploadModel);
                    _context.BulkInsert(clPrd.BenefitUploadModel);
                    transaction.Commit();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

    }
}
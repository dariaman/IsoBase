using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Data.SqlClient;
using System.Globalization;
using System.Data;

namespace IsoBase.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlashMessage flashMessage;

        public EnrollmentController(ApplicationDbContext context, IFlashMessage flash)
        {
            _context = context;
            flashMessage = flash;
        }

        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> Plan(string ClientID)
        {
            string _urlBack = (Request.Headers["Referer"].ToString() == "" ? "Index" : Request.Headers["Referer"].ToString());

            if (!int.TryParse(ClientID, out int _clientid))
            {
                flashMessage.Danger("Invalid ClientID");
                return Redirect(_urlBack);
            }

            var _client = ClientFindByID(_clientid);

            // ==============End Validation========================
            LastEnrollVM _lastEnroll = new LastEnrollVM();
            EnrollmentHdrVM __lastEnrollHdr = new EnrollmentHdrVM();
            string query = "SELECT TOP 1 * FROM dbo.EnrollmentHdr WHERE ClientID=@_ClientID ORDER BY ID DESC";
            var _params = new SqlParameter { ParameterName = "@_ClientID", SqlDbType = SqlDbType.Int, Value = _clientid };
            //var lastEnroll = new EnrollmentHdrModel();
            try
            {
                _lastEnroll._clientModel = _client;
                //ls = await _context.Set<PlanUploadVM>().FromSql(query, _params).ToListAsync();
                //var lastEnroll = _context.EnrollmentHdrModel.Where(x => x.ClientID == _client.ID).OrderByDescending(x => x.ID).FirstOrDefault();
                var lastEnroll = await _context.EnrollmentHdrModel.FromSql(query, _params).FirstOrDefaultAsync();
                _lastEnroll._enrollmentHdrVM.ID = lastEnroll?.ID ?? 0;
                _lastEnroll._enrollmentHdrVM.FileUploadName = lastEnroll?.FileUploadName;
                _lastEnroll._enrollmentHdrVM.ClientID = lastEnroll?.ClientID ?? 0;
            }
            catch (Exception ex)
            {
                flashMessage.Danger(ex.Message);
                throw new Exception(ex.Message);
            }

            return View(_lastEnroll);
        }

        public async Task<IActionResult> UploadFilePlan([Bind("ClientID,Fileupload")] UploadFilePlanVM ClientfilePlan)
        {
            string _urlBack = (Request.Headers["Referer"].ToString() == "" ? "Index" : Request.Headers["Referer"].ToString());
            try
            {
                if (ClientfilePlan.Fileupload == null || ClientfilePlan.Fileupload.Length == 0)
                {
                    flashMessage.Danger("File Not Selected");
                    throw new Exception();
                }
                var _client = ClientFindByID(Convert.ToInt32(ClientfilePlan.ClientID));
                if (_client == null)
                {
                    flashMessage.Danger("Client Not Found");
                    throw new Exception();
                }

                FileUploadExt uplExt = new FileUploadExt();
                //FileStream Filestrm;
                ExcelExt excelRead;
                EnrollmentHdrModel enrollH = new EnrollmentHdrModel();

                // upload file ke server
                // Param 1 untuk file Plan excel 
                //// Copy File To Server
                string FilePath;
                EnrollPlanFileExcelDataVM EnrolPlan = new EnrollPlanFileExcelDataVM();
                try
                {
                    EnrolPlan.StringPathFileUpload = await uplExt.BackupFile(1, ClientfilePlan.Fileupload);
                    FilePath = EnrolPlan.StringPathFileUpload;
                }
                catch (Exception ex) { throw new Exception(ex.Message); }

                try { excelRead = new ExcelExt(EnrolPlan.StringPathFileUpload, ClientfilePlan.ClientID); }
                catch (Exception ex) { throw new Exception(ex.Message); }

                //// Read Cell Value to VM
                EnrolPlan.ClientID = ClientfilePlan.ClientID;
                EnrolPlan.ClientCode = _client.ClientCode;
                try { excelRead.ReadExcelEnrollPlan(ref EnrolPlan); }
                catch (Exception ex) { throw new Exception("Error Read Excel : " + ex.Message); }

                // Proses pembersihan data dan generate error Upload 
                ValidasiFileUploadPlan(ref EnrolPlan);

                // Save To DB
                try
                {
                    enrollH.ClientID = ClientfilePlan.ClientID;
                    enrollH.FileUploadName = FilePath;
                    await SaveProductToDB(EnrolPlan, enrollH);
                }
                catch (Exception ex) { throw new Exception("Error Bulk Insert : " + ex.Message); }

                flashMessage.Confirmation("Upload Success");
            }
            catch (Exception ex) { flashMessage.Danger(ex.Message); }

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
            int.TryParse(EnrollmentHdrID.Trim(), out int _clientID);
            var query = string.Format(@"SELECT pu.ID,pu.PayorCode,pu.PlanId,pu.CorpCode,pu.EffectiveDate,pu.TerminationDate,pu.ActiveFlag,
                            pu.ShortName,pu.LongName,pu.Remarks,pu.PolicyNo,fc.Description FrequencyCode,lc.Description LimitCode,pu.MaxValue,
                            pu.FamilyMaxValue,pu.PrintText,CONCAT(CONVERT(VARCHAR(30),pu.UploadDate,106),' ',CONVERT(VARCHAR(30),pu.UploadDate,108)) UploadDate,
                            pu.UserUpload,CONCAT(pu.errPayorCode+'<br>',pu.errPlanId+'<br>',pu.errCorpCode+'<br>',pu.errEffectiveDate+'<br>',pu.errTerminationDate+'<br>',pu.errActiveFlag+'<br>',
                            pu.errFrequencyCode+'<br>',pu.errLimitCode+'<br>',pu.errMaxValue+'<br>',pu.errFamilyMaxValue) ErrorMessage 
                        FROM stg.PlanUpload pu
                            LEFT JOIN FrequencyCodes fc ON fc.ID = pu.FrequencyCode
                            LEFT JOIN LimitCodes lc ON lc.ID = pu.LimitCode
                            WHERE pu.EnrollmentHdrID=@EnrollmentHdrID ");

            var ls = new List<PlanUploadVM>();
            try
            {
                var _params = new SqlParameter { ParameterName = "@EnrollmentHdrID", SqlDbType = SqlDbType.Int, Value = _clientID };
                ls = await _context.Set<PlanUploadVM>().FromSql(query, _params).ToListAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CoverageUploadListByClient([DataTablesRequest] DataTablesRequest dataRequest, string EnrollmentHdrID)
        {
            int.TryParse(EnrollmentHdrID.Trim(), out int _clientID);
            var query = string.Format(@"SELECT cu.ID,cu.PlanId,cu.CorpCode,cu.CoverageCode,cu.ActiveFlag,
                                        cu.ClientCoverageCode,lc.Description LimitCode,fc.Description FrequencyCode,cu.MinValue,cu.MaxValue,
                                        cu.FamilyValue,CONCAT(CONVERT(VARCHAR(30),cu.UploadDate,106),' ',CONVERT(VARCHAR(30),cu.UploadDate,108)) UploadDate,
                                        cu.UserUpload,CONCAT(cu.errPlanId+'<br>',cu.errCorpCode+'<br>',cu.errCoverageCode+'<br>',cu.errActiveFlag+'<br>',cu.errClientCoverageCode+'<br>',
                                        cu.errLimitCode+'<br>',cu.errFrequencyCode+'<br>',cu.errMaxValue+'<br>',cu.errFamilyValue+'<br>') ErrorMessage 
                                    FROM stg.CoverageUpload cu
                                    LEFT JOIN FrequencyCodes fc ON fc.ID = cu.FrequencyCode
                                    LEFT JOIN LimitCodes lc ON lc.ID = Cu.LimitCode
                                    WHERE cu.EnrollmentHdrID=@EnrollmentHdrID ");

            var ls = new List<CoverageUploadVM>();
            try
            {
                var _params = new SqlParameter { ParameterName = "@EnrollmentHdrID", SqlDbType = SqlDbType.Int, Value = _clientID };
                ls = await _context.Set<CoverageUploadVM>().FromSql(query, _params).ToListAsync();
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
            int.TryParse(EnrollmentHdrID.Trim(), out int _clientID);
            var query = string.Format(@"SELECT bu.ID,bu.PlanId,bu.CorpCode,cc.ShortName CoverageCode,bu.BenefitCode,bc.Code BenefitShortName,
                                    bc.Description BenefitDescription,bu.ActiveFlag,bu.ConditionDescription,bu.LOA_Description,bu.ClientBenefitcode,
                                    lc.Description LimitCode,fc.Description FrequencyCode,bu.MaxValue,bu.MultipleCondition,
                                    CONCAT(CONVERT(VARCHAR(30),bu.UploadDate,106),' ',CONVERT(VARCHAR(30),bu.UploadDate,108)) UploadDate,bu.UserUpload,
                                    CONCAT(bu.errPlanId+'<br>',bu.errCorpCode+'<br>',bu.errCoverageCode+'<br>',bu.errBenefitCode+'<br>',bu.errConditionDescription+'<br>',
                                    bu.errLOA_Description+'<br>',bu.errActiveFlag+'<br>',bu.errClientBenefitcode+'<br>',bu.errLimitCode+'<br>',bu.errFrequencyCode+'<br>',
                                    bu.errMaxValue+'<br>') ErrorMessage
                                FROM stg.BenefitUpload bu
                                LEFT JOIN dbo.CoverageCodes cc ON cc.ID = bu.CoverageCode
                                LEFT JOIN dbo.BenefitCodes bc ON bc.ID = bu.BenefitCode
                                LEFT JOIN FrequencyCodes fc ON fc.ID = bu.FrequencyCode
                                LEFT JOIN LimitCodes lc ON lc.ID = bu.LimitCode
                                    WHERE bu.EnrollmentHdrID=@EnrollmentHdrID ");
            var ls = new List<BenefitUploadVM>();
            try
            {
                var _params = new SqlParameter { ParameterName = "@EnrollmentHdrID", SqlDbType = SqlDbType.Int, Value = _clientID };
                ls = await _context.Set<BenefitUploadVM>().FromSql(query, _params).ToListAsync();
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
            int tmp; DateTime tmpdate;
            var CleanPlan = new EnrollPlanFileExcelDataVM();
            CleanPlan.ClientID = pln.ClientID;
            CleanPlan.ClientCode = pln.ClientCode;
            CleanPlan.PlanUploadModel = new List<PlanUploadModel>();
            CleanPlan.CoverageUploadModel = new List<CoverageUploadModel>();
            CleanPlan.BenefitUploadModel = new List<BenefitUploadModel>();

            foreach (var item in pln.PlanUploadModel)
            {
                var newItem = new PlanUploadModel
                {
                    PayorCode = (item.PayorCode ?? "").Trim(),
                    PlanId = (item.PlanId ?? "").Trim(),
                    CorpCode = (item.CorpCode ?? "").Trim(),
                    EffectiveDate = (item.EffectiveDate ?? "").Trim(),
                    TerminationDate = (item.TerminationDate ?? "").Trim(),
                    ActiveFlag = (item.ActiveFlag ?? "").Trim(),
                    ShortName = (item.ShortName ?? "").Trim(),
                    LongName = (item.LongName ?? "").Trim(),
                    Remarks = (item.Remarks ?? "").Trim(),
                    PolicyNo = (item.PolicyNo ?? "").Trim(),
                    FrequencyCode = (item.FrequencyCode ?? "").Trim(),
                    LimitCode = (item.LimitCode ?? "").Trim(),
                    MaxValue = (item.MaxValue ?? "").Trim(),
                    FamilyMaxValue = (item.FamilyMaxValue ?? "0").Trim(),
                    PrintText = (item.PrintText ?? "").Trim(),
                    UploadDate = item.UploadDate,
                    UserUpload = item.UserUpload
                };
                //newItem.ErrorMessage = item.ErrorMessage;

                if (newItem.EffectiveDate.Length != 8) newItem.errEffectiveDate = "EffectiveDate : Invalid Date";
                else if (!DateTime.TryParseExact(newItem.EffectiveDate, "yyyymmdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpdate)) newItem.errEffectiveDate = "EffectiveDate : Invalid Format Date";

                if (newItem.TerminationDate.Length != 8) newItem.errTerminationDate = "TerminationDate : Invalid Date";
                else if (!DateTime.TryParseExact(newItem.TerminationDate, "yyyymmdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpdate)) newItem.errTerminationDate = "TerminationDate : Invalid Format Date";

                if (newItem.PlanId.Trim() == "") newItem.errPlanId = "PlanID : Can not be empty";
                if (newItem.ActiveFlag.Trim().ToUpper() != "Y" && newItem.ActiveFlag != "N") newItem.errActiveFlag = "ActiveFlag : Invalid Character";
                if (newItem.CorpCode != pln.ClientCode) newItem.errCorpCode = "CorpCode : Incorrect ClientCode for this client";
                if (!int.TryParse(newItem.FrequencyCode, out tmp)) newItem.errFrequencyCode = "FrequencyCode : Invalid Number";
                if (!int.TryParse(newItem.LimitCode, out tmp)) newItem.errLimitCode = "LimitCode : Invalid Number";
                if (!int.TryParse(newItem.MaxValue, out tmp)) newItem.errMaxValue = "MaxValue : Invalid Number";
                if (!int.TryParse(newItem.FamilyMaxValue, out tmp)) newItem.errFamilyMaxValue = "FamilyMaxValue : Invalid Number";

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
                newItem.MinValue = (item.MinValue ?? "0").Trim();
                newItem.MaxValue = (item.MaxValue ?? "").Trim();
                newItem.FamilyValue = (item.FamilyValue ?? "0").Trim();
                newItem.UploadDate = item.UploadDate;
                newItem.UserUpload = item.UserUpload;
                //newItem.ErrorMessage = item.ErrorMessage;

                if (newItem.PlanId.Trim() == "") newItem.errPlanId = "PlanID : Can not be empty";
                if (newItem.ActiveFlag.Trim().ToUpper() != "Y" && newItem.ActiveFlag != "N") newItem.errActiveFlag = "ActiveFlag : Invalid Character";
                if (newItem.CorpCode != pln.ClientCode) newItem.errCorpCode = "CorpCode : Incorrect ClientCode for this client";

                if (!int.TryParse(newItem.CoverageCode, out tmp)) newItem.errCoverageCode = "CoverageCode : Invalid Number";
                if (!int.TryParse(newItem.FrequencyCode, out tmp)) newItem.errFrequencyCode = "FrequencyCode : Invalid Number";
                if (!int.TryParse(newItem.LimitCode, out tmp)) newItem.errLimitCode = "LimitCode : Invalid Number";
                if (!int.TryParse(newItem.MaxValue, out tmp)) newItem.errMaxValue = "MaxValue : Invalid Number";
                if (!int.TryParse(newItem.FamilyValue, out tmp)) newItem.errFamilyValue = "FamilyValue : Invalid Number";

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
                //newItem.ErrorMessage = item.ErrorMessage;

                if (newItem.PlanId.Trim() == "") newItem.errPlanId = "PlanID : Can not be empty";

                if (newItem.CorpCode != pln.ClientCode) newItem.errCorpCode = "CorpCode : Incorrect ClientCode for this client";
                if (newItem.ActiveFlag.Trim().ToUpper() != "Y" && newItem.ActiveFlag != "N") newItem.errActiveFlag = "ActiveFlag : Invalid Character";

                if (!int.TryParse(newItem.CoverageCode, out tmp)) newItem.errCoverageCode = "CoverageCode : Invalid Number";
                if (!int.TryParse(newItem.BenefitCode, out tmp)) newItem.errBenefitCode = "BenefitCode : Invalid Number";
                if (newItem.ConditionDescription.Trim() == "") newItem.errConditionDescription = "ConditionDescription : Can not be empty";
                if (newItem.LOA_Description.Trim() == "") newItem.errLOA_Description = "LOA_Description : Can not be empty";
                if (newItem.ClientBenefitcode.Trim() == "") newItem.errClientBenefitcode = "ClientBenefitcode : Can not be empty";

                if (!int.TryParse(newItem.FrequencyCode, out tmp)) newItem.errFrequencyCode = "FrequencyCode : Invalid Number";
                if (!int.TryParse(newItem.LimitCode, out tmp)) newItem.errLimitCode = "LimitCode : Invalid Number";
                if (!int.TryParse(newItem.MaxValue, out tmp)) newItem.errMaxValue = "MaxValue : Invalid Number";
                //if (!int.TryParse(newItem.FamilyValue, out tmp)) newItem.errFamilyValue = "FamilyValue : Invalid Number";


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

                    if (clPrd.PlanUploadModel.Count() <= 0) throw new Exception("Empty Data Plan for insert");
                    if (clPrd.CoverageUploadModel.Count() <= 0) throw new Exception("Empty Data coveverage for insert");
                    if (clPrd.BenefitUploadModel.Count() <= 0) throw new Exception("Empty Data Benefit for insert");

                    _context.BulkInsert(clPrd.PlanUploadModel);
                    _context.BulkInsert(clPrd.CoverageUploadModel);
                    _context.BulkInsert(clPrd.BenefitUploadModel);

                    try
                    {
                        var _params = new SqlParameter { ParameterName = "@EnrollmentHdrID", SqlDbType = SqlDbType.Int, Value = _EnrolH.ID };
                        //validasi  data plan Upload
                        _context.Database.ExecuteSqlCommand("exec ValidasiUploadPlan @EnrollmentHdrID", _params);
                        // validasi data coverage upload
                        _context.Database.ExecuteSqlCommand("exec ValidasiUploadCoverage @EnrollmentHdrID", _params);
                        // validasi data benefit upload
                        _context.Database.ExecuteSqlCommand("exec ValidasiUploadBenefit @EnrollmentHdrID", _params);
                    }
                    catch (Exception ex) { throw new Exception(ex.Message); }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}

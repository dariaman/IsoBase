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

namespace IsoBase.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IFlashMessage flashMessage;
        private static string paternAngka { get; } = @"[^0-9]";
        private IConfiguration Configuration { get; }

        public EnrollmentController(ApplicationDbContext context, IFlashMessage flash, IConfiguration configuration)
        {
            _context = context;
            flashMessage = flash;
            Configuration = configuration;
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
            string _urlBack = (Request.Headers["Referer"].ToString() == "" ? "Index" : Request.Headers["Referer"].ToString());
            if (ClientfilePlan.Fileupload == null || ClientfilePlan.Fileupload.Length == 0)
            {
                flashMessage.Danger("File Not Selected");
                return Redirect(_urlBack);
            }

            // ==============End Validation========================

            var path = Path.Combine(Configuration.GetConnectionString("FilePlanExcel"), ClientfilePlan.Fileupload.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await ClientfilePlan.Fileupload.CopyToAsync(stream);
            }
            return RedirectToAction("index");
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
        public IActionResult PlanByClientListAll([DataTablesRequest] DataTablesRequest dataRequest, string clientID)
        {
            var pgData = new PageData(dataRequest, _context)
            {
                select = @"SELECT pl.ID,pl.ClientID,pl.ClientPlanID,pl.PolicyNo,pl.ShortName,pl.LongName,fc.Description Frequency,lc.Description Limit, 
                            pl.MaxLimitValue,pl.IsActive,pl.UserCreate,pl.DateCreate ",
                Tabel = @" FROM dbo.PlanLimit pl
                            INNER JOIN dbo.FrequencyCodes fc ON fc.ID = pl.FrequencyCodeID
                            INNER JOIN dbo.LimitCodes lc ON lc.ID = pl.LimitCodeID
                            WHERE pl.ClientID=1 ",
            };

            //defenisikan Where condition
            foreach (var req in dataRequest.Columns)
            {
                if (string.IsNullOrEmpty(req.SearchValue)) continue;
                else if (req.Data == "id") pgData.AddWhereRegex(pgData.paternAngkaLike, req.SearchValue, "ID");
                else if (req.Data == "clientID") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "ClientID");
                else if (req.Data == "policyNo") pgData.AddWhereRegex(pgData.paternAngka, req.SearchValue, "PolicyNo");
                else if (req.Data == "planCode") pgData.AddWhereRegex(pgData.paternAngka, req.SearchValue, "PlanCode");
            }
            pgData.CountData(); // hitung jlh total data dan total dataFilter

            DataTable _dt = new DataTable();
            try
            {
                _dt = pgData.ListData();
            }
            catch (Exception ex) { flashMessage.Info(ex.Message); }

            List<PlanVM> ls = new List<PlanVM>();
            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    ls.Add(new PlanVM
                    {
                        ID = row["ID"].ToString(),
                        ClientID = row["ClientID"].ToString(),
                        PolicyNo = row["PolicyNo"].ToString(),
                        ClientPlanID = row["PlanCode"].ToString(),

                        ShortName = row["ShortName"].ToString(),
                        LongName = row["LongName"].ToString(),
                        Frequency = row["Frequency"].ToString(),
                        Limit = row["Limit"].ToString(),
                        MaxLimitValue = row["MaxLimitValue"].ToString(),

                        IsActive = row["IsActive"].ToString(),
                        UserCreate = row["UserCreate"].ToString(),
                        DateCreate = row["DateCreate"].ToString(),
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

        private ClientModel ClientFindByID(int id)
        {
            return _context.ClientModel.Where(x => x.ID == id).FirstOrDefault();
        }

    }
}
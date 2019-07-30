using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataTables.AspNetCore.Mvc.Binder;
using IsoBase.Data;
using IsoBase.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;

namespace IsoBase.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IFlashMessage flashMessage;

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
            return View();
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
                else if (req.Data == "clientName") pgData.AddWhereRegex(pgData.paternAngka, req.SearchValue, "Name");
            }
            pgData.CountData(); // hitung jlh total data dan total dataFilter

            DataTable _dt = new DataTable();
            try
            {
                _dt = pgData.ListData();
            }
            catch (Exception ex) { flashMessage.Info(ex.Message);  }

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
        public IActionResult PlanByClientListAll([DataTablesRequest] DataTablesRequest dataRequest,string clientID)
        {
            var pgData = new PageData(dataRequest, _context)
            {
                select = @"SELECT ID,ClientID,PolicyNo,PlanCode,IsActive,UserCreate,DateCreate ",
                Tabel = @" FROM dbo.PlanLimit WHERE IsActive=1 ",
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
                        PlanCode = row["PlanCode"].ToString(),
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

    }
}
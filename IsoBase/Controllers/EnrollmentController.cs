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
            flashMessage.Info("Your informational message");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListClientActive([DataTablesRequest] DataTablesRequest dataRequest)
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
                else if (req.Data == "id") pgData.AddWhereRegex(pgData.paternAngkaLike, req.SearchValue, "ID");
                else if (req.Data == "clientCode") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "ClientCode");
                else if (req.Data == "name") pgData.AddWhereRegex(pgData.paternAngka, req.SearchValue, "Name");
            }
            pgData.CountData(); // hitung jlh total data dan total dataFilter

            DataTable _dt = new DataTable();
            try
            {
                _dt = pgData.ListData();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

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
                throw new Exception(ex.Message);
            }
            return Json(ls.ToDataTablesResponse(dataRequest, pgData.recordsTotal, pgData.recordsFilterd));
        }

    }
}
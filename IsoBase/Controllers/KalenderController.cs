using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IsoBase.Data;
using IsoBase.Models;
using DataTables.AspNetCore.Mvc.Binder;
using Vereyon.Web;
using IsoBase.ViewModels;

namespace IsoBase.Controllers
{
    public class KalenderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlashMessage flashMessage;

        public KalenderController(ApplicationDbContext context, IFlashMessage flash)
        {
            _context = context;
            flashMessage = flash;
        }

        // GET: Kalender
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListTglOperational([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var query = @"SELECT ID,CONVERT(VARCHAR(30),Tgl) Tgl,DayNumMonth,DayNumYear,DayNameEn,DayNameInd,MonthYear,MonthNameEn,MonthNameInd,YearNumber,
                        CASE IsHoliday WHEN 1 THEN 'Yes' ELSE 'No' END IsHoliday,COALESCE(DateCreate,DateUpdate) LastUpdate,COALESCE(UserCreate,UserUpdate) UserUpdate
                        FROM dbo.KalenderOperational WITH(NOLOCK) ";
            var ls = new List<KalenderVM>();

            try
            {
                ls = await _context.Set<KalenderVM>().FromSql(query).ToListAsync();
            }
            catch (Exception ex)
            {
                flashMessage.Danger("Error Paging ListTglOperational : " + ex.Message);
                throw new Exception();
            }

            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
        }


        private bool KalenderOperationalModelExists(int id)
        {
            return _context.KalenderOperationalModel.Any(e => e.ID == id);
        }
    }
}

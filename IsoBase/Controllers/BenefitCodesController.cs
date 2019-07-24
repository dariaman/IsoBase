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
using System.Data;

namespace IsoBase.Controllers
{
    public class BenefitCodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BenefitCodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BenefitCodes
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListBenefitAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var pgData = new PageData(dataRequest, _context)
            {
                select = @"SELECT Code,LOA_Desc,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate ",
                Tabel = @" FROM BenefitCodes WITH(NOLOCK) WHERE 1=1 ",
            };

            //defenisikan Where condition
            foreach (var req in dataRequest.Columns)
            {
                if (string.IsNullOrEmpty(req.SearchValue)) continue;
                else if (req.Data == "code") pgData.AddWhereRegex(pgData.paternAngkaLike, req.SearchValue, "Code");
                else if (req.Data == "loa") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "LOA_Desc");
                else if (req.Data == "description") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "Description");
                else if (req.Data == "isActive") pgData.AddWhereRegex(pgData.paternAngka, req.SearchValue, "IsActive");
            }
            pgData.CountData(); // hitung jlh total data dan total dataFilter

            DataTable _dt = new DataTable();
            try
            {
                _dt = pgData.ListData();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

            List<BenefitCodesModel> ls = new List<BenefitCodesModel>();
            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    ls.Add(new BenefitCodesModel
                    {
                        Code = (int)row["Code"],
                        Loa = row["LOA_Desc"].ToString(),
                        Description = row["Description"].ToString(),

                        IsActive = (Boolean)row["IsActive"],
                        UserCreate = row["UserCreate"].ToString(),
                        DateCreate = (DateTime)row["DateCreate"],
                        UserUpdate = row["UserUpdate"].ToString(),
                        DateUpdate = row["DateUpdate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["DateUpdate"]),
                    });
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Json(ls.ToDataTablesResponse(dataRequest, pgData.recordsTotal, pgData.recordsFilterd));
        }


        // GET: BenefitCodes/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var benefitCodesModel = await _context.BenefitCodesModel
        //        .FirstOrDefaultAsync(m => m.Code == id);
        //    if (benefitCodesModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(benefitCodesModel);
        //}

        // GET: BenefitCodes/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: BenefitCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Code,LOA_Desc,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] BenefitCodesModel benefitCodesModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(benefitCodesModel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(benefitCodesModel);
        //}

        //// GET: BenefitCodes/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var benefitCodesModel = await _context.BenefitCodesModel.FindAsync(id);
        //    if (benefitCodesModel == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(benefitCodesModel);
        //}

        // POST: BenefitCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Code,LOA_Desc,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] BenefitCodesModel benefitCodesModel)
        //{
        //    if (id != benefitCodesModel.Code)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(benefitCodesModel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BenefitCodesModelExists(benefitCodesModel.Code))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(benefitCodesModel);
        //}

        //// GET: BenefitCodes/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var benefitCodesModel = await _context.BenefitCodesModel
        //        .FirstOrDefaultAsync(m => m.Code == id);
        //    if (benefitCodesModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(benefitCodesModel);
        //}

        //// POST: BenefitCodes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var benefitCodesModel = await _context.BenefitCodesModel.FindAsync(id);
        //    _context.BenefitCodesModel.Remove(benefitCodesModel);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool BenefitCodesModelExists(int id)
        {
            return _context.BenefitCodesModel.Any(e => e.Code == id);
        }
    }
}

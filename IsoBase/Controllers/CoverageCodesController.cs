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
    public class CoverageCodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoverageCodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CoverageCodes
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListCoverageCodesAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var pgData = new PageData(dataRequest, _context)
            {
                select = @"SELECT ID,ShortName,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate ",
                Tabel = @" FROM CoverageCodes WITH(NOLOCK) WHERE 1=1 ",
            };

            //defenisikan Where condition
            foreach (var req in dataRequest.Columns)
            {
                if (string.IsNullOrEmpty(req.SearchValue)) continue;
                else if (req.Data == "id") pgData.AddWhereRegex(pgData.paternAngkaLike, req.SearchValue, "ID");
                else if (req.Data == "shortName") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "ShortName");
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

            List<CoverageCodesModel> ls = new List<CoverageCodesModel>();
            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    ls.Add(new CoverageCodesModel
                    {
                        ID = (int)row["ID"],
                        ShortName = row["ShortName"].ToString(),
                        Description = row["Description"].ToString(),

                        IsActive = (Boolean)row["IsActive"],
                        UserCreate = row["UserCreate"].ToString(),
                        DateCreate = (DateTime)row["DateCreate"],
                        UserUpdate = row["UserUpdate"].ToString(),
                        DateUpdate = row["DateUpdate"] == DBNull.Value ? (DateTime?)null : (DateTime?)row["DateUpdate"],
                    });
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Json(ls.ToDataTablesResponse(dataRequest, pgData.recordsTotal, pgData.recordsFilterd));
        }

        // GET: CoverageCodes/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var coverageCodesModel = await _context.CoverageCodesModel
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (coverageCodesModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(coverageCodesModel);
        //}

        //// GET: CoverageCodes/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: CoverageCodes/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,ShortName,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] CoverageCodesModel coverageCodesModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(coverageCodesModel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(coverageCodesModel);
        //}

        //// GET: CoverageCodes/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var coverageCodesModel = await _context.CoverageCodesModel.FindAsync(id);
        //    if (coverageCodesModel == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(coverageCodesModel);
        //}

        //// POST: CoverageCodes/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ID,ShortName,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] CoverageCodesModel coverageCodesModel)
        //{
        //    if (id != coverageCodesModel.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(coverageCodesModel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CoverageCodesModelExists(coverageCodesModel.ID))
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
        //    return View(coverageCodesModel);
        //}

        //// GET: CoverageCodes/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var coverageCodesModel = await _context.CoverageCodesModel
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (coverageCodesModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(coverageCodesModel);
        //}

        //// POST: CoverageCodes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var coverageCodesModel = await _context.CoverageCodesModel.FindAsync(id);
        //    _context.CoverageCodesModel.Remove(coverageCodesModel);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool CoverageCodesModelExists(int id)
        {
            return _context.CoverageCodesModel.Any(e => e.ID == id);
        }
    }
}

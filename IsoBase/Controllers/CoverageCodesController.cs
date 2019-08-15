using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IsoBase.Data;
using IsoBase.Models;
using DataTables.AspNetCore.Mvc.Binder;
using Vereyon.Web;

namespace IsoBase.Controllers
{
    public class CoverageCodesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlashMessage flashMessage;

        public CoverageCodesController(ApplicationDbContext context, IFlashMessage flash)
        {
            _context = context;
            flashMessage = flash;
        }

        // GET: CoverageCodes
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListCoverageCodesAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var query = @"SELECT ID,ShortName,Description,CASE IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate FROM CoverageCodes WITH(NOLOCK) ";
            var ls = new List<CoverageCodesModel>();
            try
            {
                ls = await _context.Set<CoverageCodesModel>().FromSql(query).ToListAsync();
            }
            catch (Exception ex)
            {
                flashMessage.Danger("Error Paging CoverageCodes : " + ex.Message);
                throw new Exception();
            }

            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
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

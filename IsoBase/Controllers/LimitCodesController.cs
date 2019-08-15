using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using IsoBase.Data;
using IsoBase.Models;
using DataTables.AspNetCore.Mvc.Binder;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Vereyon.Web;

namespace IsoBase.Controllers
{
    public class LimitCodesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlashMessage flashMessage;

        public LimitCodesController(ApplicationDbContext context, IFlashMessage flash)
        {
            _context = context;
            flashMessage = flash;
        }

        // GET: LimitCodes
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListLimitCodeAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var query = @"SELECT ID,Description,CASE IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate FROM LimitCodes WITH(NOLOCK) ";
            var ls = new List<LimitCodesModel>();

            try
            {
                ls = await _context.Set<LimitCodesModel>().FromSql(query).ToListAsync();
            }
            catch (Exception ex)
            {
                flashMessage.Danger("Error Paging LimitCode : " + ex.Message);
                throw new Exception();
            }

            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
        }

        // GET: LimitCodes/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var limitCodesModel = await _context.LimitCodesModel
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (limitCodesModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(limitCodesModel);
        //}

        //// GET: LimitCodes/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: LimitCodes/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] LimitCodesModel limitCodesModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(limitCodesModel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(limitCodesModel);
        //}

        //// GET: LimitCodes/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var limitCodesModel = await _context.LimitCodesModel.FindAsync(id);
        //    if (limitCodesModel == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(limitCodesModel);
        //}

        //// POST: LimitCodes/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ID,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] LimitCodesModel limitCodesModel)
        //{
        //    if (id != limitCodesModel.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(limitCodesModel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!LimitCodesModelExists(limitCodesModel.ID))
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
        //    return View(limitCodesModel);
        //}

        //// GET: LimitCodes/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var limitCodesModel = await _context.LimitCodesModel
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (limitCodesModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(limitCodesModel);
        //}

        //// POST: LimitCodes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var limitCodesModel = await _context.LimitCodesModel.FindAsync(id);
        //    _context.LimitCodesModel.Remove(limitCodesModel);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool LimitCodesModelExists(int id)
        {
            return _context.LimitCodesModel.Any(e => e.ID == id);
        }
    }
}

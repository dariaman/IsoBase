using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using IsoBase.Data;
using IsoBase.Models;
using DataTables.AspNetCore.Mvc.Binder;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Vereyon.Web;

namespace IsoBase.Controllers
{
    public class FrequencyCodesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlashMessage flashMessage;

        public FrequencyCodesController(ApplicationDbContext context, IFlashMessage flash)
        {
            _context = context;
            flashMessage = flash;
        }

        // GET: FrequencyCodes
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListFrequencyCodeAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var query = @"SELECT ID,Description,CASE IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate FROM FrequencyCodes WITH(NOLOCK) ";
            var ls = new List<FrequencyCodesModel>();

            try
            {
                ls = await _context.Set<FrequencyCodesModel>().FromSql(query).ToListAsync();
            }
            catch (Exception ex)
            {
                flashMessage.Danger("Error Paging FrequencyCode : " + ex.Message);
                throw new Exception();
            }

            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
        }


        // GET: FrequencyCodes/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var frequencyCodesModel = await _context.FrequencyCodesModel
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (frequencyCodesModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(frequencyCodesModel);
        //}

        //// GET: FrequencyCodes/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: FrequencyCodes/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] FrequencyCodesModel frequencyCodesModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(frequencyCodesModel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(frequencyCodesModel);
        //}

        //// GET: FrequencyCodes/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var frequencyCodesModel = await _context.FrequencyCodesModel.FindAsync(id);
        //    if (frequencyCodesModel == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(frequencyCodesModel);
        //}

        //// POST: FrequencyCodes/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ID,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] FrequencyCodesModel frequencyCodesModel)
        //{
        //    if (id != frequencyCodesModel.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(frequencyCodesModel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!FrequencyCodesModelExists(frequencyCodesModel.ID))
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
        //    return View(frequencyCodesModel);
        //}

        //// GET: FrequencyCodes/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var frequencyCodesModel = await _context.FrequencyCodesModel
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (frequencyCodesModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(frequencyCodesModel);
        //}

        //// POST: FrequencyCodes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var frequencyCodesModel = await _context.FrequencyCodesModel.FindAsync(id);
        //    _context.FrequencyCodesModel.Remove(frequencyCodesModel);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool FrequencyCodesModelExists(int id)
        {
            return _context.FrequencyCodesModel.Any(e => e.ID == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IsoBase.Data;
using IsoBase.Models;

namespace IsoBase.Controllers
{
    public class KalenderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KalenderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Kalender
        public async Task<IActionResult> Index()
        {
            return View(await _context.KalenderOperationalModel.ToListAsync());
        }

        //// GET: Kalender/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var kalenderOperationalModel = await _context.KalenderOperationalModel
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (kalenderOperationalModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(kalenderOperationalModel);
        //}

        //// GET: Kalender/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Kalender/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,Tgl,DayNumMonth,DayNumYear,DayNameEn,DayNameInd,MonthYear,MonthNameEn,MonthNameInd,YearNumber,IsHoliday,UserCreate,DateCreate,UserUpdate,DateUpdate")] KalenderOperationalModel kalenderOperationalModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(kalenderOperationalModel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(kalenderOperationalModel);
        //}

        //// GET: Kalender/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var kalenderOperationalModel = await _context.KalenderOperationalModel.FindAsync(id);
        //    if (kalenderOperationalModel == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(kalenderOperationalModel);
        //}

        //// POST: Kalender/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ID,Tgl,DayNumMonth,DayNumYear,DayNameEn,DayNameInd,MonthYear,MonthNameEn,MonthNameInd,YearNumber,IsHoliday,UserCreate,DateCreate,UserUpdate,DateUpdate")] KalenderOperationalModel kalenderOperationalModel)
        //{
        //    if (id != kalenderOperationalModel.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(kalenderOperationalModel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!KalenderOperationalModelExists(kalenderOperationalModel.ID))
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
        //    return View(kalenderOperationalModel);
        //}

        //// GET: Kalender/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var kalenderOperationalModel = await _context.KalenderOperationalModel
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (kalenderOperationalModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(kalenderOperationalModel);
        //}

        //// POST: Kalender/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var kalenderOperationalModel = await _context.KalenderOperationalModel.FindAsync(id);
        //    _context.KalenderOperationalModel.Remove(kalenderOperationalModel);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool KalenderOperationalModelExists(int id)
        {
            return _context.KalenderOperationalModel.Any(e => e.ID == id);
        }
    }
}

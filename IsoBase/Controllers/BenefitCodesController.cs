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
    public class BenefitCodesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlashMessage flashMessage;

        public BenefitCodesController(ApplicationDbContext context, IFlashMessage flash)
        {
            _context = context;
            flashMessage = flash;
        }

        // GET: BenefitCodes
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListBenefitAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var query = @"SELECT ID,Code,Description,CASE IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate FROM BenefitCodes WITH(NOLOCK) WHERE 1=1 ";
            var ls = new List<BenefitCodesModel>();

            try
            {
                ls = await _context.Set<BenefitCodesModel>().FromSql(query).ToListAsync();
            }
            catch (Exception ex)
            {
                flashMessage.Danger("Error Paging FrequencyCode : " + ex.Message);
                throw new Exception();
            }

            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
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
            return _context.BenefitCodesModel.Any(e => e.ID == id);
        }
    }
}

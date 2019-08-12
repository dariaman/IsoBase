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
using IsoBase.ViewModels;
using Vereyon.Web;

namespace IsoBase.Controllers
{
    public class PicCodeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlashMessage flashMessage;

        public PicCodeController(ApplicationDbContext context, IFlashMessage flash)
        {
            _context = context;
            flashMessage = flash;
        }

        // GET: PicCode
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListPicCodeAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var query = @"SELECT ID,PicDesc,Remark,IsActive,DateCreate,UserCreate,DateUpdate,UserUpdate FROM dbo.PicCode ";
            List<PicCodeModel> ls;

            try
            {
                ls = await _context.Set<PicCodeModel>().FromSql(query).ToListAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

            return Json(ls.ToDataTablesResponse(dataRequest, ls.Count, ls.Count));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PicDesc,Remark")] CreatePicCodeVM cpm)
        {

            if (ModelState.IsValid)
            {
                PicCodeModel Picm = new PicCodeModel();
                Picm.PicDesc = cpm.PicDesc;
                Picm.Remark = cpm.Remark;

                _context.Add(Picm);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    flashMessage.Danger(ex.Message);
                }
            }
            return View(cpm);
        }

        // GET: PicCode/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var picCodeModel = await _context.PicCodeModel.FindAsync(id);
            if (picCodeModel == null)
            {
                return NotFound();
            }
            return View(picCodeModel);
        }

        // POST: PicCode/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,PicDesc,Remark,IsActive,DateCreate,UserCreate,DateUpdate,UserUpdate")] PicCodeModel picCodeModel)
        {
            if (id != picCodeModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(picCodeModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PicCodeModelExists(picCodeModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(picCodeModel);
        }

        // GET: PicCode/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var picCodeModel = await _context.PicCodeModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (picCodeModel == null)
            {
                return NotFound();
            }

            return View(picCodeModel);
        }

        // POST: PicCode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var picCodeModel = await _context.PicCodeModel.FindAsync(id);
            _context.PicCodeModel.Remove(picCodeModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PicCodeModelExists(int id)
        {
            return _context.PicCodeModel.Any(e => e.ID == id);
        }
    }
}

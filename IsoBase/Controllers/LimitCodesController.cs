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
    public class LimitCodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LimitCodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LimitCodes
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListLimitCodeAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var pgData = new PageData(dataRequest, _context)
            {
                select = @"SELECT ID,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate ",
                Tabel = @" FROM LimitCodes WITH(NOLOCK) WHERE 1=1 ",
            };

            //defenisikan Where condition
            foreach (var req in dataRequest.Columns)
            {
                if (string.IsNullOrEmpty(req.SearchValue)) continue;
                else if (req.Data == "id") pgData.AddWhereRegex(pgData.paternAngkaLike, req.SearchValue, "ID");
                else if (req.Data == "isActive") pgData.AddWhereRegex(pgData.paternAngka, req.SearchValue, "IsActive");
                else if (req.Data == "description") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "Description");
            }
            pgData.CountData(); // hitung jlh total data dan total dataFilter

            DataTable _dt = new DataTable();
            try
            {
                _dt = pgData.ListData();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

            List<LimitCodesModel> ls = new List<LimitCodesModel>();
            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    ls.Add(new LimitCodesModel
                    {
                        ID = (int)row["ID"],
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

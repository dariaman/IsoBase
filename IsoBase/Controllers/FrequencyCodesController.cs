using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using IsoBase.Data;
using IsoBase.Models;
using System.Data;
using DataTables.AspNetCore.Mvc.Binder;

namespace IsoBase.Controllers
{
    public class FrequencyCodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FrequencyCodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FrequencyCodes
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListFrequencyCodeAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var pgData = new PageData(dataRequest, _context)
            {
                select = @"SELECT ID,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate ",
                Tabel = @" FROM FrequencyCodes WITH(NOLOCK) WHERE 1=1 ",
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

            List<FrequencyCodesModel> ls = new List<FrequencyCodesModel>();
            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    ls.Add(new FrequencyCodesModel
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

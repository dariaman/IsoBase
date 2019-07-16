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

namespace IsoBase.Controllers
{
    public class ClientMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClientMaster
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClientMasterModel.ToListAsync());
        }

        public IActionResult Index2()
        {
            return View();
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult PageData([DataTablesRequest] DataTablesRequest dataRequest)
        {
            IEnumerable<ClientMasterModel> products = _context.ClientMasterModel.Skip(dataRequest.Start).Take(dataRequest.Length);
            int recordsTotal = 2000; // _context.Tabel1Model.Count();
            //int recordsTotal = products.Count();
            int recordsFilterd = 1000; // recordsTotal;

            return Json(products
                .Select(e => new
                {
                    e.ClientID,
                    e.ClientCode,
                    e.Name,
                    e.ClientTypeID,
                    e.UserCreate,
                    e.DateCreate,
                    e.UserUpdate,
                    e.DateUpdate
                })
                .ToDataTablesResponse(dataRequest, recordsTotal, recordsFilterd));
        }

        // GET: ClientMaster/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientMasterModels = await _context.ClientMasterModel
                .FirstOrDefaultAsync(m => m.ClientID == id);
            if (clientMasterModels == null)
            {
                return NotFound();
            }

            return View(clientMasterModels);
        }

        // GET: ClientMaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClientMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientID,ClientCode,Name,ClientTypeID,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] ClientMasterModel clientMasterModels)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clientMasterModels);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clientMasterModels);
        }

        // GET: ClientMaster/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientMasterModels = await _context.ClientMasterModel.FindAsync(id);
            if (clientMasterModels == null)
            {
                return NotFound();
            }
            return View(clientMasterModels);
        }

        // POST: ClientMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientID,ClientCode,Name,ClientTypeID,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] ClientMasterModel clientMasterModels)
        {
            if (id != clientMasterModels.ClientID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clientMasterModels);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientMasterModelsExists(clientMasterModels.ClientID))
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
            return View(clientMasterModels);
        }

        // GET: ClientMaster/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var clientMasterModels = await _context.ClientMasterModels
        //        .FirstOrDefaultAsync(m => m.ClientID == id);
        //    if (clientMasterModels == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(clientMasterModels);
        //}

        // POST: ClientMaster/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var clientMasterModels = await _context.ClientMasterModels.FindAsync(id);
        //    _context.ClientMasterModels.Remove(clientMasterModels);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool ClientMasterModelsExists(int id)
        {
            return _context.ClientMasterModel.Any(e => e.ClientID == id);
        }
    }
}

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
    public class ClientTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClientType
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClientTypeModel.ToListAsync());
        }

        [HttpGet()]
        [Route("clientAll")]
        public IActionResult Get([DataTablesRequest] DataTablesRequest dataRequest)
        {
            IEnumerable<ClientModel> products = _context.ClientModel.Skip(dataRequest.Start).Take(dataRequest.Length);
            int recordsTotal = 2000; // _context.Tabel1Model.Count();
            //int recordsTotal = products.Count();
            int recordsFilterd = 1000; // recordsTotal;

            //if (!string.IsNullOrEmpty(dataRequest.Search?.Value))
            //{
            //    //products = products.Where(e => e.policyno.Contains(dataRequest.Search.Value));
            //    recordsFilterd = products.Count();
            //}
            //products = products.Skip(dataRequest.Start).Take(dataRequest.Length);

            return Json(products
                .Select(e => new
                {
                    e.ID,
                    e.ClientCode,
                    e.Name
                })
                .ToDataTablesResponse(dataRequest, recordsTotal, recordsFilterd));
            //return Json(new {
            //    draw = dataRequest.Draw,
            //    recordsTotal = products.Count(),
            //    recordsFiltered = products.Count(),
            //    data = products.Select(e => new {  Id = e.Id, Name = e.Name, Created = e.Created, Price = 10 })
            //});
        }

        // GET: ClientType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientTypeModel = await _context.ClientTypeModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (clientTypeModel == null)
            {
                return NotFound();
            }

            return View(clientTypeModel);
        }

        // GET: ClientType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClientType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] ClientTypeModel clientTypeModel)
        {
            if (ModelState.IsValid)
            {
                clientTypeModel.UserCreate = User.Identity.Name;
                _context.Add(clientTypeModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clientTypeModel);
        }

        // GET: ClientType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientTypeModel = await _context.ClientTypeModel.FindAsync(id);
            if (clientTypeModel == null)
            {
                return NotFound();
            }
            return View(clientTypeModel);
        }

        // POST: ClientType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] ClientTypeModel clientTypeModel)
        {
            if (id != clientTypeModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clientTypeModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientTypeModelExists(clientTypeModel.ID))
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
            return View(clientTypeModel);
        }

        // GET: ClientType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientTypeModel = await _context.ClientTypeModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (clientTypeModel == null)
            {
                return NotFound();
            }

            return View(clientTypeModel);
        }

        // POST: ClientType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clientTypeModel = await _context.ClientTypeModel.FindAsync(id);
            _context.ClientTypeModel.Remove(clientTypeModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientTypeModelExists(int id)
        {
            return _context.ClientTypeModel.Any(e => e.ID == id);
        }
    }
}

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
            try
            {
                var dd = _context.ClientTypeModel.ToListAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);

            }

            return View(await _context.ClientTypeModel.ToListAsync());
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

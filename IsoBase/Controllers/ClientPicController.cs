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
    public class ClientPicController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientPicController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClientPic
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClientPicModel.ToListAsync());
        }

        // GET: ClientPic/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientPicModel = await _context.ClientPicModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (clientPicModel == null)
            {
                return NotFound();
            }

            return View(clientPicModel);
        }

        // GET: ClientPic/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClientPic/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ClientID,PicCodeID,PicName,PicTitle,AddressBuilding,Phone,PhoneExt,HP,Email,Fax,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] ClientPicModel clientPicModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clientPicModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clientPicModel);
        }

        // GET: ClientPic/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientPicModel = await _context.ClientPicModel.FindAsync(id);
            if (clientPicModel == null)
            {
                return NotFound();
            }
            return View(clientPicModel);
        }

        // POST: ClientPic/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ClientID,PicCodeID,PicName,PicTitle,AddressBuilding,Phone,PhoneExt,HP,Email,Fax,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] ClientPicModel clientPicModel)
        {
            if (id != clientPicModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clientPicModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientPicModelExists(clientPicModel.ID))
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
            return View(clientPicModel);
        }

        // GET: ClientPic/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientPicModel = await _context.ClientPicModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (clientPicModel == null)
            {
                return NotFound();
            }

            return View(clientPicModel);
        }

        // POST: ClientPic/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clientPicModel = await _context.ClientPicModel.FindAsync(id);
            _context.ClientPicModel.Remove(clientPicModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientPicModelExists(int id)
        {
            return _context.ClientPicModel.Any(e => e.ID == id);
        }
    }
}

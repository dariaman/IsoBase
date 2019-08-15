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
    public class ClientAccBankController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientAccBankController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClientAccBank
        public IActionResult Index()
        {
            return View();
        }


        // GET: ClientAccBank/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClientAccBank/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ClientID,BankCode,AccountName,AccountNo,Remark,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] ClientAccBankModel clientAccBankModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clientAccBankModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clientAccBankModel);
        }

        // GET: ClientAccBank/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientAccBankModel = await _context.ClientAccBankModel.FindAsync(id);
            if (clientAccBankModel == null)
            {
                return NotFound();
            }
            return View(clientAccBankModel);
        }

        // POST: ClientAccBank/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ClientID,BankCode,AccountName,AccountNo,Remark,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] ClientAccBankModel clientAccBankModel)
        {
            if (id != clientAccBankModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clientAccBankModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientAccBankModelExists(clientAccBankModel.ID))
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
            return View(clientAccBankModel);
        }

        // GET: ClientAccBank/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientAccBankModel = await _context.ClientAccBankModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (clientAccBankModel == null)
            {
                return NotFound();
            }

            return View(clientAccBankModel);
        }

        // POST: ClientAccBank/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clientAccBankModel = await _context.ClientAccBankModel.FindAsync(id);
            _context.ClientAccBankModel.Remove(clientAccBankModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientAccBankModelExists(int id)
        {
            return _context.ClientAccBankModel.Any(e => e.ID == id);
        }
    }
}

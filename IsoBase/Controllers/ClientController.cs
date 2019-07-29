using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IsoBase.Data;
using IsoBase.Models;
using DataTables.AspNetCore.Mvc.Binder;
using System.Data;
using IsoBase.ViewModels;

namespace IsoBase.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListClientAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var pgData = new PageData(dataRequest, _context)
            {
                select = @"SELECT cm.ID,cm.ClientCode,cm.Name ClientName,cm.ClientTypeID,ct.Name ClientTypeName,
                            cm.IsActive,cm.DateCreate,cm.UserCreate,ct.DateUpdate,ct.UserUpdate ",
                Tabel = @" FROM Client cm WITH(NOLOCK)
                            INNER JOIN dbo.ClientType ct WITH(NOLOCK) ON ct.ID = cm.ClientTypeID
                            WHERE 1=1 ",
            };

            //defenisikan Where condition
            foreach (var req in dataRequest.Columns)
            {
                if (string.IsNullOrEmpty(req.SearchValue)) continue;
                else if (req.Data == "clientID") pgData.AddWhereRegex(pgData.paternAngkaLike, req.SearchValue, "cm.ID");
                else if (req.Data == "clientCode") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "cm.ClientCode");
                else if (req.Data == "clientName") pgData.AddWhereRegex(pgData.paternAngkaHurufLike, req.SearchValue, "cm.Name");
                else if (req.Data == "clientTypeName") pgData.AddWhereRegex(pgData.paternAngka, req.SearchValue, "cm.ClientTypeID");
                else if (req.Data == "isActive") pgData.AddWhereRegex(pgData.paternAngka, req.SearchValue, "cm.IsActive");
            }
            pgData.CountData(); // hitung jlh total data dan total dataFilter

            DataTable _dt = new DataTable();
            try
            {
                _dt = pgData.ListData();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

            List<ClientListVM> ls = new List<ClientListVM>();
            try
            {
                foreach (DataRow row in _dt.Rows)
                {
                    ls.Add(new ClientListVM
                    {
                        ClientID = row["ID"].ToString(),
                        ClientCode = row["ClientCode"].ToString(),
                        ClientName = row["ClientName"].ToString(),
                        ClientTypeName = row["ClientTypeName"].ToString(),
                        IsActive = row["IsActive"].ToString(),
                        UserCreate = row["UserCreate"].ToString(),
                        DateCreate = row["DateCreate"].ToString(),
                        UserUpdate = row["UserUpdate"].ToString(),
                        DateUpdate = row["DateUpdate"].ToString(),
                    });
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Json(ls.ToDataTablesResponse(dataRequest, pgData.recordsTotal, pgData.recordsFilterd));
        }

        // GET: ClientMaster/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }

            var clientMasterModels = await _context.ClientModel
                .FirstOrDefaultAsync(m => m.ID == id);
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
        public async Task<IActionResult> Create([Bind("ClientID,ClientCode,Name,ClientTypeID,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] ClientModel clientMasterModels)
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

            var clientMasterModels = await _context.ClientModel.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("ClientID,ClientCode,Name,ClientTypeID,IsActive,UserCreate,DateCreate,UserUpdate,DateUpdate")] ClientModel clientMasterModels)
        {
            if (id != clientMasterModels.ID)
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
                    if (!ClientMasterModelsExists(clientMasterModels.ID))
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
            return _context.ClientModel.Any(e => e.ID == id);
        }
    }
}

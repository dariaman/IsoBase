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
using System.Data.SqlClient;
using IsoBase.ViewModels;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListClientAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var pgData = new PageData(dataRequest)
            {
                select = @"SELECT cm.ClientID,cm.ClientCode,cm.Name ClientName,cm.ClientTypeID,ct.Name ClientTypeName,
                            cm.IsActive,cm.DateCreate,cm.UserCreate,ct.DateUpdate,ct.UserUpdate ",
                Tabel = @" FROM dbo.ClientMaster cm 
                            INNER JOIN dbo.ClientType ct ON ct.ID = cm.ClientTypeID
                            WHERE 1=1 ",
            };

            //defenisikan Where condition
            foreach (var req in dataRequest.Columns)
            {
                if (string.IsNullOrEmpty(req.SearchValue)) continue;
                else if (req.Data == "clientID") pgData.AddWhereRegex(pgData.paternAngka, req.SearchValue, "cm.ClientID");
                else if (req.Data == "clientCode") pgData.AddWhereRegex(pgData.paternAngkaHuruf, req.SearchValue, "cm.ClientCode");
                else if (req.Data == "clientTypeID") pgData.AddWhereRegex(pgData.paternAngka, req.SearchValue, "cm.ClientTypeID");
                else if (req.Data == "name") pgData.AddWhereRegex(pgData.paternAngkaHuruf, req.SearchValue, "cm.Name");
            }
            var queryString = pgData.GenerateQueryData();
            //List<ClientListVM> ClientLst = new List<ClientListVM>();

            //using (var dr =  _context.Database.ExecuteSqlCommand(queryString,ClientLst))
            //{

            //}

            //var ClientLst = _context.Database.SqlQuery<string>.FromSql(query); //.Set<ClientListVM>().FromSql(query);
            //var lst = _context.Database.ExecuteSqlCommand(query).ToList();


            //var ClientLst = _context.Database.ExecuteSqlCommand(queryString);
            //var myCustomList = _context.Database.SqlQuery<CustomType>("SELECT … FROM …");
            //var ClientLst = _context.Database.SqlQuery<ClientListVM>(queryString).ToList<ClientListVM>();

            //var myList = await _context.Database.SqlQuery<CustomType>("SELECT … FROM …")
            //                       .ToListAsync();
            var ClientLst = _context.Set<ClientListVM>().FromSql(queryString).AsNoTracking().ToList();

            //IEnumerable<ClientMasterModel> products = _context.ClientMasterModel.Skip(dataRequest.Start).Take(dataRequest.Length);
            //int recordsTotal = 2000; // _context.Tabel1Model.Count();
            ////int recordsTotal = products.Count();
            //int recordsFilterd = 1000; // recordsTotal;
            var a = 1;

            //var psd = products
            //    .Select(e => new
            //    {
            //        e.ClientID,
            //        e.ClientCode,
            //        e.Name,
            //        e.ClientTypeID,
            //        e.UserCreate,
            //        e.DateCreate,
            //        e.UserUpdate,
            //        e.DateUpdate
            //    });
            return Json(ClientLst.ToDataTablesResponse(dataRequest, pgData.recordsTotal, pgData.recordsFilterd));
        }

        // GET: ClientMaster/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }

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

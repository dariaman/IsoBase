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
using Microsoft.AspNetCore.Mvc.Rendering;
using Vereyon.Web;

namespace IsoBase.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlashMessage flashMessage;

        public ClientController(ApplicationDbContext context, IFlashMessage flash)
        {
            _context = context;
            flashMessage = flash;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListClientAll([DataTablesRequest] DataTablesRequest dataRequest)
        {
            var pgData = new PageData(dataRequest, _context)
            {
                select = @"SELECT cm.ID ClientID,cm.ClientCode,cm.Name ClientName,cm.ClientTypeID,
                            cm.IsActive,cm.UserCreate,cm.DateCreate,ct.UserUpdate,ct.DateUpdate,
                            cm.Building,cm.Address,cm.Phone,cm.Fax,cm.Email,ct.Name ClientTypeName ",
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

            List<ClientListVM> ls = new List<ClientListVM>();
            try
            {
                pgData.CountData(); // hitung jlh total data dan total dataFilter
                ls = await _context.Set<ClientListVM>().FromSql(pgData.GenerateQueryString()).ToListAsync();
            }
            catch (Exception ex)
            {
                flashMessage.Danger("Error ListClientAll : " + ex.Message);
            }

            return Json(ls.ToDataTablesResponse(dataRequest, pgData.recordsTotal, pgData.recordsFilterd));
        }

        // GET: ClientMaster/Details/5
        public async Task<IActionResult> Details(string id)
        {
            int ClientID = 0;
            ClientDetailVM cdvm = new ClientDetailVM();
            var clientMasterModels = new ClientModel();
            try
            {
                if (id == null) throw new Exception();
                if (!int.TryParse(id, out ClientID)) throw new Exception();
                if (!(ClientID > 0)) throw new Exception();
                clientMasterModels = await _context.ClientModel.FirstOrDefaultAsync(m => m.ID == ClientID);
                if (clientMasterModels == null) throw new Exception();
            }
            catch (Exception)
            {
                flashMessage.Danger("Invalid ClientID {" + id + "}");
                throw new Exception();
            }

            cdvm.ClientData = clientMasterModels;
            try
            {
                var _pic = await _context.ClientPicModel.Where(x => x.ClientID == ClientID).ToListAsync();
                var _Acc = await _context.ClientAccBankModel.Where(x => x.ClientID == ClientID).ToListAsync();
                cdvm.PicList = _pic;
                cdvm.AccBankList = _Acc;
            }
            catch (Exception ex)
            {
                flashMessage.Danger(ex.Message);
                throw new Exception();
            }
            
            return View();
        }



        // GET: ClientMaster/Create
        public IActionResult Create()
        {
            ViewBag.ClientTypeList = _context.ClientTypeModel.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.ID.ToString()
            });

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientCode,Name,ClientTypeID,Building,Address,Phone,Fax,Email")] ClientCreateVM cvm)
        {
            if (ModelState.IsValid)
            {
                ClientModel clientMasterModels = new ClientModel
                {
                    ClientCode = cvm.ClientCode,
                    Name = cvm.Name,
                    ClientTypeID = cvm.ClientTypeID,
                    Building = cvm.Building,
                    Address = cvm.Address,
                    Phone = cvm.Phone,
                    Fax = cvm.Fax,
                    Email = cvm.Email
                };
                //clientMasterModels = cvm;
                _context.Add(clientMasterModels);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ClientTypeList = _context.ClientTypeModel.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.ID.ToString()
            });

            return View(cvm);
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

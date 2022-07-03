using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LOST_AND_FOUND.Data;
using LOST_AND_FOUND.Models;
using System.Security.Claims;
using FastReport;
using Microsoft.AspNetCore.Authorization;

namespace LOST_AND_FOUND.Controllers
{
    public class FoundItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _hostEnvironment;

        public FoundItemsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return _context.FoundItem != null ?
                        View(await _context.FoundItem.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.FoundItem'  is null.");
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.FoundItem == null)
            {
                return NotFound();
            }

            var foundItem = await _context.FoundItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (foundItem == null)
            {
                return NotFound();
            }

            return View(foundItem);
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProductPicture")] FoundItem foundItem)
        {
            if (ModelState.IsValid)
            {
                foundItem.Id = Guid.NewGuid();
                foundItem.PostTime = DateTime.Now;
                foundItem.FoundBy = User.FindFirstValue(ClaimTypes.Email);

                string rootPath = _hostEnvironment.WebRootPath;
                string filename = Path.GetFileNameWithoutExtension(foundItem.ProductPicture.FileName);
                string extension = Path.GetExtension(foundItem.ProductPicture.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                foundItem.PictureName = filename;
                string path = Path.Combine(rootPath + "/image/", filename);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await foundItem.ProductPicture.CopyToAsync(fileStream);
                }

                _context.Add(foundItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(foundItem);
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.FoundItem == null)
            {
                return NotFound();
            }


            var foundItem = await _context.FoundItem.FindAsync(id);

            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if(userEmail != foundItem.FoundBy)
            {
                return RedirectToAction("NoAcess", "Home");
            }

            if (foundItem == null)
            {
                return NotFound();
            }
            return View(foundItem);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description")] FoundItem foundItem)
        {
            if (id != foundItem.Id)
            {
                return NotFound();
            }
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var foundData = await _context.FoundItem
               .FirstOrDefaultAsync(m => m.FoundBy == userEmail);

            foundItem.PostTime = DateTime.Now;
            foundItem.PictureName = foundData.PictureName;
            foundItem.FoundBy = foundData.FoundBy;



            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foundItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoundItemExists(foundItem.Id))
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
            return View(foundItem);
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.FoundItem == null)
            {
                return NotFound();
            }

            var foundItem = await _context.FoundItem
                .FirstOrDefaultAsync(m => m.Id == id);

            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userEmail != foundItem.FoundBy)
            {
                return RedirectToAction("NoAcess", "Home");
            }

            if (foundItem == null)
            {
                return NotFound();
            }

            return View(foundItem);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.FoundItem == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FoundItem'  is null.");
            }

            var foundItem = await _context.FoundItem.FindAsync(id);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userEmail != foundItem.FoundBy)
            {
                return RedirectToAction("NoAcess", "Home");
            }

            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", foundItem.PictureName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            if (foundItem != null)
            {
                _context.FoundItem.Remove(foundItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoundItemExists(Guid id)
        {
            return (_context.FoundItem?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Generate()
        {
            string rootPath = _hostEnvironment.WebRootPath;
            FastReport.Utils.Config.WebMode = true;
            Report report = new Report();
            string path = Path.Combine(rootPath + "/Report/Found.frx");
            report.Load(path);
            var foundItems = new List<FoundItemReportModel>();
            await _context.FoundItem.ForEachAsync(p =>
            {
                var item = new FoundItemReportModel
                {
                    Id = p.Id,
                    Description = p.Description,
                    FoundBy = p.FoundBy,
                    PostTime = p.PostTime,
                    Title = p.Title,
                };

                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", p.PictureName);
                if (System.IO.File.Exists(imagePath))
                {
                    item.PictureName = System.IO.File.ReadAllBytes(imagePath);
                }
                foundItems.Add(item);
            }
            );
            report.RegisterData(foundItems, "FoundItemRef");


            if (report.Report.Prepare())
            {
                FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                pdfExport.ShiftNonExportable = false;
                pdfExport.Subject = "Noyon";
                pdfExport.Title = "bhyean";
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                report.Report.Export(pdfExport, ms);
                report.Dispose();
                ms.Position = 0;
                return File(ms, "application/pdf", "repot.pdf");
            }
            else
            {
                return null;
            }

        }
    }
}

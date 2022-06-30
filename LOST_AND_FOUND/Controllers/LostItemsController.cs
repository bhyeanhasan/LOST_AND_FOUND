using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LOST_AND_FOUND.Data;
using LOST_AND_FOUND.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LOST_AND_FOUND.Controllers
{
    public class LostItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public LostItemsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: LostItems

        public async Task<IActionResult> Index()
        {
              return _context.LostItem != null ? 
                          View(await _context.LostItem.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.LostItem'  is null.");
        }

        // GET: LostItems/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.LostItem == null)
            {
                return NotFound();
            }

            var lostItem = await _context.LostItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lostItem == null)
            {
                return NotFound();
            }

            return View(lostItem);
        }

        // GET: LostItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LostItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProductPicture")] LostItem lostItem)
        {
            if (ModelState.IsValid)
            {
                lostItem.Id = Guid.NewGuid();
                lostItem.PostTime = DateTime.Now;
                lostItem.LostBy = User.FindFirstValue(ClaimTypes.Email);

                string rootPath = _hostEnvironment.WebRootPath;
                string filename = Path.GetFileNameWithoutExtension(lostItem.ProductPicture.FileName);
                string extension = Path.GetExtension(lostItem.ProductPicture.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                lostItem.PictureName = filename;
                string path = Path.Combine(rootPath + "/image/", filename);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await lostItem.ProductPicture.CopyToAsync(fileStream);
                }

                _context.Add(lostItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lostItem);
        }

        // GET: LostItems/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.LostItem == null)
            {
                return NotFound();
            }

            var lostItem = await _context.LostItem.FindAsync(id);
            if (lostItem == null)
            {
                return NotFound();
            }
            return View(lostItem);
        }

        // POST: LostItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,PostTime,LostBy")] LostItem lostItem)
        {
            if (id != lostItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lostItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LostItemExists(lostItem.Id))
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
            return View(lostItem);
        }

        // GET: LostItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.LostItem == null)
            {
                return NotFound();
            }

            var lostItem = await _context.LostItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lostItem == null)
            {
                return NotFound();
            }

            return View(lostItem);
        }

        // POST: LostItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.LostItem == null)
            {
                return Problem("Entity set 'ApplicationDbContext.LostItem'  is null.");
            }
            var lostItem = await _context.LostItem.FindAsync(id);

            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", lostItem.PictureName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            if (lostItem != null)
            {
                _context.LostItem.Remove(lostItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LostItemExists(Guid id)
        {
          return (_context.LostItem?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

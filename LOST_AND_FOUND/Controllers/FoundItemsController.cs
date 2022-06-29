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

        // GET: FoundItems
        public async Task<IActionResult> Index()
        {
              return _context.FoundItem != null ? 
                          View(await _context.FoundItem.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.FoundItem'  is null.");
        }

        // GET: FoundItems/Details/5
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

        // GET: FoundItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FoundItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,PostTime,FoundBy,ProductPicture")] FoundItem foundItem)
        {
            if (ModelState.IsValid)
            {
                foundItem.Id = Guid.NewGuid();

                string rootPath = _hostEnvironment.WebRootPath;
                string filename = Path.GetFileNameWithoutExtension(foundItem.ProductPicture.FileName);
                string extension = Path.GetExtension(foundItem.ProductPicture.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                foundItem.PictureName = filename;
                string path = Path.Combine(rootPath+"/image/", filename);

                using (var fileStream = new FileStream(path,FileMode.Create))
                {
                    await foundItem.ProductPicture.CopyToAsync(fileStream);
                }

                _context.Add(foundItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(foundItem);
        }

        // GET: FoundItems/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.FoundItem == null)
            {
                return NotFound();
            }

            var foundItem = await _context.FoundItem.FindAsync(id);
            if (foundItem == null)
            {
                return NotFound();
            }
            return View(foundItem);
        }

        // POST: FoundItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,PostTime,FoundBy")] FoundItem foundItem)
        {
            if (id != foundItem.Id)
            {
                return NotFound();
            }

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

        // GET: FoundItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
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

        // POST: FoundItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.FoundItem == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FoundItem'  is null.");
            }
            var foundItem = await _context.FoundItem.FindAsync(id);
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
    }
}

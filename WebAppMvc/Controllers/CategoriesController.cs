using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebAppMvc.Data;
using WebAppMvc.Filters;
using WebAppMvc.Models;

namespace WebAppMvc.Controllers {

    public class CategoriesController : Controller
    {
        private readonly WebAppMvcContext _context;

        public CategoriesController(WebAppMvcContext context)
        {
            _context = context;
        }


        private IQueryable<Category> ApplyFilters(
            IQueryable<Category> query, 
            string? nameFilter, 
            string? descriptionFilter) {

            if (!string.IsNullOrWhiteSpace(nameFilter)) {
                query = query.Where(x => x.Name.Contains(nameFilter));
            }

            if (!string.IsNullOrWhiteSpace(descriptionFilter)) {
                query = query.Where(x => x.Description.Contains(descriptionFilter));
            }

            return query;
        }

        private IQueryable<Category> ApplySorting(IQueryable<Category> query,
            string orderBy, string orderDirection) {

            if (orderDirection == "asc") {
                query = orderBy switch {
                    "Name" => query.OrderBy(x => x.Name),
                    "Description" => query.OrderBy(x => x.Description),
                    _ => query
                };
            } else if (orderDirection == "desc") {
                query = orderBy switch {
                    "Name" => query.OrderByDescending(x => x.Name),
                    "Description" => query.OrderByDescending(x => x.Description),
                    _ => query
                };
            }

            return query;
        }

        private async Task<(IEnumerable<Category> items, int totalPages)> ApplyPagination(
            IQueryable<Category> query,
            int page, int pageSize) {

            var count = await query.CountAsync();
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            return (data, count);
        }

        // GET: Categories
        public async Task<IActionResult> Index(
            int page = 1, 
            int pageSize = 10, 
            string? nameFilter = null, 
            string? descriptionFilter = null,
            string orderBy = "Name",
            string orderDirection = "asc"
            )
        {
            var categoriesQuery = ApplyFilters(_context.Category, nameFilter, descriptionFilter);

            categoriesQuery = ApplySorting(categoriesQuery, orderBy, orderDirection);
           
            var (data, totalCount) = await ApplyPagination(categoriesQuery, page, pageSize);

            return View(new PaginatedViewModel<Category>(
                data, page, pageSize, totalCount,
                nameFilter, descriptionFilter, orderBy, orderDirection));
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Category == null)
            {
                return Problem("Entity set 'WebAppMvcContext.Category'  is null.");
            }
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Category?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GoApp.Db;
using GoApp.Entitys;

namespace GoApp.Pages
{
    public class all_itemsModel : PageModel
    {
        private readonly GoApp.Db.SqlDbContext _context;

        public all_itemsModel(GoApp.Db.SqlDbContext context)
        {
            _context = context;
        }

        public IList<Item> Item { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Items != null)
            {
                Item = await _context.Items.ToListAsync();
            }
        }
    }
}

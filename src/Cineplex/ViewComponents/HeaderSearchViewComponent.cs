using Cineplex.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cineplex.ViewComponents
{
    public class HeaderSearchViewComponent : ViewComponent
    {
        private ApplicationDbContext _db;
        public HeaderSearchViewComponent(ApplicationDbContext context)
        {
            _db = context;
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

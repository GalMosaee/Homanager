using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Homanager.Data;
using Microsoft.AspNetCore.Mvc;
using Homanager.Models;
using Microsoft.AspNetCore.Authorization;

namespace Homanager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        [Authorize]
        public IActionResult Supermarkets()
        {
            ViewData["Message"] = "Your application description page.";
            return View(_context.Supermarket.ToList());
        }

        [Authorize]
        public IActionResult Supermarketscreator()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        [Authorize]
        public IActionResult Stats()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }
        
        public IActionResult Facebook()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

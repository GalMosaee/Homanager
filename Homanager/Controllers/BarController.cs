using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homanager.Data;
using Homanager.Models;
using Homanager.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Homanager.Controllers
{
    public class BarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BarController(ApplicationDbContext context ,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var barQuery = _context.UserGroup.Join(_context.UserGroup,
                    g => g.GroupId,
                    g => g.GroupId,
                    (a, b) => new Tuple<UserGroup, UserGroup>(a, b))
                .Where(tuple => tuple.Item1.User.Email == currentUser.Email)
                .GroupBy(t => t.Item1.Group.DateOpened.Month)
                .Select(t => new UsersGroupCountModel
                {
                    GroupOpenMonth = t.Key,
                    GroupsCount = t.Count()
                }).ToList();

            return View(barQuery);


      
        }
    }
}
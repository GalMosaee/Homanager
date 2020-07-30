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
    public class PieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PieController(ApplicationDbContext context ,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var pieQuery =_context.UserGroup.Join(_context.UserGroup,
                    g => g.GroupId,
                    g => g.GroupId,
                    (a, b) => new Tuple<UserGroup, UserGroup>(a, b))
                .Where(tuple => tuple.Item1.User.Email == currentUser.Email)
                .Where(tuple => tuple.Item2.User.Email != currentUser.Email)
                .GroupBy(t => t.Item2.User.Email)
                .Select(t => new UsersGroupCountModel
                {
                    UserEmail = t.Key,
                    GroupsCount = t.Count()
                }).ToList();


            return View(pieQuery);


      
        }
    }
}
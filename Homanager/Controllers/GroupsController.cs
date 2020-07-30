using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Homanager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Homanager.Data;
using Homanager.ViewModels;

namespace Homanager.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public GroupsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Groups
        public async Task<IActionResult> Index(string SearchString, DateTime DateOpened, Boolean OwnedByMe)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var groups = _context.Group.Include(g => g.Owner).Where(g => g.Participants.Select(i => i.UserId).Contains(user.Id) || g.Owner.Id == user.Id);
            if (!String.IsNullOrEmpty(SearchString))
            {
                groups = groups.Where(g => g.Name.Contains(SearchString));
            }
            if (DateOpened >= DateTime.MinValue)
            {
                groups = groups.Where(g => g.DateOpened >= DateOpened);
            }
            if (OwnedByMe)
            {
                groups = groups.Where(g => g.OwnerId == user.Id);
            }
	return View(await groups.ToListAsync());
        }

        // GET: Groups/Users/5
        [Route("Groups/Users/{id}")]
        public async Task<IActionResult> Users(string id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var group = await _context.Group.Include(g => g.Participants).FirstOrDefaultAsync(g => g.GroupId == id);

            foreach (var partisipate in group.Participants)
            {
                partisipate.User = _userManager.Users.FirstOrDefault(i => i.Id == partisipate.UserId);
            }
            if (group == null)
                return NotFound();
            return View(group);
        }


        // GET: Groups/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Group
                .Include(g => g.Owner)
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,Name,DateOpened,OwnerId")] Group @group)
        {
            var date = DateTime.Now;
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
                return View(@group);
            if (_context.Group.Select(i => i.Name).Contains(group.Name))
                return View(@group);
            group.DateOpened = date;
            group.Owner = user;
            group.OwnerId = user.Id;
            group.Participants = new List<Models.UserGroup>();

            var userGroup = new Models.UserGroup();
            userGroup.User = user;
            userGroup.Group = group;
            userGroup.UserId = user.Id;
            userGroup.GroupId = group.GroupId;
            group.Participants.Add(userGroup);
            if (ModelState.IsValid)
            {
                _context.Add(@group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", @group.OwnerId);
            return View(@group);
        }

        // Go to Add users for a group page
        // GET: Groups/AddUser/{id}
        [HttpGet]
        [Route("Groups/AddUser/{id}")]
        public IActionResult AddUser(string id)
        {
            var groupToAdd = _context.Group.FirstOrDefault(g => g.GroupId == id);
            if (groupToAdd == null)
            {
                return NotFound();
            }
            var users = new List<AppUser>();
            ViewData["users"] = users;
            return base.View(new ViewModels.UserGroupModel
            {
                GroupId = groupToAdd.GroupId,
                Group = groupToAdd,
                SearchUserKey = "",
                SearchResultAppUsers = new AppUser[0],
            });
        }

        // The action of adding the chosen user
        // GET: Groups/AddUser/{id}
        [Route("Groups/AddUserToGroup")]
        public async Task<IActionResult> AddUserToGroup(string groupId, string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var groupToAdd = _context.Group.Include(g => g.Participants).FirstOrDefault(g => g.GroupId == groupId);
            
            if (user == null || groupToAdd == null)
            {
                return NotFound();
            }

            if (groupToAdd.Participants == null)
            {
                groupToAdd.Participants = new List<Models.UserGroup>();
            }
            var userGroup = new Models.UserGroup();
            userGroup.User = user;
            userGroup.Group = groupToAdd;
            userGroup.UserId = user.Id;
            userGroup.GroupId = groupToAdd.GroupId;
            
            if (!groupToAdd.Participants.Select(i => i.UserId).Contains(userId))
                groupToAdd.Participants.Add(userGroup);
            _context.Update(groupToAdd);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Users), new {id = groupId});
        }

        // The action of adding the chosen user
        // GET: Groups/AddUser/{id}
        [Route("Groups/RemoveUserFromGroup")]
        public async Task<IActionResult> RemoveUserFromGroup(string groupId, string userId)
        {
            var userToRemove = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var group = _context.Group.Include(g => g.Participants).FirstOrDefault(g => g.GroupId == groupId);

            if (userToRemove == null || group == null)
            {
                return NotFound();
            }

            if (group.Participants == null)
            {
                group.Participants = new List<Models.UserGroup>();
            }

            var userGroupRemove = group.Participants.FirstOrDefault(i => i.UserId == userToRemove.Id);
            group.Participants.Remove(userGroupRemove);
            _context.Update(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users), new { id = groupId });
        }

        // Update table of users to add
        // POST: Groups/SearchUser/{userGroupModel}
        [HttpPost]
        [Route("Groups/SearchUser")]
        public IActionResult SearchUser(ViewModels.UserGroupModel userGroupModel)
        {
            var users = _userManager.Users
                .Where(u => u.UserName.Contains(userGroupModel.SearchUserKey));

            userGroupModel.SearchResultAppUsers = users.ToList();
            return View("AddUser", userGroupModel);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Group.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", @group.OwnerId);
            return View(@group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("GroupId,Name")] Group @group)
        {
            if (id != @group.GroupId)
            {
                return NotFound();
            }
            var editedGroup = await _context.Group.FindAsync(id);
            if (editedGroup == null)
                return NotFound();
            editedGroup.Name = group.Name;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(editedGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(editedGroup.GroupId))
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
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", @group.OwnerId);
            return View(@group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Group
                .Include(g => g.Owner)
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var deleteGroup = await _context.Group.FirstOrDefaultAsync(i=>i.GroupId == id);
            deleteGroup.Carts = await _context.Cart.Where(i => i.OwnerGroupId == id).ToListAsync();
            deleteGroup.Owner = await _context.Users.FindAsync(deleteGroup.OwnerId);
            if (deleteGroup == null)
                return NotFound();
            _context.Group.Remove(deleteGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(string id)
        {
            return _context.Group.Any(e => e.GroupId == id);
        }
    }
}

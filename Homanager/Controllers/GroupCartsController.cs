using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Homanager.Models;
using Homanager.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Homanager.ViewModels;

namespace Homanager.Controllers
{
    [Authorize]
    [Route("Groups/{groupId}/Carts")]
    public class GroupCartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private string _groupId;

        public GroupCartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);
            _groupId = ControllerContext.RouteData.Values["groupId"].ToString();
        }

        private async Task<Group> GetGroup()
        {
            Group @group = await _context.Group
                .Include(g => g.Carts)
                .SingleOrDefaultAsync(g => g.GroupId == _groupId);
            return @group;
        }

        // GET: Groups/{groupId}/Carts
        [Route("")]
        public async Task<IActionResult> Index(string CartName, DateTime DateOpened)
        {
            Group @group = await GetGroup();
            ViewData["GroupId"] = @group.GroupId;
            if (@group == null)
            {
                return NotFound();
            }
            var carts = _context.Cart.Where(c => c.OwnerGroupId.Equals(@group.GroupId));
            if(!String.IsNullOrEmpty(CartName))
            {
                carts = carts.Where(c => c.Name.Contains(CartName));
            }
            if (DateOpened >= DateTime.MinValue)
            {
                carts = carts.Where(c => c.DateOpened >= DateOpened);
            }
            return View(carts.ToList());
        }

        // GET: Groups/{groupId}/Carts/Details/5
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .FirstOrDefaultAsync(m => m.CartId == id);
            var group = await _context.Group.FirstOrDefaultAsync(g => g.GroupId == cart.OwnerGroupId);
            cart.OwnerGroup = group;
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Groups/{groupId}/Carts/Create
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            Group @group = await GetGroup();
            if (@group == null)
            {
                return NotFound();
            }
            return View();
        }

        // POST: Groups/{groupId}/Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create([Bind("CartId,Name,DateOpened,OwnerGroupId")] Cart cart)
        {
            Group @group = await GetGroup();
            if (@group == null)
            {
                return NotFound();
            }

            var date = DateTime.Now;

            cart.DateOpened = date;
            cart.OwnerGroupId = _groupId;
            cart.OwnerGroup = @group;

            if (ModelState.IsValid)
            {
  
                _context.Add(cart);
                await _context.SaveChangesAsync();
                var vm_cart = new CartProductsModel();
                vm_cart.CartId = cart.CartId;
                vm_cart.Cart = cart;
                vm_cart.Products = new List<CartProduct>();
                vm_cart.AddProduct = new CartProduct();
                vm_cart.ProductName = "";
                vm_cart.Quantity = 1;
                vm_cart.Comment = "";
                return RedirectToAction(nameof(Edit),"CartProducts", vm_cart);
            }
            return View(cart);
        }

        // GET: Groups/{groupId}/Carts/Edit/5
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            Group @group = await GetGroup();
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }

        // POST: Groups/{groupId}/Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CartId,Name")] Cart cart)
        {
            if (id != cart.CartId)
            {
                return NotFound();
            }

            Group @group = await GetGroup();
            if (@group == null)
            {
                return NotFound();
            }

            var editedCart = await _context.Cart.FindAsync(id);
            if (editedCart == null)
            {
                return NotFound();
            }

            editedCart.Name = cart.Name;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(editedCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartId))
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
            return View(cart);
        }

        // GET: Groups/{groupId}/Carts/Delete/5
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Groups/{groupId}/Carts/Delete/5
        [Route("Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cart = await _context.Cart.Include(i => i.CartProducts).FirstOrDefaultAsync(x => x.CartId == id);
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(string id)
        {
            return _context.Cart.Any(e => e.CartId == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Homanager.Data;
using Homanager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Homanager.ViewModels;

namespace Homanager.Controllers
{

    [Authorize]
    public class CartProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private RecommendedProduct machine;
        
        public CartProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: CartProducts
        public async Task<IActionResult> Index(string cartId)
        {
            var cart = await _context.Cart.FindAsync(cartId);
            if (cart.CartProducts == null)
                cart.CartProducts = new List<CartProduct>();
            return View(cart);
        }

        // GET: CartProducts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartProduct = await _context.CartProduct
                .Include(c => c.Cart)
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cartProduct == null)
            {
                return NotFound();
            }

            return View(cartProduct);
        }

        // GET: CartProducts/Create
        //public IActionResult Create()
        //{
        //    ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "CartId");
        //    ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductId");
        //    return View();
        //}

        // POST: CartProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("CartProducts/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductName,Quantity,Comment")] CartProduct cartProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit),cartProduct.CartId);
            }
            ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "CartId", cartProduct.CartId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductId", cartProduct.ProductId);
            return View(cartProduct);
        }

        // GET: CartProducts/Edit/5
        public async Task<IActionResult> Edit(CartProductsModel vm_cart)
        {
            if (vm_cart.CartId == null)
            {
                return NotFound();
            }

            vm_cart.Cart = await _context.Cart.FindAsync(vm_cart.CartId);
            vm_cart.Cart.CartProducts = await _context.CartProduct.Where(i => i.CartId == vm_cart.CartId).ToListAsync();
            if (vm_cart.Cart.CartProducts == null)
                vm_cart.Cart.CartProducts = new List<CartProduct>();
            vm_cart.Products = vm_cart.Cart.CartProducts;
            vm_cart.AddProduct = new CartProduct();
            vm_cart.ProductName = "";
            vm_cart.Quantity = 1;
            vm_cart.Comment = "";


            var currentCartProducts = vm_cart.Cart.CartProducts.Select(i => i.ProductName).ToList();
            
            

            ViewData["MachineLearning1"] = "";
            ViewData["MachineLearning2"] = "";
            if (MachineLearningSuggestProduct(currentCartProducts) && ViewData["Recommended"].ToString() != string.Empty)
            {
                ViewData["MachineLearning1"] = "Our algorithm found that many carts like yours wanted {0} also.";
                ViewData["MachineLearning2"] = "Would you like to add {0} to your cart?";
            }
            
            //ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "CartId", cartProduct.CartId);
            //ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductId", cartProduct.ProductId);
            return View(vm_cart);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentCartProducts"></param>
        /// <returns>true for success</returns>
        private bool MachineLearningSuggestProduct(List<string> currentCartProducts)
        {
            List<Dictionary<int, string>> cartsData = new List<Dictionary<int, string>>();
            Dictionary<int, string> dictCartProducts = new Dictionary<int, string>();
            ViewData["Recommended"] = "";

            if (currentCartProducts == null || currentCartProducts.Count == 0)
                return false;

            var carts = _context.Cart.Include(i => i.CartProducts).ToList();
            int counter = 0;

            if (carts == null && carts.Count == 0)
                return false;

            // Build the data adapted for the machine model.
            foreach (var cart in carts)
            {
                var products = new Dictionary<int, string>();
                if (cart.CartProducts == null)
                    continue;
                foreach (var product in cart.CartProducts)
                {
                    if (!products.Values.Contains(product.ProductName))
                    {
                        // add general collection id
                        products.Add(counter, product.ProductName);
                        // add current collection id
                        if (!dictCartProducts.Values.Contains(product.ProductName) && currentCartProducts.Contains(product.ProductName))
                            dictCartProducts.Add(counter, product.ProductName);
                        counter++;
                    }
                }
                cartsData.Add(products);
            }
            var word = new RecommendedProduct(cartsData).RecommendOnProduct(dictCartProducts);
            ViewData["Recommended"] = word;
            return true;
        }

        // POST: CartProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductToCart(string cartId, CartProductsModel cartProduct)
        {
            if (cartId != cartProduct.CartId)
            {
                return NotFound();
            }
            cartProduct.AddProduct.CartId = cartId;
            cartProduct.AddProduct.Cart = await _context.Cart.FindAsync(cartId);
            //cartProduct.AddProduct.Product = _context.Product.FirstOrDefaultAsync(i => i.Name == cartProduct.AddProduct.ProductName).Result;
            //cartProduct.AddProduct.ProductId = cartProduct.AddProduct.Product.ProductId;
            cartProduct.Cart = await _context.Cart.Include(i => i.CartProducts).FirstOrDefaultAsync(c => c.CartId == cartId);
            cartProduct.Cart.CartProducts.Add(cartProduct.AddProduct);
            cartProduct.AddProduct.Cart.CartProducts = cartProduct.Cart.CartProducts;
            cartProduct.Products = cartProduct.Cart.CartProducts;


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(cartProduct.AddProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartProductExists(cartProduct.CartId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var vm_cart = new CartProductsModel();
                return RedirectToAction(nameof(Edit), "CartProducts", cartProduct);
            }
            ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "CartId", cartProduct.CartId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductId", cartProduct.AddProduct.ProductId);
            return View(cartProduct);
        }

        // GET: CartProducts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartProduct = await _context.CartProduct
                .Include(c => c.Cart)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (cartProduct == null)
            {
                return NotFound();
            }

            return View(cartProduct);
        }

        // POST: CartProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cartProduct = await _context.CartProduct.FirstOrDefaultAsync(i => i.ProductId == id);
            if (cartProduct != null)
            {
                _context.CartProduct.Remove(cartProduct);
                await _context.SaveChangesAsync();
                var vm_cart = new CartProductsModel();
                vm_cart.Cart = await _context.Cart.FindAsync(cartProduct.CartId);
                vm_cart.Cart.CartProducts = await _context.CartProduct.Where(i => i.CartId == cartProduct.CartId).ToListAsync();
                if (vm_cart.Cart.CartProducts == null)
                    vm_cart.Cart.CartProducts = new List<CartProduct>();
                vm_cart.Products = vm_cart.Cart.CartProducts;
                vm_cart.AddProduct = new CartProduct();
                vm_cart.CartId = cartProduct.CartId;
                vm_cart.ProductName = "";
                vm_cart.Quantity = 1;
                vm_cart.Comment = "";
                return RedirectToAction(nameof(Edit), vm_cart);
            }
            return NotFound();
        }

        private bool CartProductExists(string id)
        {
            return _context.CartProduct.Any(e => e.CartId == id);
        }
    }
}

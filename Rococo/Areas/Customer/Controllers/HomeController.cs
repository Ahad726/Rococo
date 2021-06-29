using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using Rococo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Rococo.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var allProducts = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return View(allProducts);
        }

        public IActionResult Details(int id)
        {
            var product = _unitOfWork.Product.GetAll(x => x.Id == id, includeProperties: "Category,CoverType").FirstOrDefault();
            var newCart = new ShoppingCart
            {
                Product = product,
                ProductId = product.Id
            };
            return View(newCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCartObj)
        {
            shoppingCartObj.Id = 0;
            if (ModelState.IsValid)
            {
                // Then we add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCartObj.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetAll(

                        c => c.ApplicationUserId == shoppingCartObj.ApplicationUserId
                        && c.ProductId == shoppingCartObj.ProductId,
                        includeProperties: "Product"
                    ).FirstOrDefault();

                if (cartFromDb == null)
                {
                    // no records exits in the database for that product for that user
                    _unitOfWork.ShoppingCart.Add(shoppingCartObj);
                }
                else
                {
                    cartFromDb.Count += shoppingCartObj.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = _unitOfWork.Product.GetAll(x => x.Id == shoppingCartObj.ProductId, includeProperties: "Category,CoverType").FirstOrDefault();
                var newCart = new ShoppingCart
                {
                    Product = product,
                    ProductId = product.Id
                };
                return View(newCart);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using Rococo.Models.ViewModels;
using Rococo.Utility;
using Stripe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Rococo.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public CartController(
             UserManager<IdentityUser> userManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var shoppingCartVM = new ShoppingCartVM
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            shoppingCartVM.OrderHeader.OrderTotal = 0;
            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                                                            .GetAll(u => u.Id == claim.Value,
                                                            includeProperties: "Company")
                                                            .FirstOrDefault();

            foreach (var list in shoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count,
                    list.Product.Price, list.Product.Price50,
                    list.Product.Price100);

                shoppingCartVM.OrderHeader.OrderTotal += list.Count * list.Price;

                if (list.Product.Description != null)
                {
                    list.Product.Description = SD.ConvertToRawHtml(list.Product.Description);
                    if (list.Product.Description.Length > 100)
                    {
                        list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                    }
                }

            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            // Get User claim
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _userManager.Users.FirstOrDefault(u => u.Id == claim.Value);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email is empty!");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
               "ConfirmEmail",
                "Account",
                values: new { area = "Customer", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(new List<Message>
            {
                        new Message
                        {
                            Receiver = user.Email,
                            Subject = "Confirm your email",
                            Body =  $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>."
                        }
             });

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Plus(int cartId)
        {
            // get the cart by id including specific product with foreighKey relation
            var cart = _unitOfWork.ShoppingCart.GetAll(c => c.Id == cartId, includeProperties: "Product").FirstOrDefault();

            // increase count by 1
            cart.Count += 1;

            // update the price
            cart.Price = SD.GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                cart.Product.Price50, cart.Product.Price100);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            // get the cart by id including specific product with foreighKey relation
            var cart = _unitOfWork.ShoppingCart.GetAll(c => c.Id == cartId, includeProperties: "Product").FirstOrDefault();

            // Decrease count by 1
            cart.Count -= 1;

            if (cart.Count == 0)
            {
                // Remove the item from the cart
                return RedirectToAction(nameof(Remove),new { cartId });
            }
            else
            {
                // item count decreased. calculate new price
                cart.Price = SD.GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                cart.Product.Price50, cart.Product.Price100);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Remove(int cartId)
        {
            // get the cart by id including specific product with foreighKey relation
            var cart = _unitOfWork.ShoppingCart.GetAll(c => c.Id == cartId, includeProperties: "Product").FirstOrDefault();

            _unitOfWork.ShoppingCart.Remove(cartId);
            _unitOfWork.Save();

            // Get user's all products count and 
            var count = _unitOfWork.ShoppingCart
                .GetAll(c => c.ApplicationUserId == cart.ApplicationUserId)
                .ToList().Count();

            //save the count to session
            HttpContext.Session.SetInt32(SD.ssShoppingCartKey, count);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            // Get User claim
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCartVM = new ShoppingCartVM
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value,
                                                        includeProperties: "Product")
            };

            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                                                            .GetAll(u => u.Id == claim.Value,includeProperties:"Company")
                                                            .FirstOrDefault();
            foreach (var list in shoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count,
                    list.Product.Price, list.Product.Price50,
                    list.Product.Price100);
                shoppingCartVM.OrderHeader.OrderTotal += list.Count * list.Price;
            }

            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
            shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM shoppingCartVM, string stripeToken)
        {
            // Get User claim
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                                                            .GetAll(u => u.Id == claim.Value, includeProperties: "Company")
                                                            .FirstOrDefault();
            shoppingCartVM.ListCart = _unitOfWork.ShoppingCart
                                           .GetAll(c => c.ApplicationUserId == claim.Value,
                                           includeProperties:"Product");

            shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            shoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

            _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            foreach (var item in shoppingCartVM.ListCart)
            {
                item.Price = SD.GetPriceBasedOnQuantity(item.Count, item.Product.Price,
                                                        item.Product.Price50, item.Product.Price100);

                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = item.ProductId,
                    OrderId = shoppingCartVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                shoppingCartVM.OrderHeader.OrderTotal += orderDetails.Count * orderDetails.Price;
                _unitOfWork.OrderDetails.Add(orderDetails);
            }

            // Remove this order from shopping cart
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCartVM.ListCart);

            // save all changes that occured above
            _unitOfWork.Save();

            // update cart count in session
            HttpContext.Session.SetInt32(SD.ssShoppingCartKey, 0);

            if (string.IsNullOrEmpty(stripeToken))
            {
                // order will be created for delayed payment for authorized company
                shoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            else
            {
                // Process the instant payment  
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(shoppingCartVM.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order Id : " + shoppingCartVM.OrderHeader.Id,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                {
                    shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
                else
                {
                    shoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
                }
                if (charge.Status.ToLower() == "succeeded")
                {
                    shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
                    shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    shoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;
                }
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }

    }
}

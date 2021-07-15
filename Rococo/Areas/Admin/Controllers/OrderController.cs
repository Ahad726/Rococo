using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using Rococo.Models.ViewModels;
using Rococo.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rococo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderDetailsVM   OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Details(int id)
        {
            OrderVM = new OrderDetailsVM
            {
                OrderHeader = _unitOfWork.OrderHeader.GetAll(o => o.Id == id,
                                    includeProperties: "ApplicationUser").FirstOrDefault(),

                OrderDetails = _unitOfWork.OrderDetails.GetAll(d => d.OrderId == id,
                                                includeProperties: "Product")
            };
            return View(OrderVM);
        }


        #region API Calls
        public IActionResult GetOrderList(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderList;


            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            }
            else
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll(c => c.ApplicationUserId == claim.Value,
                                    includeProperties: "ApplicationUser");
            }


            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;

                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusApproved ||
                                                        o.OrderStatus == SD.StatusInProcess ||
                                                        o.OrderStatus == SD.StatusPending);
                    break;

                case "completed":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;

                case "rejected":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusCancelled ||
                                                       o.OrderStatus == SD.StatusRefunded ||
                                                        o.OrderStatus == SD.PaymentStatusRejected);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaderList });
        }
        #endregion
    }
}

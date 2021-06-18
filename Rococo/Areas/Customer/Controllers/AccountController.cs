using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rococo.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(ILogger<HomeController> logger, IUnitOfWork unitOfWork )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Register(string returnUrl = null)
        {
            var registerModel = new RegisterModel
            {
                ReturnUrl = returnUrl
            };

            return View(registerModel);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {


            }
            return View(model);
        }
    }
}

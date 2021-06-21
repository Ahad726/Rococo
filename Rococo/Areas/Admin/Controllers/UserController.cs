using Microsoft.AspNetCore.Mvc;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rococo.DataAccess.Data;
using Microsoft.AspNetCore.Identity;

namespace Rococo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(IUnitOfWork unitOfWork,
              UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }


        #region API Call
        [HttpGet]
        public async  Task<IActionResult> GetAllAsync()
        {
            var userList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company");
            foreach (var user in userList)
            {
                var role = await _userManager.GetRolesAsync(user);
                user.Role = role.FirstOrDefault();
                if (user.Company == null)
                {
                    user.Company = new Company
                    {
                        Name = string.Empty
                    };

                }
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string id)
        {
            var message = string.Empty;
            var user = _userManager.Users.Where(x => x.Id == id).FirstOrDefault();
            if (user == null)
            {
                return Json(new { success = false, message = "Erroe while locking unlocking user" });
            }
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                // user is currently locked. We will unloack them.
                user.LockoutEnd = DateTime.Now;               
                message = "user unlocked";
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
                message = "User locked";
            }

            // user lockEnd property modified. Update now.
            await _userManager.UpdateAsync(user);

            return Json(new { success = true, message });
        }
        #endregion
    }
}

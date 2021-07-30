using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using Rococo.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rococo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)
            {
                // this is for Create
                return View(category);
            }

            // this is for Edit
            category = await _unitOfWork.Category.GetAsync(id.GetValueOrDefault());
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    // Create new 
                    await _unitOfWork.Category.AddAsync(category);
                }
                else
                {
                    // Edit
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));

            }
            return View(category);
        }

        #region API Call
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var allCategories = await _unitOfWork.Category.GetAllAsync();
            return Json(new { data = allCategories });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Category.GetAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            await _unitOfWork.Category.RemoveAsync(category);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}

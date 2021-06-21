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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
            {
                // this is for Create
                return View(company);
            }

            // this is for Edit
            company = _unitOfWork.Company.Get(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    // Create new 
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    // Edit
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));

            }
            return View(company);
        }

        #region API Call
        [HttpGet]
        public IActionResult GetAll()
        {
            var allCompaniess = _unitOfWork.Company.GetAll();
            return Json(new { data = allCompaniess });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var company = _unitOfWork.Company.Get(id);
            if (company == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}

using Microsoft.AspNetCore.Mvc;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rococo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
            {
                // This if for create new
                return View(coverType);
            }
            //this is for edit
            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                if (coverType.Id == 0)
                {
                    // Create
                    _unitOfWork.CoverType.Add(coverType);
                    
                }
                else
                {
                    //Edit
                    _unitOfWork.CoverType.Update(coverType);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var allCoverTypes = _unitOfWork.CoverType.GetAll();
            return Json(new { data = allCoverTypes });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var coverType = _unitOfWork.CoverType.Get(id);
            if (coverType != null)
            {
                _unitOfWork.CoverType.Remove(coverType);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Delete successful" });
            }
            return Json(new { success = false, message = "Error while deleting" });
        }

        #endregion
    }
}

using Dapper;
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
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            coverType = _unitOfWork.SP_Call.OneRecode<CoverType>(SD.Proc_CoverType_Get, parameter);
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
            var parameter = new DynamicParameters();
            parameter.Add("@Name", coverType.Name);

            if (ModelState.IsValid)
            {
                if (coverType.Id == 0)
                {
                    // Create
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameter);
                    
                }
                else
                {
                    //Edit
                    parameter.Add("@Id", coverType.Id);
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameter);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var allCoverTypes = _unitOfWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll);
            return Json(new { data = allCoverTypes });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            var coverType = _unitOfWork.SP_Call.OneRecode<CoverType>(SD.Proc_CoverType_Get, parameter);
            if (coverType != null)
            {
                _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter);
                return Json(new { success = true, message = "Delete successful" });
            }
            return Json(new { success = false, message = "Error while deleting" });
        }

        #endregion
    }
}

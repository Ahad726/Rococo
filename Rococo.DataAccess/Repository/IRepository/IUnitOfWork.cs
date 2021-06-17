using System;
using System.Collections.Generic;
using System.Text;

namespace Rococo.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
         ICategoryRepository Category { get;}
        ICoverTypeRepository CoverType { get; set; }
        IProductRepository Product { get; set; }
        ICompanyRepository Company { get; set; }
        IApplicationUserRepository ApplicationUser { get; set; }
        ISP_Call SP_Call { get;}
        void Save();
    }
}

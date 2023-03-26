using Rococo.DataAccess.Data;
using Rococo.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rococo.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; set; }
        public IProductRepository Product { get; set; }
        public ICompanyRepository Company { get; set; }

        public IShoppingCartRepository ShoppingCart { get; set; }

        public IApplicationUserRepository ApplicationUser { get; set; }
        public IOrderHeaderRepository OrderHeader { get; set; }
        public IOrderDetailsRepository  OrderDetails { get; set; }
        public ISP_Call SP_Call { get; private set; }



        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Category = new CategoryRepository(_dbContext);
            SP_Call = new SP_Call(_dbContext);
            CoverType = new CoverTypeRepository(_dbContext);
            Product = new ProductRepository(_dbContext);
            Company = new CompanyRepository(_dbContext);
            ApplicationUser = new ApplicationUserRepository(_dbContext);
            ShoppingCart = new ShoppingCartRepository(_dbContext);
            OrderHeader = new OrderHeaderRepository(_dbContext);
            OrderDetails = new OrderDetailsRepository(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}

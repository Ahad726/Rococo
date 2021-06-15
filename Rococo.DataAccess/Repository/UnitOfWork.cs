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
        public ISP_Call SP_Call { get; private set; }


        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Category = new CategoryRepository(_dbContext);
            SP_Call = new SP_Call(_dbContext);
            CoverType = new CoverTypeRepository(_dbContext);
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

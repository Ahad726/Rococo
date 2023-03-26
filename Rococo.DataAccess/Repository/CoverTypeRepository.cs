using Rococo.DataAccess.Data;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Rococo.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CoverTypeRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(CoverType coverType)
        {
            var coverTypeFrmDB = _dbContext.CoverTypes.FirstOrDefault(x => x.Id == coverType.Id);

            if (coverTypeFrmDB != null)
            {
                coverTypeFrmDB.Name = coverType.Name;
            }

        }
    }
}

using Rococo.DataAccess.Data;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rococo.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ApplicationUserRepository(ApplicationDbContext dbContext)
            :base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

using Rococo.DataAccess.Data;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rococo.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderHeaderRepository(ApplicationDbContext dbContext)
            :base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(OrderHeader orderHeader)
        {
            _dbContext.Update(orderHeader);
            
        }
    }
}

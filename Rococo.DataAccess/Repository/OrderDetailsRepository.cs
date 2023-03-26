using Rococo.DataAccess.Data;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rococo.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderDetailsRepository(ApplicationDbContext dbContext)
            :base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(OrderDetails orderDetails)
        {
            _dbContext.Update(orderDetails);
        }
    }
}

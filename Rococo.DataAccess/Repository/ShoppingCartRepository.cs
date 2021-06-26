using Rococo.DataAccess.Data;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rococo.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext)
            :base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _dbContext.ShoppingCarts.Update(shoppingCart);
        }
    }
}

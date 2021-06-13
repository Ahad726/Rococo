using Rococo.DataAccess.Data;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rococo.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
            :base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Category category)
        {
            var categoryFromDb = _dbContext.Categories.FirstOrDefault(x => x.Id == category.Id);

            if (categoryFromDb != null)
                categoryFromDb.Name = category.Name;
            
        }
    }
}

using Rococo.DataAccess.Data;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rococo.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
            :base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Update(Product product)
        {
            var existingproduct = dbContext.Products.FirstOrDefault(x => x.Id == product.Id);
            if (existingproduct != null)
            {
                if (existingproduct.ImageUrl != null)
                {
                    existingproduct.ImageUrl = product.ImageUrl;
                }
                existingproduct.ISBN = product.ISBN;
                existingproduct.Price = product.Price;
                existingproduct.Price50 = product.Price50;
                existingproduct.ListPrice = product.ListPrice;
                existingproduct.Price100 = product.Price100;
                existingproduct.Title = product.Title;
                existingproduct.Description = product.Description;
                existingproduct.CategoryId = product.CategoryId;
                existingproduct.Author = product.Author;
                existingproduct.CoverTypeId = product.CoverTypeId;
               
            }

        }
    }
}

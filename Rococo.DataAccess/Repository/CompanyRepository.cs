using Rococo.DataAccess.Data;
using Rococo.DataAccess.Repository.IRepository;
using Rococo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rococo.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CompanyRepository(ApplicationDbContext dbContext)
            :base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Company company)
        {
            var companyFromDb = _dbContext.Companies.FirstOrDefault(x => x.Id == company.Id);

            if (companyFromDb != null)
            {
                companyFromDb.Name = company.Name;
                companyFromDb.StreetAddress = company.StreetAddress;
                companyFromDb.City = company.City;
                companyFromDb.State = company.State;
                companyFromDb.PostalCode = company.PostalCode;
                companyFromDb.PhoneNumber = company.PhoneNumber;
                companyFromDb.IsAuthorizedCompany = company.IsAuthorizedCompany;

            }
            
        }
    }
}

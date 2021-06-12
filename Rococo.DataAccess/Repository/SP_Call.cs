using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Rococo.DataAccess.Data;
using Rococo.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Rococo.DataAccess.Repository
{
    public class SP_Call : ISP_Call
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _connectionString = string.Empty;

        public SP_Call(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public void Execute(string ProcedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlCon = new SqlConnection(_connectionString);
            sqlCon.Open();
            sqlCon.Execute(ProcedureName, param, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<T> List<T>(string ProcedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlCon = new SqlConnection(_connectionString);
            sqlCon.Open();
            return sqlCon.Query<T>(ProcedureName, param, commandType: CommandType.StoredProcedure);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string ProcedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(_connectionString))
            {
                sqlCon.Open();
                var result = SqlMapper.QueryMultiple(sqlCon, ProcedureName, param, commandType: CommandType.StoredProcedure);
                var item1 = result.Read<T1>().ToList();
                var item2 = result.Read<T2>().ToList();

                if (item1 != null && item2 != null)
                {
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
                }
            }
            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());

        }

        public T OneRecode<T>(string ProcedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlCon = new SqlConnection(_connectionString);
            sqlCon.Open();
            var value = sqlCon.Query<T>(ProcedureName, param, commandType: CommandType.StoredProcedure);
            return (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T));
        }

        public T Single<T>(string ProcedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlCon = new SqlConnection(_connectionString);
            sqlCon.Open();
            return (T)Convert.ChangeType(sqlCon.ExecuteScalar<T>(ProcedureName, param, commandType: CommandType.StoredProcedure), typeof(T));
        }
    }
}

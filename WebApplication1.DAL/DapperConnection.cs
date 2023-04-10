using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.DAL.Interfaces;

namespace WebApplication.DAL
{
    public class DapperConnection : IDapperConnection
    {
        private readonly IConfiguration _configuration;

        public DapperConnection(IConfiguration configuration)
        {
            _configuration=configuration;
        }
        public IDbConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}

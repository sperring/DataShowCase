using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IDbManager
    {
        SqlConnection CreateSQLConnection(DBContext dbContext);
    }
}

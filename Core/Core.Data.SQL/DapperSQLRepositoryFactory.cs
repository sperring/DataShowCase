using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.SQL
{
    public class DapperSQLRepositoryFactory : IDapperSQLRepositoryFactory
    {
        private static readonly IDapperSQLRepositoryFactory instance = new DapperSQLRepositoryFactory();
        private IRepository showcase;
       

        static DapperSQLRepositoryFactory()
        {
        }

        public static IDapperSQLRepositoryFactory Instance
        {
            get
            {
                return instance;
            }
        }

        public IRepository GetInShowcaseContext()
        {
            if (showcase == null)
            {
                showcase = new DapperSQLRepository(DBContext.showcase);
            }
            return showcase;
        }

    }
}

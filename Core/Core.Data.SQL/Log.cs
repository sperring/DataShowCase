using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.SQL
{
    public class Log
    {
        public string Application { get; set; }

        public string Source { get; set; }

        public DateTime Date { get; set; }

        public int Thread { get; set; }

        public string Level { get; set; }

        public string Logger { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }

        public Guid? CorrelationId { get; set; }
    }
}

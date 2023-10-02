using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyNames
{
    internal class Contracts
    {
        public int Id { get; set; }
        public int LegalPersonId { get; set; }
        public int PersonId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime SigningDate { get; set; }

    }
}

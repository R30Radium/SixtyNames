using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyNames
{
    /// <summary>
    /// Сущность LegalPerson (Юрлицо)
    /// </summary>
    internal class LegalPerson
    {
        public int CompanyName { get; set; }
        public string Taxnumber { get; set; }
        public string Regnumber { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }
}

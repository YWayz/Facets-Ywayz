using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facets.Core.Reports.Dtos
{
    public class CollectionReportDto
    {
        public string Description { get; set; } = null!;
        public int CardPayment { get; set; }

        public decimal CardAmount { get; set; }

        public int CashPayment { get; set; }

        public decimal CashAmount { get; set; }
        public int RegisteredCount { get; set; }
        public decimal Amount { get; set; }

       
    }
}

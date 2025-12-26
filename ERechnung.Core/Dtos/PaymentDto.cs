using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERechnung.Core.Dtos
{
    public class PaymentDto
    {
        // BT-81 – Payment means code (z. B. 42 = SEPA Überweisung)
        public string PaymentMeansCode { get; set; } = "42";

        // BT-84 – Payee financial account (IBAN)
        public string Iban { get; set; } = string.Empty;

        // OPTIONAL (für spätere Erweiterung)
        // public string? Bic { get; set; }
        // public string? AccountName { get; set; }
        // public string? RemittanceInformation { get; set; }
    }
}

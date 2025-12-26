namespace ERechnung.Core.Dtos
{
    public class InvoiceDto
    {
        // BT-1
        public string InvoiceNumber { get; set; } = string.Empty;

        // BT-2
        public DateTime InvoiceDate { get; set; }

        // BT-5
        public string Currency { get; set; } = "EUR";

        // BT-20 (Pflicht wenn kein DueDate)
        public string PaymentTerms { get; set; } = string.Empty;

        // BT-106
        public decimal TotalNetAmount { get; set; }

        // BT-110
        public decimal TotalVatAmount { get; set; }

        // BT-112
        public decimal TotalGrossAmount { get; set; }

        public string BuyerReference { get; set; } = "NO-REFERENCE";
        // Zahlungsinformationen
        public PaymentDto Payment { get; set; } = new PaymentDto();
    }
}

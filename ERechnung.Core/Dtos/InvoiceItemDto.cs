namespace ERechnung.Core.Dtos
{
    public class InvoiceItemDto
    {
        // BT-153 (PFLICHT)
        public string Name { get; set; } = string.Empty;

        // BT-154
        public string Description { get; set; } = string.Empty;

        // BT-129
        public decimal Quantity { get; set; }

        // BT-130 (z.B. C62)
        public string UnitCode { get; set; } = "C62";

        // BT-146 (PFLICHT)
        public decimal UnitPriceNet { get; set; }

        // BT-131 (PFLICHT)
        public decimal LineNetAmount { get; set; }

        // BT-152
        public decimal VatRate { get; set; }
    }
}

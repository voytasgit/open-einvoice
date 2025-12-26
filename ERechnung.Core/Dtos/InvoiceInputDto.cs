namespace ERechnung.Core.Dtos
{
    public class InvoiceInputDto
    {
        public SellerDto Seller { get; set; } = new();
        public BuyerDto Buyer { get; set; } = new();
        public InvoiceDto Invoice { get; set; } = new();
        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}

namespace ERechnung.Core.Dtos
{
    public class SellerDto : PartyBaseDto
    {
        public string VatId { get; set; } = "";
        public string Phone { get; set; } = "";
    }
}

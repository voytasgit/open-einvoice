namespace ERechnung.Core.Dtos
{
    public abstract class PartyBaseDto
    {
        public string Name { get; set; } = "";
        public string Street { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string City { get; set; } = "";
        public string CountryCode { get; set; } = "";
        public string Email { get; set; } = "";
        public string EndpointId { get; set; } = "";
    }

}

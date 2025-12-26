using ERechnung.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ERechnung.Core.Services
{
    public interface IInvoiceImportService
    {
        ImportResult ImportXRechnung(Stream xmlStream);
    }

    public class InvoiceImportService : IInvoiceImportService
    {
        public ImportResult ImportXRechnung(Stream xmlStream)
        {
            var doc = XDocument.Load(xmlStream);

            XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
            XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";

            var result = new ImportResult
            {
                Invoice = new InvoiceInputDto
                {
                    Seller = ReadSeller(doc, cac, cbc),
                    Buyer = ReadBuyer(doc, cac, cbc),
                    Invoice = ReadInvoice(doc, cac, cbc),
                    Items = ReadItems(doc, cac, cbc)
                }
            };

            return result;
        }

        // ---------------- SELLER ----------------
        private SellerDto ReadSeller(XDocument doc, XNamespace cac, XNamespace cbc)
        {
            var party = doc.Root
                .Element(cac + "AccountingSupplierParty")
                ?.Element(cac + "Party");

            return new SellerDto
            {
                Name = party?
                    .Element(cac + "PartyLegalEntity")
                    ?.Element(cbc + "RegistrationName")?.Value,

                Street = party?
                    .Element(cac + "PostalAddress")
                    ?.Element(cbc + "StreetName")?.Value,

                PostalCode = party?
                    .Element(cac + "PostalAddress")
                    ?.Element(cbc + "PostalZone")?.Value,

                City = party?
                    .Element(cac + "PostalAddress")
                    ?.Element(cbc + "CityName")?.Value,

                CountryCode = party?
                    .Element(cac + "PostalAddress")
                    ?.Element(cac + "Country")
                    ?.Element(cbc + "IdentificationCode")?.Value,

                VatId = party?
                    .Element(cac + "PartyTaxScheme")
                    ?.Element(cbc + "CompanyID")?.Value,

                Email = party?
                    .Element(cac + "Contact")
                    ?.Element(cbc + "ElectronicMail")?.Value,

                Phone = party?
                    .Element(cac + "Contact")
                    ?.Element(cbc + "Telephone")?.Value,

                //EndpointId = party?
                //    .Element(cbc + "EndpointID")?.Value
                EndpointId = party?
                        .Elements(cbc + "EndpointID")
                        .Select(e => e.Value.Trim())
                        .FirstOrDefault()

            };
        }

        // ---------------- BUYER ----------------
        private BuyerDto ReadBuyer(XDocument doc, XNamespace cac, XNamespace cbc)
        {
            var party = doc.Root
                .Element(cac + "AccountingCustomerParty")
                ?.Element(cac + "Party");

            return new BuyerDto
            {
                Name = party?
                    .Element(cac + "PartyLegalEntity")
                    ?.Element(cbc + "RegistrationName")?.Value,

                Street = party?
                    .Element(cac + "PostalAddress")
                    ?.Element(cbc + "StreetName")?.Value,

                PostalCode = party?
                    .Element(cac + "PostalAddress")
                    ?.Element(cbc + "PostalZone")?.Value,

                City = party?
                    .Element(cac + "PostalAddress")
                    ?.Element(cbc + "CityName")?.Value,

                CountryCode = party?
                    .Element(cac + "PostalAddress")
                    ?.Element(cac + "Country")
                    ?.Element(cbc + "IdentificationCode")?.Value,

                Email = party?
                    .Element(cac + "Contact")
                    ?.Element(cbc + "ElectronicMail")?.Value,

                //EndpointId = party?
                //    .Element(cbc + "EndpointID")?.Value
                EndpointId = party?
                            .Elements(cbc + "EndpointID")
                            .Select(e => e.Value.Trim())
                            .FirstOrDefault()

            };
        }

        // ---------------- INVOICE ----------------
        //private InvoiceDto ReadInvoice(XDocument doc, XNamespace cac, XNamespace cbc)
        //{
        //    return new InvoiceDto
        //    {
        //        InvoiceNumber = doc.Root.Element(cbc + "ID")?.Value,
        //        InvoiceDate = DateTime.Parse(
        //            doc.Root.Element(cbc + "IssueDate")?.Value ?? DateTime.Today.ToString()
        //        ),
        //        Currency = doc.Root.Element(cbc + "DocumentCurrencyCode")?.Value,
        //        BuyerReference = doc.Root.Element(cbc + "BuyerReference")?.Value,
        //        Payment = new PaymentDto(),
        //        PaymentTerms = doc.Root
        //            .Element(cac + "PaymentTerms")
        //            ?.Element(cbc + "Note")?.Value
        //    };
        //}
        private InvoiceDto ReadInvoice(XDocument doc, XNamespace cac, XNamespace cbc)
        {
            var invoice = new InvoiceDto
            {
                InvoiceNumber = doc.Root.Element(cbc + "ID")?.Value,

                InvoiceDate = DateTime.Parse(
                    doc.Root.Element(cbc + "IssueDate")?.Value
                    ?? DateTime.Today.ToString()
                ),

                Currency = doc.Root.Element(cbc + "DocumentCurrencyCode")?.Value,

                BuyerReference = doc.Root.Element(cbc + "BuyerReference")?.Value,

                PaymentTerms = doc.Root
                    .Element(cac + "PaymentTerms")
                    ?.Element(cbc + "Note")?.Value,

                // 🔴 Payment wird gleich separat gesetzt
                Payment = new PaymentDto()
            };

            // ---------------- PAYMENT ----------------

            var paymentMeans = doc.Root.Element(cac + "PaymentMeans");
            if (paymentMeans != null)
            {
                invoice.Payment.PaymentMeansCode =
                    paymentMeans.Element(cbc + "PaymentMeansCode")?.Value;

                invoice.Payment.Iban =
                    paymentMeans
                        .Element(cac + "PayeeFinancialAccount")
                        ?.Element(cbc + "ID")
                        ?.Value;
            }

            return invoice;
        }


        // ---------------- ITEMS ----------------
        private List<InvoiceItemDto> ReadItems(
            XDocument doc, XNamespace cac, XNamespace cbc)
        {
            return doc.Root
                .Elements(cac + "InvoiceLine")
                .Select(l => new InvoiceItemDto
                {
                    Name = l.Element(cac + "Item")
                        ?.Element(cbc + "Name")?.Value,

                    Description = l.Element(cac + "Item")
                        ?.Elements(cbc + "Description")
                        .Select(e => e.Value.Trim())
                        .FirstOrDefault(),

                    Quantity = ParseDecimal(
                        l.Element(cbc + "InvoicedQuantity")?.Value ?? "1"
                    ),

                    UnitCode = l.Element(cbc + "InvoicedQuantity")
                        ?.Attribute("unitCode")?.Value ?? "C62",

                    UnitPriceNet = ParseDecimal(
                        l.Element(cac + "Price")
                            ?.Element(cbc + "PriceAmount")?.Value ?? "0"
                    ),

                    VatRate = ParseDecimal(
                        l.Element(cac + "Item")
                            ?.Element(cac + "ClassifiedTaxCategory")
                            ?.Element(cbc + "Percent")?.Value ?? "0"
                    )
                })
                .ToList();
        }
        private static decimal ParseDecimal(string? value, decimal fallback = 0m)
        {
            return decimal.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var result)
                ? result
                : fallback;
        }

    }

}

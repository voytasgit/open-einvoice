//using s2industries.ZUGFeRD;
//using s2industries.ZUGFeRD.PDF;
//using ERechnung.Core.Dtos;

//public class ZugferdGenerator
//{
//    public byte[] GeneratePdf(InvoiceInputDto dto)
//    {
//        var desc = new InvoiceDescriptor
//        {
//             Profile = Profile.XRechnung1..ToString EN16931,
//            InvoiceNo = dto.Invoice.InvoiceNumber,
//            InvoiceDate = dto.Invoice.InvoiceDate,
//            Currency = Enum.Parse<CurrencyCodes>(dto.Invoice.Currency)
//        };

//        // Seller
//        desc.SetSeller(
//            dto.Seller.Name,
//            dto.Seller.Street,
//            dto.Seller.PostalCode,
//            dto.Seller.City,
//            dto.Seller.CountryCode);

//        desc.AddSellerVATID(dto.Seller.VatId);
//        desc.AddSellerEmail(dto.Seller.Email);

//        // Buyer
//        desc.SetBuyer(
//            dto.Buyer.Name,
//            dto.Buyer.Street,
//            dto.Buyer.PostalCode,
//            dto.Buyer.City,
//            dto.Buyer.CountryCode);

//        desc.BuyerReference = dto.Invoice.BuyerReference;

//        // Payment (IBAN!)
//        if (!string.IsNullOrWhiteSpace(dto.Invoice.Payment?.Iban))
//        {
//            desc.PaymentMeans = new PaymentMeans
//            {
//                TypeCode = PaymentMeansCodes.SEPACreditTransfer,
//                PayeeFinancialAccount = new PayeeFinancialAccount
//                {
//                    ID = dto.Invoice.Payment.Iban
//                }
//            };
//        }

//        // Items
//        foreach (var it in dto.Items)
//        {
//            desc.TradeLineItems.Add(new TradeLineItem
//            {
//                Name = it.Name,
//                Description = it.Description,
//                Quantity = it.Quantity,
//                UnitCode = it.UnitCode,
//                NetUnitPrice = it.UnitPriceNet,
//                TaxPercent = it.VatRate
//            });
//        }

//        // Totals
//        desc.SetTotals(
//            dto.Invoice.TotalNetAmount,
//            dto.Invoice.TotalVatAmount,
//            dto.Invoice.TotalGrossAmount
//        );

//        using var ms = new MemoryStream();
//        new ZUGFeRDExporter().Export(desc, ms);

//        return ms.ToArray();
//    }
//}

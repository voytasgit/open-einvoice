using System.Globalization;
using System.Text;
using System.Xml;
using ERechnung.Core.Dtos;

namespace ERechnung.Core.Services
{
    public class XRechnungGenerator
    {
        public string GenerateXml(InvoiceInputDto dto)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            using var sw = new Utf8StringWriter();
            using var w = XmlWriter.Create(sw, settings);

            w.WriteStartDocument();

            w.WriteStartElement("ubl", "Invoice",
                "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
            w.WriteAttributeString("xmlns", "cac", null,
                "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            w.WriteAttributeString("xmlns", "cbc", null,
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

            E(w, "cbc:CustomizationID",
                "urn:cen.eu:en16931:2017#compliant#urn:xeinkauf.de:kosit:xrechnung_3.0");
            E(w, "cbc:ProfileID",
                "urn:fdc:peppol.eu:2017:poacc:billing:01:1.0");
            E(w, "cbc:ID", dto.Invoice.InvoiceNumber);
            E(w, "cbc:IssueDate", dto.Invoice.InvoiceDate.ToString("yyyy-MM-dd"));
            E(w, "cbc:InvoiceTypeCode", "380");
            E(w, "cbc:DocumentCurrencyCode", dto.Invoice.Currency);
            E(w, "cbc:BuyerReference", dto.Invoice.BuyerReference);

            WriteParty(w, "AccountingSupplierParty", dto.Seller, true);
            WriteParty(w, "AccountingCustomerParty", dto.Buyer, false);

            //w.WriteStartElement("cac", "PaymentMeans", null);
            //E(w, "cbc:PaymentMeansCode", "42");
            //w.WriteEndElement();
            // ---------------- PAYMENT ----------------
            w.WriteStartElement("cac", "PaymentMeans", null);

            // BT-81 Payment means code
            E(w, "cbc:PaymentMeansCode",
                dto.Invoice.Payment?.PaymentMeansCode ?? "42");

            // BT-84 Payee financial account (IBAN)
            if (!string.IsNullOrWhiteSpace(dto.Invoice.Payment?.Iban))
            {
                w.WriteStartElement("cac", "PayeeFinancialAccount", null);
                E(w, "cbc:ID", dto.Invoice.Payment.Iban);
                w.WriteEndElement(); // PayeeFinancialAccount
            }

            w.WriteEndElement(); // PaymentMeans


            w.WriteStartElement("cac", "PaymentTerms", null);
            E(w, "cbc:Note", dto.Invoice.PaymentTerms);
            w.WriteEndElement();

            WriteTax(w, dto);
            WriteTotals(w, dto);
            WriteLines(w, dto);

            w.WriteEndElement(); // Invoice
            w.WriteEndDocument();
            w.Flush();

            var xml = sw.ToString();
            if (string.IsNullOrWhiteSpace(xml))
                throw new Exception("XRechnung XML ist leer");

            return xml;
        }

        // ---------------- PARTY ----------------

        private static void WriteParty(
    XmlWriter w,
    string role,
    PartyBaseDto p,
    bool isSeller)
        {
            w.WriteStartElement("cac", role, null);
            w.WriteStartElement("cac", "Party", null);

            // 🔴 EndpointID (BT-34 / BT-49) – Pflicht
            w.WriteStartElement("cbc", "EndpointID", null);
            w.WriteAttributeString("schemeID", "EM");
            w.WriteString(p.EndpointId);
            w.WriteEndElement();

            // PartyName (Trading Name)
            w.WriteStartElement("cac", "PartyName", null);
            E(w, "cbc:Name", p.Name);
            w.WriteEndElement();

            // 🔴 PostalAddress (BT-35..38 / BT-50..53) – MUSS HIER STEHEN
            w.WriteStartElement("cac", "PostalAddress", null);
            E(w, "cbc:StreetName", p.Street);
            E(w, "cbc:CityName", p.City);
            E(w, "cbc:PostalZone", p.PostalCode);
            w.WriteStartElement("cac", "Country", null);
            E(w, "cbc:IdentificationCode", p.CountryCode);
            w.WriteEndElement(); // Country
            w.WriteEndElement(); // PostalAddress

            // VAT nur Seller
            if (isSeller && p is SellerDto s)
            {
                w.WriteStartElement("cac", "PartyTaxScheme", null);
                E(w, "cbc:CompanyID", s.VatId);
                w.WriteStartElement("cac", "TaxScheme", null);
                E(w, "cbc:ID", "VAT");
                w.WriteEndElement();
                w.WriteEndElement();
            }

            // 🔴 PartyLegalEntity (BT-27 / BT-44) – Pflicht DE
            w.WriteStartElement("cac", "PartyLegalEntity", null);
            E(w, "cbc:RegistrationName", p.Name);
            w.WriteEndElement();

            // 🔴 Contact (BT-41..43) – Pflicht DE
            w.WriteStartElement("cac", "Contact", null);
            E(w, "cbc:Name", "Rechnungswesen");

            if (isSeller && p is SellerDto s2 && !string.IsNullOrWhiteSpace(s2.Phone))
                E(w, "cbc:Telephone", s2.Phone);

            E(w, "cbc:ElectronicMail", p.Email);
            w.WriteEndElement(); // Contact

            w.WriteEndElement(); // Party
            w.WriteEndElement(); // role
        }





        // ---------------- TAX ----------------

        private static void WriteTax(XmlWriter w, InvoiceInputDto d)
        {
            w.WriteStartElement("cac", "TaxTotal", null);
            A(w, "cbc:TaxAmount", d.Invoice.TotalVatAmount, d.Invoice.Currency);

            w.WriteStartElement("cac", "TaxSubtotal", null);
            A(w, "cbc:TaxableAmount", d.Invoice.TotalNetAmount, d.Invoice.Currency);
            A(w, "cbc:TaxAmount", d.Invoice.TotalVatAmount, d.Invoice.Currency);

            w.WriteStartElement("cac", "TaxCategory", null);
            E(w, "cbc:ID", "S");
            E(w, "cbc:Percent", d.Items[0].VatRate.ToString(CultureInfo.InvariantCulture));
            w.WriteStartElement("cac", "TaxScheme", null);
            E(w, "cbc:ID", "VAT");
            w.WriteEndElement();
            w.WriteEndElement();

            w.WriteEndElement();
            w.WriteEndElement();
        }

        // ---------------- TOTALS ----------------

        private static void WriteTotals(XmlWriter w, InvoiceInputDto d)
        {
            w.WriteStartElement("cac", "LegalMonetaryTotal", null);
            A(w, "cbc:LineExtensionAmount", d.Invoice.TotalNetAmount, d.Invoice.Currency);
            A(w, "cbc:TaxExclusiveAmount", d.Invoice.TotalNetAmount, d.Invoice.Currency);
            A(w, "cbc:TaxInclusiveAmount", d.Invoice.TotalGrossAmount, d.Invoice.Currency);
            A(w, "cbc:PayableAmount", d.Invoice.TotalGrossAmount, d.Invoice.Currency);
            w.WriteEndElement();
        }

        // ---------------- LINES ----------------

        private static void WriteLines(XmlWriter w, InvoiceInputDto d)
        {
            int i = 1;
            foreach (var it in d.Items)
            {
                w.WriteStartElement("cac", "InvoiceLine", null);
                E(w, "cbc:ID", i++.ToString());

                w.WriteStartElement("cbc", "InvoicedQuantity", null);
                w.WriteAttributeString("unitCode", it.UnitCode);
                w.WriteString(it.Quantity.ToString(CultureInfo.InvariantCulture));
                w.WriteEndElement();

                A(w, "cbc:LineExtensionAmount", it.LineNetAmount, d.Invoice.Currency);

                w.WriteStartElement("cac", "Item", null);

                // ✅ Description(BT - 154) – optional, aber korrekt
                if (!string.IsNullOrWhiteSpace(it.Description))
                {
                    E(w, "cbc:Description", it.Description);
                }
                E(w, "cbc:Name", it.Name);

                w.WriteStartElement("cac", "ClassifiedTaxCategory", null);
                E(w, "cbc:ID", "S");
                E(w, "cbc:Percent", it.VatRate.ToString(CultureInfo.InvariantCulture));
                w.WriteStartElement("cac", "TaxScheme", null);
                E(w, "cbc:ID", "VAT");
                w.WriteEndElement();
                w.WriteEndElement();

                w.WriteEndElement(); // Item

                w.WriteStartElement("cac", "Price", null);
                A(w, "cbc:PriceAmount", it.UnitPriceNet, d.Invoice.Currency);
                w.WriteEndElement();

                w.WriteEndElement(); // InvoiceLine
            }
        }

        // ---------------- HELPERS ----------------

        private static void E(XmlWriter w, string name, string value)
        {
            var p = name.Split(':');
            w.WriteElementString(p[0], p[1], null, value);
        }

        private static void A(XmlWriter w, string name, decimal value, string cur)
        {
            var p = name.Split(':');
            w.WriteStartElement(p[0], p[1], null);
            w.WriteAttributeString("currencyID", cur);
            w.WriteString(value.ToString("0.00", CultureInfo.InvariantCulture));
            w.WriteEndElement();
        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}

using ERechnung.Core.Dtos;

namespace ERechnung.Core.Services
{
    public class InvoiceRecalculationService
    {
        public void Recalculate(InvoiceInputDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("Invoice must contain items");

            // -----------------
            // Positionen
            // -----------------
            foreach (var item in dto.Items)
            {
                item.LineNetAmount =
                    Math.Round(item.Quantity * item.UnitPriceNet, 2);
            }

            // -----------------
            // Summen
            // -----------------
            dto.Invoice.TotalNetAmount =
                dto.Items.Sum(i => i.LineNetAmount);

            dto.Invoice.TotalVatAmount =
                Math.Round(
                    dto.Items.Sum(i =>
                        i.LineNetAmount * (i.VatRate / 100m)
                    ),
                    2
                );

            dto.Invoice.TotalGrossAmount =
                dto.Invoice.TotalNetAmount + dto.Invoice.TotalVatAmount;
        }
    }
}

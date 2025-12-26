using ERechnung.Core.Dtos;
using ERechnung.Core.Exceptions;

public class InvoiceBusinessValidator
{
    public void Validate(InvoiceInputDto invoiceInputDto)
    {
        if (string.IsNullOrWhiteSpace(invoiceInputDto.Seller.Name))
            throw new BusinessException("Seller name is required");

        if (string.IsNullOrWhiteSpace(invoiceInputDto.Buyer.Name))
            throw new BusinessException("Buyer name is required");

        if (!invoiceInputDto.Items.Any())
            throw new BusinessException("At least one invoice line is required");

        foreach (var l in invoiceInputDto.Items)
        {
            if (l.UnitPriceNet < 0)
                throw new BusinessException("Unit price must not be negative");

            if (l.Quantity <= 0)
                throw new BusinessException("Quantity must be > 0");

            if (l.LineNetAmount != l.Quantity * l.UnitPriceNet)
                throw new BusinessException("Line net amount mismatch");
        }

        //if (invoiceInputDto.TotalNet <= 0)
        //    throw new BusinessException("Invoice net total must be > 0");

        if (string.IsNullOrWhiteSpace(invoiceInputDto.Seller.Email))
            throw new BusinessException("Seller email is required (DE rule)");

        if (string.IsNullOrWhiteSpace(invoiceInputDto.Seller.PostalCode))
            throw new BusinessException("Seller postal code is required");

        if (string.IsNullOrWhiteSpace(invoiceInputDto.Buyer.PostalCode))
            throw new BusinessException("Buyer postal code is required");
    }
}

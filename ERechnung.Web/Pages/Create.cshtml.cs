using ERechnung.Core.Dtos;
using ERechnung.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace ERechnung.Web.Pages
{
    public class CreateModel : PageModel
    {
        private readonly InvoiceRecalculationService _calc;
        private readonly XRechnungGenerator _xml;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            InvoiceRecalculationService calc,
            XRechnungGenerator xml,
            ILogger<CreateModel> logger)
        {
            _calc = calc;
            _xml = xml;
            _logger = logger;
        }

        [BindProperty]
        public InvoiceInputDto Invoice { get; set; }
        public IActionResult OnPostPreview()
        {
            // 🔒 Validierung
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _calc.Recalculate(Invoice);

            // 🔁 Invoice serialisieren
            TempData["InvoicePreview"] =
                JsonSerializer.Serialize(Invoice);

            // 👉 Weiter zur Vorschau
            return RedirectToPage("/Preview");
        }
        public void OnGet()
        {
            LoadInvoice();
        }

        public IActionResult OnPost()
        {

            _calc.Recalculate(Invoice);

            var xml = _xml.GenerateXml(Invoice);

            return File(
                Encoding.UTF8.GetBytes(xml),
                "application/xml",
                "XRechnung.xml");
        }

        private void LoadInvoice()
        {
            // 1️⃣ Import vorhanden?
            if (TempData.TryGetValue("Invoice", out var json))
            {
                Invoice = JsonSerializer.Deserialize<InvoiceInputDto>(json!.ToString()!)!;
                TempData.Keep("Invoice"); // 🔥 extrem wichtig
            }
            else
            {
                Invoice = CreateDefaultInvoice();
            }

            // 2️⃣ Defensive Initialisierung (Pflicht!)
            Invoice.Seller ??= new SellerDto();
            Invoice.Buyer ??= new BuyerDto();
            Invoice.Invoice ??= new InvoiceDto();
            Invoice.Items ??= new List<InvoiceItemDto>();

            if (!Invoice.Items.Any())
            {
                Invoice.Items.Add(new InvoiceItemDto
                {
                    UnitCode = "C62",
                    VatRate = 19
                });
            }
        }

        private static InvoiceInputDto CreateDefaultInvoice()
        {
            return new InvoiceInputDto
            {
                Seller = new SellerDto { CountryCode = "DE" },
                Buyer = new BuyerDto { CountryCode = "DE" },
                Invoice = new InvoiceDto
                {
                    Currency = "EUR",
                    InvoiceDate = DateTime.Today
                },
                Items =
            {
                new InvoiceItemDto
                {
                    UnitCode = "C62",
                    VatRate = 19
                }
            }
            };
        }
    }


}

using ERechnung.Core.Dtos;
using ERechnung.Core.Services;
using ERechnung.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

public class PreviewModel : PageModel
{
    public InvoiceInputDto Invoice { get; private set; } = new();
    private readonly InvoiceRecalculationService _calc;
    private readonly XRechnungGenerator _xml;
    private readonly ILogger<CreateModel> _logger;
    public PreviewModel(InvoiceRecalculationService calc, XRechnungGenerator xml,
            ILogger<CreateModel> logger)
    {
        _calc = calc;
        _xml = xml;
        _logger = logger;
    }
    public IActionResult OnGet()
    {


        if (!TempData.TryGetValue("InvoicePreview", out var raw))
        {
            // Sicherheitsnetz: zurück zu Create
            return RedirectToPage("/Create");
        }

        // 🔁 Zurück in Objekt
        Invoice = JsonSerializer.Deserialize<InvoiceInputDto>(
            raw!.ToString()!
        )!;

        // 🔒 Defensive Check (nur falls Create vergessen wurde)
        if (Invoice.Invoice.TotalGrossAmount <= 0
            && Invoice.Items.Any())
        {
            _calc.Recalculate(Invoice);

            // 🔁 Nur dann neu speichern
            TempData["InvoicePreview"] =
                JsonSerializer.Serialize(Invoice);
        }

        // Wichtig: für POST behalten
        TempData.Keep("InvoicePreview");

        return Page();
    }

    public IActionResult OnPostDownload(string format)
    {
        if (!TempData.TryGetValue("InvoicePreview", out var raw))
        {
            return RedirectToPage("/Create");
        }

        Invoice = JsonSerializer.Deserialize<InvoiceInputDto>(
            raw!.ToString()!
        )!;

        // ----------------------------
        // Format auswählen
        // ----------------------------
        if (format == "xrechnung")
        {
            // XML erzeugen
            // 🔒 Defensive Check (nur falls Create vergessen wurde)
            if (Invoice.Invoice.TotalGrossAmount <= 0
                && Invoice.Items.Any())
            {
                _calc.Recalculate(Invoice);

                // 🔁 Nur dann neu speichern
                TempData["InvoicePreview"] =
                    JsonSerializer.Serialize(Invoice);
            }

            var xml = _xml.GenerateXml(this.Invoice);

            return File(
                Encoding.UTF8.GetBytes(xml),
                "application/xml",
                "XRechnung.xml");
        }

        if (format == "zugferd")
        {
            // PDF erzeugen
            // return File(...)
        }

        return Page();
    }
}

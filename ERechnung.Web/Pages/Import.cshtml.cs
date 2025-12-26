using ERechnung.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace ERechnung.Web.Pages
{
    public class ImportModel : PageModel
    {
        private readonly IInvoiceImportService _import;

        public ImportModel(IInvoiceImportService import)
        {
            _import = import;
        }
        [BindProperty]
        public IFormFile Upload { get; set; }
        public IActionResult OnPost()
        {
            if (Upload == null || Upload.Length == 0)
                return Page();

            using var stream = Upload.OpenReadStream();
            var dto = _import.ImportXRechnung(stream);

            TempData["Invoice"] = JsonSerializer.Serialize(dto.Invoice);

            return RedirectToPage("/Create");
        }
    }

}

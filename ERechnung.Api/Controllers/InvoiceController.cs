using ERechnung.Core.Dtos;
using ERechnung.Core.Exceptions;
using ERechnung.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ERechnung.Api.Controllers
{
    [ApiController]
    [Route("api/invoice")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceRecalculationService _recalc;
        private readonly XRechnungGenerator _generator;
        private readonly ILogger<InvoiceController> _logger;
        private readonly InvoiceBusinessValidator _validator;

        public InvoiceController(
            InvoiceRecalculationService recalc,
            XRechnungGenerator generator,
            ILogger<InvoiceController> logger,
            InvoiceBusinessValidator validator)
        {
            _recalc = recalc;
            _generator = generator;
            _logger = logger;
            _validator = validator;
        }

        [HttpPost("generate")]
        public IActionResult Generate([FromBody] InvoiceInputDto input)
        {
            try
            {
                // Validate 
                _validator.Validate(input);
                // 1️⃣ Rechnen
                _recalc.Recalculate(input);

                // 1️⃣ XML erzeugen
                var xml = _generator.GenerateXml(input);

                if (string.IsNullOrWhiteSpace(xml))
                    return BadRequest("XRechnung XML ist leer");

                // 2️⃣ STRING → BYTES (WICHTIG!)
                var bytes = Encoding.UTF8.GetBytes(xml);

                // 3️⃣ FILE RESPONSE
                return File(
                    bytes,
                    "application/xml",
                    "XRechnung.xml"
                );
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");
                return StatusCode(500, "Internal server error");
            }

        }
    }
}

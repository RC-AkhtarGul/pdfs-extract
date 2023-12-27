using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace pdfs_extract.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly PdfExtractorService _pdfExtractorService;

        public PdfController(IHostEnvironment hostEnvironment, PdfExtractorService pdfExtractorService)
        {
            _hostEnvironment = hostEnvironment;
            _pdfExtractorService = pdfExtractorService;
        }

        [HttpGet("extractText")]
        public IActionResult ExtractTextFromPdf()
        {
            try
            {
                var wwwRootPath = _hostEnvironment.ContentRootPath; // Assuming wwwroot is at the project's root level
                var filePath = Path.Combine(wwwRootPath, "www", "Transcript.pdf");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { Error = "File not found" });
                }

                var pdfBytes = System.IO.File.ReadAllBytes(filePath);
                var extractedText = _pdfExtractorService.ExtractStudentInfoFromPdf(pdfBytes);

                return Ok(new { Text = extractedText });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}


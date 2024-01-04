using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

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
                var wwwRootPath = _hostEnvironment.ContentRootPath;
                var filePath = Path.Combine(wwwRootPath, "www", "Transcript.pdf");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { Error = "File not found" });
                }

                var pdfBytes = System.IO.File.ReadAllBytes(filePath);
                string extractedText = _pdfExtractorService.ExtractStudentInfoFromPdf(pdfBytes);

                // Create a new TranscriptRequest object and populate its properties
                TranscriptRequest transcriptRequest = new TranscriptRequest
                {
                    Name = ExtractValue(extractedText, "Name:"),
                    RegistrationNo = ExtractValue(extractedText, "Registration No:"),
                    Degree = ExtractValue(extractedText, "Degree:"),
                    Specialization = ExtractValue(extractedText, "Specialization:"),
                    DateAwarded = ExtractValue(extractedText, "Date Awarded:"),
                    SemesterDetails = ExtractCourseDetails(extractedText)
                };

                return Ok(transcriptRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        private string ExtractValue(string text, string keyword)
        {
            int startIndex = text.IndexOf(keyword);
            if (startIndex == -1)
                return null;

            startIndex += keyword.Length;
            int endIndex = text.IndexOf('\n', startIndex);

            if (endIndex == -1)
                endIndex = text.Length;

            string value = text.Substring(startIndex, endIndex - startIndex).Trim();

            // If the extracted value is empty, try to find the next non-empty line
            if (string.IsNullOrEmpty(value))
            {
                int nextLineIndex = text.IndexOf('\n', endIndex + 1);
                if (nextLineIndex != -1)
                {
                    value = text.Substring(endIndex, nextLineIndex - endIndex).Trim();
                }
            }

            return value;
        }

        private string ExtractCourseDetails(string text)
        {
            int startIndex = text.IndexOf("Course Details:");
            if (startIndex == -1)
                return null;

            startIndex = text.IndexOf('\n', startIndex) + 1;
            int endIndex = text.LastIndexOf('\n');

            if (endIndex == -1)
                endIndex = text.Length;

            return text.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}

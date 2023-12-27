using System.IO;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

public class PdfExtractorService
{
    public string ExtractStudentInfoFromPdf(byte[] pdfBytes)
    {
        using (var pdfReader = new PdfReader(new MemoryStream(pdfBytes)))
        {
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                var text = new StringWriter();
                var strategy = new LocationTextExtractionStrategy();

                for (var page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
                {
                    var currentText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);
                    text.Write(currentText);
                }

                return ExtractStudentInfo(text.ToString());
            }
        }
    }

    private string ExtractStudentInfo(string pdfText)
    {
        // Define a regular expression pattern to match the student's name and registration number
        string pattern = @"Student's Name: (?<Name>.+?) Registration No: (?<RegistrationNo>\d+)";

        // Use Regex.Match to find the match
        Match match = Regex.Match(pdfText, pattern);

        // Check if the match is successful
        if (match.Success)
        {
            // Access the captured groups using the group names
            string studentName = match.Groups["Name"].Value;
            string registrationNo = match.Groups["RegistrationNo"].Value;

            // Construct a string with the extracted information
            return $"Student's Name: {studentName}\nRegistration No: {registrationNo}";
        }
        else
        {
            return "Match not found";
        }
    }
}

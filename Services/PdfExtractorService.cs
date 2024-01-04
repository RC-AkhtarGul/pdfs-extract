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

                return ExtractTranscriptInfo(text.ToString());
            }
        }
    }
    private string ExtractTranscriptInfo(string pdfText)
    {
        // Regex to match student name and registration
        string pattern = @"Student's Name: (?<Name>.+?) Registration No: (?<RegistrationNo>\d+)";

        Match match = Regex.Match(pdfText, pattern);

        if (match.Success)
        {
            string name = match.Groups["Name"].Value;
            string registration = match.Groups["RegistrationNo"].Value;

            // New regex to match degree info
            pattern = @"Degree Awarded : (?<Degree>.*?) Specialization: (?<Specialization>.*?) Date Awarded: (?<DateAwarded>\d{2}-\w{3}-\d{4})";

            match = Regex.Match(pdfText, pattern);

            string degree = match.Groups["Degree"].Value;
            string specialization = match.Groups["Specialization"].Value;
            string dateAwarded = match.Groups["DateAwarded"].Value;

            // Extracting course details
            pattern = @"Semester\s+Year\s+Course No\s+Course Title\s+CH\s+Grade\s+SCH\s+SGP\s+SGPA\s+CCH\s+CGP\s+CGPA(.+?)Degree Requirements Completed:";
            match = Regex.Match(pdfText, pattern, RegexOptions.Singleline);

            if (match.Success)
            {
                string courseDetails = match.Groups[1].Value;

                // You can process the course details further if needed
                // For now, let's include it in the result string
                string result = $"Name: {name}\nRegistration No: {registration}\nDegree: {degree}\nSpecialization: {specialization}\nDate Awarded: {dateAwarded}\n\nCourse Details:\n{courseDetails}";

                return result;
            }
            else
            {
                return "Course details not found";
            }
        }
        else
        {
            return "Student information not found";
        }
    }


    //private string ExtractStudentInfo(string pdfText)
    //{
    //    // Regex to match student name and registration
    //    string pattern = @"Student's Name: (?<Name>.+?) Registration No: (?<RegistrationNo>\d+)";

    //    Match match = Regex.Match(pdfText, pattern);

    //    if (match.Success)
    //    {

    //        string name = match.Groups["Name"].Value;
    //        string registration = match.Groups["RegistrationNo"].Value;

    //        // New regex to match degree info
    //        pattern = @"Degree Awarded : (?<Degree>.*?) Specialization: (?<Specialization>.*?) Date Awarded: (?<DateAwarded>\d{2}-\w{3}-\d{4})";

    //        match = Regex.Match(pdfText, pattern);

    //        string degree = match.Groups["Degree"].Value;
    //        string specialization = match.Groups["Specialization"].Value;
    //        string dateAwarded = match.Groups["DateAwarded"].Value;

    //        // Construct result string
    //        return $"Name: {name}\nRegistration No: {registration}\nDegree: {degree}\nSpecialization: {specialization}\nDate Awarded: {dateAwarded}";

    //    }
    //    else
    //    {
    //        return "Match not found";
    //    }

    //}
}

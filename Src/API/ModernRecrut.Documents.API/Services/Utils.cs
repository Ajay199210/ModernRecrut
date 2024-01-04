using ModernRecrut.Documents.API.Interfaces;
using ModernRecrut.Documents.API.Entites;

namespace ModernRecrut.Documents.API.Services
{
    public class Utils : IUtils
    {
        // Required MIME Types (maybe using a dictionnary)
        private readonly List<string> requiredMIMETypes = new List<string>
        {
            "application/pdf", // Portable Document Format
            "application/msword", // Microsoft Word
            // Microsoft Word (OpenXML)
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

        public int GenererNumAleatoire()
        {
            var rand = new Random();
            return rand.Next(1, 1000);
        }

        public bool VerifyFileType(IFormFile fileData)
        {
            var fileExt = Path.GetExtension(fileData.FileName).Split('.')[1].ToLower();
            var validTypes = Enum.GetValues(typeof(FileType)).Cast<FileType>().ToList();
            var extMatch = validTypes.Any(t => t.ToString().ToLower() == fileExt); // file extension match

            return extMatch && requiredMIMETypes.Contains(fileData.ContentType);
        }
    }
}

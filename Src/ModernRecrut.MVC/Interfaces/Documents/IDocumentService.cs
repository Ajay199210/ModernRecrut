using ModernRecrut.MVC.Models.Document;

namespace ModernRecrut.MVC.Interfaces.Documents
{
    public interface IDocumentService
    {
        public Task<bool> PostFileAsync(FileUpload fileUpload);
        public Task<IEnumerable<string>> GetDocumentsByUserId(Guid IdUtilisateur);
        public Task<string> DownloadFileByName(string fileName);
    }
}

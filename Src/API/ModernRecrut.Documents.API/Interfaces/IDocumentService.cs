using ModernRecrut.Documents.API.Entites;

namespace ModernRecrut.Documents.API.Interfaces
{
    public interface IDocumentService
    {
        public Task<string> PostFileAsync(FileUpload fileUpload);
        public IEnumerable<string> GetDocumentsByUserId(Guid IdUtilisateur);
        public string DownloadFileByName(string fileName);
    }
}

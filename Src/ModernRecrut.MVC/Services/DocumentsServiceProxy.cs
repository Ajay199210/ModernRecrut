using ModernRecrut.MVC.Interfaces.Documents;
using ModernRecrut.MVC.Models.Document;
using System.Net.Http.Headers;

namespace ModernRecrut.MVC.Services
{
    public class DocumentsServiceProxy : IDocumentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DocumentsServiceProxy> _logger;
        private readonly string _documentsApiUrl = "api/documents/";

        public DocumentsServiceProxy(HttpClient httpClient, ILogger<DocumentsServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> DownloadFileByName(string fileName)
        {
            var response = await _httpClient.GetAsync(_documentsApiUrl + fileName);

            _logger.LogInformation($"[{DateTime.Now}] - [+] Téléchargement du fichier `{fileName}` en cours...");

            return await response.Content.ReadFromJsonAsync<string>();
        }

        public async Task<IEnumerable<string>> GetDocumentsByUserId(Guid IdUtilisateur)
        {
            var response = await _httpClient.GetAsync(_documentsApiUrl + IdUtilisateur);

            _logger.LogInformation($"[{DateTime.Now}] - [+] Obtention des documents pour l'utilisateur #{IdUtilisateur}");

            return await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
        }

        public async Task<bool> PostFileAsync(FileUpload fileUpload)
        {
            using (var multipartFormContent = new MultipartFormDataContent())
            {
                // Préparer le fichier (stream) et  màj l'entête du Content-Type
                var fileStreamContent = new StreamContent(fileUpload.FileData.OpenReadStream());
                fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                    fileUpload.FileData.ContentType);

                // Mettre à jour les entêtes de la requête HTTP
                multipartFormContent.Add(new StringContent(fileUpload.IdUtilisateur.ToString()),
                    name: "IdUtilisateur");
                multipartFormContent.Add(new StringContent(fileUpload.DocType.ToString()),
                    name: "DocType");

                // Ajouter le fichier
                multipartFormContent.Add(fileStreamContent, name: "filedata",
                                            fileName: fileUpload.FileData.FileName);

                // Transmettre vers l'API
                _logger.LogInformation($"[{DateTime.Now}] - [+] Téléversement du fichier `{fileUpload.FileData.FileName}` en cours...");

                var response = await _httpClient.PostAsync(_documentsApiUrl, multipartFormContent);
                return response.IsSuccessStatusCode;
                //await response.Content.ReadAsStringAsync();
            }
        }
    }
}

using ModernRecrut.Documents.API.Interfaces;
using ModernRecrut.Documents.API.Entites;

namespace ModernRecrut.Documents.API.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IUtils _utils;
        private readonly IWebHostEnvironment _webHostEnvironment;   // pour l'obtention du dossier `wwwroot`
        private readonly IHttpContextAccessor _httpContextAccessor; // pour l'obtention de l'URL base de l'API
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(IUtils utils, IWebHostEnvironment webHostEnvironment, 
            IHttpContextAccessor httpContextAccessor, ILogger<DocumentService> logger)
        {
            _utils = utils;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // Enregistrer un document
        public async Task<string> PostFileAsync(FileUpload fileUpload)
        {
            // Vérifier d'abord si le document correspond à un CV, lettre de présentaion ou un diplôme
            if(fileUpload.DocType > 0)
            {
                // Générer un nom unique pour éviter les collisions des noms des fichiers
                var fileNameElements = new List<string>
                {
                    fileUpload.IdUtilisateur.ToString(),
                    Enum.GetName(typeof(DocumentType), fileUpload.DocType),
                    _utils.GenererNumAleatoire().ToString()
                };

                // Extension du fichier à téléverser
                var fileExt = Path.GetExtension(fileUpload.FileData.FileName);

                // La location d'enregistrement du fichier
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath + "\\documents",
                    string.Join("_", fileNameElements) + fileExt );

                // Vérifier si le fichier existe
                if(!File.Exists(filePath))
                {
                    // Vérifier le type du fichier
                    var fileTypeVerified = _utils.VerifyFileType(fileUpload.FileData);

                    if(fileTypeVerified)
                    {
                        // Copier les données du fichier dans le répértoire `Documents`
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileUpload.FileData.CopyToAsync(stream);
                        }

                        _logger.LogInformation($"[{DateTime.Now}] - [+] Le fichier {fileUpload.FileData.FileName} de " +
                            $"l'utilisateur {fileUpload.IdUtilisateur} est ajouté en succès sur le serveur");

                        return filePath;
                    }
                    return "";
                }
                return "";
            }
            return "";
        }

        // Obtenir les documents d'un utilisateur
        public IEnumerable<string> GetDocumentsByUserId(Guid IdUtilisateur)
        {
            var repertoireDocs = _webHostEnvironment.WebRootPath + "\\documents"; // make it a helper private func.
            var listeDocumentsUtilisateur = new List<string>();
            var listeDocuments = Directory.GetFiles(repertoireDocs); // Tous les documents
            foreach (var document in listeDocuments)
            {
                if (document.Contains(IdUtilisateur.ToString()))
                {
                    listeDocumentsUtilisateur.Add(Path.GetFileName(document));
                }
            }

            if(listeDocuments.Any())
            {
                _logger.LogInformation($"[{DateTime.Now}] - [*] Obtention de {listeDocuments.Count()} documents " +
                    $"pour l'utilisateur {IdUtilisateur}");
            }

            return listeDocumentsUtilisateur;
        }

        public string DownloadFileByName(string fileName)
        {
            // La location d'enregistrement du fichier
            var filePath = _webHostEnvironment.WebRootPath + $"\\documents\\{fileName}";
            
            // Vérifier si le fichier existe
            if (File.Exists(filePath))
            {
                if(_httpContextAccessor.HttpContext != null)
                {
                    var request = _httpContextAccessor.HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";

                    _logger.LogInformation($"[{DateTime.Now}] - [*] Obtention du lien du téléchargement du fichier...");

                    return $"{baseUrl}/documents/{fileName}";
                }
                return "";
            }
            return "";
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ModernRecrut.Documents.API.Entites;
using ModernRecrut.Documents.API.Interfaces;

namespace ModernRecrut.Documents.API.Controllers
{
    [Route("api/documents")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _fileService;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IDocumentService fileService, ILogger<DocumentsController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>Retourne la liste des documents pour un utilisateur</summary>
        /// <remarks>
        ///     Normalement, l'utilisateur doit être existant dans la BD pour 
        ///     afficher ses documents
        /// </remarks>
        /// <param name="idUtilisateur">
        ///     L'id de l'utilisateur est requis pour afficher ses documents
        /// </param>
        /// <response code="200">La liste de tous les documents est retournée avec succès</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // GET: api/<DocumentsController>
        [HttpGet("{idUtilisateur}")]
        public ActionResult Get(Guid idUtilisateur)
        {
            var documents = _fileService.GetDocumentsByUserId(idUtilisateur);

            _logger.LogInformation($"[{DateTime.Now}] - [*] Obtention de la liste des documents pour l'utilisateur #{idUtilisateur}");

            return Ok(documents);
        }

        /// <summary>Ajouter un document</summary>
        /// <remarks>Un utilisateur ne peut ajouter deux fois le même nom d'un document</remarks>
        /// <param name="fileUpload">
        ///     Le fichier contenant les informations du fichier qui vont être enregistré dans le document
        /// </param>
        /// <response code="201">L'ajout du document dans le répértoire est effectué avec succès</response>
        /// <response code="400">Ajout non valide du document</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // POST api/<DocumentsController>/
        [HttpPost]
        public async Task<ActionResult> PostSingleFile([FromForm] FileUpload fileUpload)
        {
            if (fileUpload.FileData != null)
            {
                var uploadPath = await _fileService.PostFileAsync(fileUpload);

                _logger.LogInformation($"[{DateTime.Now}] - [*] Essaie d'ajout du document {fileUpload.FileData.FileName} " +
                    $"de l'utilisateur #{fileUpload.IdUtilisateur} sur le serveur");

                return !string.IsNullOrEmpty(uploadPath) ?
                    CreatedAtAction("PostSingleFile", fileUpload) :
                    BadRequest("Veuillez svp choisir un PDF ou un document Word " +
                                    "en choisissant le type du document");
            }
            
            _logger.LogError($"[{DateTime.Now}] - [-] Essaie d'ajout du fichier {fileUpload.FileData.FileName} de " +
                $"l'utilisateur #{fileUpload.IdUtilisateur} de type non valide");

            return BadRequest("Veuillez svp choisir un fichier valide");
        }

        /// <summary>Obtenir le lien du téléchargement d'un fichier</summary>
        /// <param name="fileName">Le nom du fichier à consulter/télécharger</param>
        /// <response code="200">Lien existe et retourné en succès</response>
        /// <response code="400">Le fichier n'existe pas</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // GET api/<DocumentsController>/documents/<fileName>
        [HttpGet]
        public ActionResult<string> getFile(string fileName)
        {
            var downloadPath = _fileService.DownloadFileByName(fileName);
            if(!string.IsNullOrEmpty(downloadPath))
            {
                _logger.LogInformation($"[{DateTime.Now}] - [+] Fichier {fileName} trouvé");

                return Ok(downloadPath);
            }
            
            _logger.LogError($"[{DateTime.Now}] - [-] Fichier {fileName} non trouvé");

            return NotFound("Svp vérifier le nom saisi du fichier");
        }
    }
}

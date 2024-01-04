using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.Interfaces.Documents;
using ModernRecrut.MVC.Models.Document;
using ModernRecrut.MVC.Models.User;

namespace ModernRecrut.MVC.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentsService;
        private readonly UserManager<Utilisateur>? _userManager;
        private ILogger<DocumentsController> _logger;

        public DocumentsController(IDocumentService documentsService, UserManager<Utilisateur> userManager,
            ILogger<DocumentsController> logger)
        {
            _documentsService = documentsService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var documents = await _documentsService.GetDocumentsByUserId(Guid.Parse(user.Id));

                _logger.LogInformation($"[{DateTime.Now}] - [+] Obtention de {documents.Count()} documents" +
                    $" pour l'utilisateur #{user.Id}");

                return View(documents);
            }
            
            _logger.LogInformation($"[{DateTime.Now}] - [-] Essaie de l'obtention des documents pour un utilisateur non authentifié");

            return NotFound();
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm] FileUpload fileUpload) 
        {
            var user = await _userManager.GetUserAsync(User);
            fileUpload.IdUtilisateur = Guid.Parse(user.Id);

            var httpOk = await _documentsService.PostFileAsync(fileUpload);
            if(!httpOk)
            {
                ModelState.AddModelError("FileData", "Seulement les documents PDF et Word sont acceptés");
                return View();
            }

            _logger.LogInformation($"[{DateTime.Now}] - [+] Le fichier `{fileUpload.FileData.FileName}` a été téléversé en succès !");
            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(string fileName)
        {
            var fichierATelecharger = await _documentsService.DownloadFileByName(fileName);

            _logger.LogInformation($"[{DateTime.Now}] - [+] Consultation/téléchargement du fichier `{fileName}`");
            
            return Redirect(fichierATelecharger);
        }
    }
}

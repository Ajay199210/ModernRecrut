using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Interfaces.Documents;
using ModernRecrut.MVC.Models.Postulations;
using ModernRecrut.MVC.Models.User;

namespace ModernRecrut.MVC.Controllers
{
    public class PostulationsController : Controller
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly IPostulationsService _postulationsService;
        private readonly IOffresEmploisService _offresEmploisService;
        private readonly IDocumentService _documentsService;
        private readonly ILogger<PostulationsController> _logger;

        public PostulationsController(UserManager<Utilisateur> userManager,
            IPostulationsService postulationsService,
            IOffresEmploisService offresEmploisService,
            IDocumentService documentsService,
            ILogger<PostulationsController> logger)
        {
            _userManager = userManager;
            _postulationsService = postulationsService;
            _offresEmploisService = offresEmploisService;
            _documentsService = documentsService;
            _logger = logger;
        }

        [Authorize(Roles = "Employe, Admin, RH")]
        public async Task<IActionResult> Index()
        {
            var postulations = await _postulationsService.ObtenirTout();

            // Pour afficher dans la vue, le nom/prénom du candidat avec le nom de l'offre d'emploi correspondant
            foreach (var postulation in postulations)
            {
                await ObtenirCandidatEtOffreEmploi(postulation);
            }

            return View(postulations);
        }

        // Note : Svp noter que la séparation entres les postulations d'un candidat
        // et tous les postulations accessible aux RH peut se faire dans un seul index.
        // C'est juste qu'on a séparé les deux pages (views) pour être plus clair
        [Authorize(Roles = "Employe, Admin, RH, Candidat")]
        public async Task<IActionResult> IndexCandidat()
        {
            var postulations = await _postulationsService.ObtenirTout();

            if (User.IsInRole("Candidat"))
            {
                var idUtilisateur = _userManager.GetUserId(User);
                postulations = postulations.Where(p => p.IdCandidat == Guid.Parse(idUtilisateur));
            }

            // Pour afficher dans la vue, le nom/prénom du candidat avec le nom de l'offre d'emploi correspondant
            foreach (var postulation in postulations)
            {
                await ObtenirCandidatEtOffreEmploi(postulation);
            }

            return View(postulations);
        }

        [Authorize(Roles = "Candidat, Admin")]
        public IActionResult Postuler()
        {
            return View();
        }

        [Authorize(Roles = "Candidat, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Postuler(RequetePostulation requetePostulation)
        { 
            var idUtilisateur = Guid.Parse(_userManager.GetUserId(User));
            // Dernier élément dans le URL contient l'id de l'offre d'emploi
            var idOffreEmploi = Guid.Parse(Request.RouteValues["id"].ToString());

            requetePostulation.IdCandidat = idUtilisateur;

            // Vérifier d'abord si le candidat a déjà postulé pour cette offre d'emploi
            var postulations = await _postulationsService.ObtenirTout();
            if (postulations.Any(p => p.IdCandidat == idUtilisateur &&
                p.IdOffreEmploi == idOffreEmploi))
            {
                ModelState.AddModelError("", "Vous avez déjà postuler pour " +
                    "cette offre d'emploi. Veuillez postuler pour une autre offre.");
                return View(requetePostulation);
            }

            var offreEmploi = await _offresEmploisService.ObtenirSelonId(idOffreEmploi);
            requetePostulation.IdOffreEmploi = idOffreEmploi;

            // Obtenir les documents de l'utilisateur
            var documents = await _documentsService.GetDocumentsByUserId(requetePostulation.IdCandidat);

            if (!documents.Any(d => d.Contains("CV")))
            {
                ModelState.AddModelError("", "Un CV est obligatoire pour postuler. " +
                    "Veuillez déposer au préalable un CV dans votre espace Documents");
                return View(requetePostulation);
            }

            if (!documents.Any(d => d.Contains("LettreDeMotivation")))
            {
                ModelState.AddModelError("", "Une lettre de motivation est obligatoire pour postuler. " +
                    "Veuillez déposer au préalable une lettre de motivation dans votre espace Documents");
                return View(requetePostulation);
            }

            if (requetePostulation.DateDisponibilite <= DateTime.Now ||
                requetePostulation.DateDisponibilite > DateTime.Now.AddDays(45))
            {
                ModelState.AddModelError("DateDisponibilite", $"La date de disponibilité doit être supérieure " +
                    $"à la date du jour et inférieure au {DateTime.Now.AddDays(45)}");
                return View(requetePostulation);
            }

            if (requetePostulation.PretentionSalariale > 150000)
            {
                ModelState.AddModelError("PretentionSalariale", "Votre prétention salariale est au-delà de nos limites");
                return View(requetePostulation);
            }

            if (ModelState.IsValid)
            {
                await _postulationsService.Postuler(requetePostulation);
                return RedirectToAction("Index", "OffresEmplois");
            }

            return View(requetePostulation);
        }

        [Authorize(Roles = "Employe, Admin, RH")]
        // GET: Postulations/Edit/{GUID}
        public async Task<IActionResult> Details(Guid id)
        {
            var postulationAConsulter = await _postulationsService.ObtenirSelonId(id);

            if (postulationAConsulter == null)
            {
                return NotFound();
            }

            await ObtenirCandidatEtOffreEmploi(postulationAConsulter);

            return View(postulationAConsulter);
        }

        [Authorize(Roles = "Admin, RH")]
        // GET: Postulations/Edit/{GUID}
        public async Task<ActionResult> Edit(Guid id)
        {
            Postulation? postulationAModifier = await _postulationsService.ObtenirSelonId(id);
            if (postulationAModifier == null)
            {
                return NotFound();
            }

            await ObtenirCandidatEtOffreEmploi(postulationAModifier);

            return View(postulationAModifier);
        }

        [Authorize(Roles = "Admin, RH")]
        // POST: Postulations/Edit/{GUID}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Postulation postulation)
        {
            Postulation? postulationAModifier = await _postulationsService.ObtenirSelonId(postulation.Id);

            if(postulationAModifier != null) 
            {
                await ObtenirCandidatEtOffreEmploi(postulationAModifier);

                if(postulationAModifier.DateDisponibilite < DateTime.Now.AddDays(-5) ||
                    postulationAModifier.DateDisponibilite > DateTime.Now.AddDays(5))
                {
                    ModelState.AddModelError("DateDisponibilite", "Il n’est pas possible de modifier " +
                        $"une postulation dont la date de disponibilité ({postulationAModifier.DateDisponibilite}) est " +
                        $"inférieure ou supérieure de 5 jours à la date du jour ({DateTime.Today})");
                }
            }

            if (postulation.PretentionSalariale > 150000)
            {
                ModelState.AddModelError("PretentionSalariale", "Votre prétention salariale est au-delà de nos limites");
            }
            
            if (postulation.DateDisponibilite.Date <= DateTime.Today ||
                postulation.DateDisponibilite.Date > DateTime.Now.AddDays(45).Date)
            {
                ModelState.AddModelError("DateDisponibilite", $"La date de disponibilité doit" +
                    $" être supérieure à la date du jour et inférieure au {DateTime.Now.AddDays(45)}");
            }
            
            if (ModelState.IsValid)
            {
                await _postulationsService.Modifier(postulation);
                return RedirectToAction("Index");
            }

            return View(postulationAModifier);
        }

        [Authorize(Roles = "Admin, RH")]
        public async Task<IActionResult> Delete(Postulation postulation)
        {
            var postulationASupprimer = await _postulationsService.ObtenirSelonId(postulation.Id);
            if(postulationASupprimer == null)
            {
                return NotFound();
            }

            await ObtenirCandidatEtOffreEmploi(postulationASupprimer);

            return View(postulationASupprimer);
        }

        [Authorize(Roles = "Admin, RH")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var postulationASupprimer = await _postulationsService.ObtenirSelonId(id);
            if (postulationASupprimer != null)
            {
                if (postulationASupprimer.DateDisponibilite < DateTime.Now.AddDays(-5) ||
                    postulationASupprimer.DateDisponibilite > DateTime.Now.AddDays(5))
                {
                    ModelState.AddModelError("DateDisponibilite", "Il n’est pas possible de supprimer " +
                        $"une postulation dont la date de disponibilité ({postulationASupprimer.DateDisponibilite}) est " +
                        $"inférieure ou supérieure de 5 jours à la date du jour ({DateTime.Today})");
                }

                await _postulationsService.Supprimer(postulationASupprimer);
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        // Méthode qui sert à assigner le candidat et l'offre d'emploi pour une postulation
        // afin de les afficher dans le vues concernées
        private async Task ObtenirCandidatEtOffreEmploi(Postulation postulation)
        {
            postulation.Candidat = await _userManager.FindByIdAsync(postulation.IdCandidat.ToString());
            postulation.OffreEmploi = await _offresEmploisService.ObtenirSelonId(postulation.IdOffreEmploi);
        }
    }
}
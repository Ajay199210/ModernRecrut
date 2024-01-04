using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.Models;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.DTO;
using Microsoft.AspNetCore.Authorization;

namespace ModernRecrut.MVC.Controllers
{
    public class OffresEmploisController : Controller
    {
        private readonly IOffresEmploisService _offresEmploisService;
        private readonly IFavorisService _favorisService;
        private readonly ILogger<OffresEmploisController> _logger;

        public OffresEmploisController(IOffresEmploisService offresEmploisService,
            ILogger<OffresEmploisController> logger,
            IFavorisService favorisService)
        {
            _offresEmploisService = offresEmploisService;
            _logger = logger;
            _favorisService = favorisService;
        }

        // GET: OffresEmploisController
        public async Task<ActionResult> Index(string? filtre, string ordreTri)
        {
            var offresEmplois = await _offresEmploisService.ObtenirTout();

            if (!string.IsNullOrEmpty(filtre))
            {
                offresEmplois = offresEmplois.Where(o => o.Poste.Contains(filtre,
                    StringComparison.InvariantCultureIgnoreCase));
            }

            ViewData["TriParPoste"] = "";
            if (string.IsNullOrEmpty(ordreTri))
            {
                ViewData["TriParPoste"] = "poste_desc";
            }

            ViewData["TriParDateAffichage"] = "dateAffichage_desc";
            if (ordreTri == "dateAffichage_desc")
            {
                ViewData["TriParDateAffichage"] = "dateAffichage";
            }
            else if (ordreTri == "dateAffichage")
            {
                ViewData["TriParDateAffichage"] = "dateAffichage_desc";
            }

            switch (ordreTri)
            {
                case "poste_desc":
                    offresEmplois = offresEmplois.OrderByDescending(o => o.Poste);
                    break;
                case "dateAffichage":
                    offresEmplois = offresEmplois.OrderBy(o => o.DateAffichage);
                    break;
                case "dateAffichage_desc":
                    offresEmplois = offresEmplois.OrderByDescending(o => o.DateAffichage);
                    break;
                default:
                    offresEmplois = offresEmplois.OrderBy(o => o.Poste);
                    break;
            }

            return View(offresEmplois);
        }

        // GET: OffresEmploisController/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            var offreEmploiAConsulter = await _offresEmploisService.ObtenirSelonId(id);
            return View(offreEmploiAConsulter);
        }

        [Authorize(Roles = "Admin")]
        // GET: OffresEmploisController/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: OffresEmploisController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RequeteOffreEmploi requeteOffreEmploi)
        {
            if(requeteOffreEmploi.DateAffichage > requeteOffreEmploi.DateFin)
            {
                ModelState.AddModelError("DateAffichage", "La date d'affichage doit être inférieur ou " +
                    "égale à la date de fin");
                return View();
            }
            
            await _offresEmploisService.Ajouter(requeteOffreEmploi);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        // GET: OffresEmploisController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            OffreEmploi offreEmploiAModifier = await _offresEmploisService.ObtenirSelonId(id);
            return View(offreEmploiAModifier);
        }

        [Authorize(Roles = "Admin")]
        // POST: OffresEmploisController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OffreEmploi offreEmploi)
        {
            if (offreEmploi.DateAffichage > offreEmploi.DateFin)
            {
                ModelState.AddModelError("DateAffichage", "La date d'affichage doit être inférieur ou " +
                    "égale à la date de fin");
                return View(offreEmploi);
            }

            await _offresEmploisService.Modifier(offreEmploi);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        // GET: OffresEmploisController/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            OffreEmploi offreEmploi = await _offresEmploisService.ObtenirSelonId(id);
            return View(offreEmploi);
        }

        [Authorize(Roles = "Admin")]
        // POST: OffresEmploisController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, OffreEmploi offreEmploi)
        {
            OffreEmploi offreEmploiASupprimer = await _offresEmploisService.ObtenirSelonId(id);
            try
            {
                await _offresEmploisService.Supprimer(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(offreEmploiASupprimer);
            }
        }
    }
}

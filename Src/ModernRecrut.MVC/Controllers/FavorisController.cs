using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Controllers
{
    public class FavorisController : Controller
    {
        private readonly IFavorisService _favorisService;
        private readonly IOffresEmploisService _offresEmploisSerivce;

        public FavorisController(IFavorisService favorisService, IOffresEmploisService offresEmploisService)
        {
            _favorisService = favorisService;
            _offresEmploisSerivce = offresEmploisService;
        }

        // GET: Favoris
        public async Task<ActionResult> Index()
        {
            //string AdresseIPClient = AdresseIpGetter.ObtenirAdresseIp();
            string? AdresseIPClient = ObtenirAdresseIp();
            if (AdresseIPClient != null)
            {
                return View(await _favorisService.ObtenirTout(AdresseIPClient));
            }

            return View(new List<OffreEmploi>());
        }

        // GET: Favoris/Details/{GUID}
        public async Task<ActionResult> Details(Guid id)
        {
            var favoriAConsulter = await _favorisService.ObtenirSelonId(id);
            return View(favoriAConsulter);
        }

        // POST: Favoris/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Guid id)
        {
            OffreEmploi offreAAjouter = await _offresEmploisSerivce.ObtenirSelonId(id);
            RequeteFavori requeteFavori = new RequeteFavori
            {
                //AdresseIPClient = AdresseIpGetter.ObtenirAdresseIp(),
                AdresseIPClient = ObtenirAdresseIp(),
                OffreEmploi = offreAAjouter
            };

            try
            {
                await _favorisService.Ajouter(requeteFavori);
                // Lors de l'ajout d'un favori, on reste sur la page des offres d'emplois
                // au lieu d'une redirection
                return RedirectToAction("Index", "OffresEmplois");
            }
            catch
            {
                return View();
            }
        }

        // GET: Favoris/Delete/{GUID}
        public async Task<ActionResult> Delete(Guid id)
        {
            OffreEmploi offreASupprimer = await _offresEmploisSerivce.ObtenirSelonId(id);
            return View(offreASupprimer);
        }

        // POST: Favoris/Delete/{GUID}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(OffreEmploi offreEmploi)
        {
            OffreEmploi offreASupprimer = await _offresEmploisSerivce.ObtenirSelonId(offreEmploi.Id);
            try
            {
                //string AdresseIPClient = AdresseIpGetter.ObtenirAdresseIp();
                string? AdresseIPClient = ObtenirAdresseIp();
                await _favorisService.Supprimer(AdresseIPClient, offreASupprimer.Id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // Obtenir l'addresse IP du client
        private string? ObtenirAdresseIp()
        {
            var remoteIPAddresse = Request.HttpContext.Connection.RemoteIpAddress;
            return remoteIPAddresse?.MapToIPv4().ToString();
        }
    }
}

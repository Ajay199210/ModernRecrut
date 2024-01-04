using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ModernRecrut.Favoris.API.Interfaces;
using ModernRecrut.Favoris.API.DTO;

namespace ModernRecrut.Favoris.API.Controllers
{
    [Route("api/favoris")]
    [ApiController]
    public class FavorisController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IFavorisService _favorisService;

        public FavorisController(IMemoryCache memoryCache, IFavorisService favorisService)
        {
            _memoryCache = memoryCache;
            _favorisService = favorisService;
        }

        /// <summary>
        /// Retourner la liste des favoris (offres d'emplois)
        /// </summary>
        /// <remarks></remarks>
        /// <param name="addresseIPClient">L'id de l'offre d'emploi qui va être supprimé de la liste des favoris</param>
        /// <response code="200">La liste des favoris (offres d'emplois) est retournée avec succès</response>
        /// <response code="400">La liste des favoris n'est pas retournée</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        // GET: api/<favoris>
        [HttpGet("{addresseIPClient}")]
        public IActionResult Get(string addresseIPClient)
        {
            var favoris = _favorisService.ObtenirTout(addresseIPClient);
            if (favoris.Count() >= 0)
            {
                return Ok(favoris);
            }
            return BadRequest("Svp vérifier que l'addresse IP saisi existe et dans le bon format");
        }

        /// <summary>
        /// Ajouter un favori (offre d'emploi)
        /// </summary>
        /// <remarks></remarks>
        /// <param name="requeteFavori">L'offre d'emploi (DTO) qui va être ajoutée</param>
        /// <response code="201">
        /// L'ajout de l'offre d'emploi dans la liste des favoris est effectué avec succès
        /// </response>
        /// <response code="400">L'ajout de l'offre d'emploi est non valide</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // POST api/<FavorisController>
        [HttpPost]
        public IActionResult Post([FromBody] RequeteFavori requeteFavori)
        {
            var offreAjoute =  _favorisService.Ajouter(requeteFavori);
            return offreAjoute ? CreatedAtAction("Post", requeteFavori) :
                BadRequest("Addresse IP non valide ou offre déjà existante pour l'IP saisie");
        }

        /// <summary>
        /// Supprimer une offre d'emploi de la liste des favoris
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="addresseIP">L'id de l'offre d'emploi qui va être supprimé de la liste des favoris</param>
        /// <param name="id">L'id de l'offre d'emploi qui va être supprimé de la liste des favoris</param>
        /// <response code="204">Suppression du favoris (offre d'emploi) effectué avec succès</response>
        /// <response code="400">Erreur lors de la suppression. Informations mal saisies</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        ///
        // DELETE api/<FavorisController>/{GUID}
        [HttpDelete("{addresseIP}/{id}")]
        public IActionResult Delete(string addresseIP, Guid id)
        {
            if(_favorisService.Supprimer(addresseIP, id))
            {
                return NoContent();
            }
            return BadRequest("Svp vérifier l'informations saisies");
        }
    }
}
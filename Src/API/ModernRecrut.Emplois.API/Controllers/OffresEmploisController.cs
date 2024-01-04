using Microsoft.AspNetCore.Mvc;
using ModernRecrut.Emplois.API.Interfaces;
using ModernRecrut.Emplois.API.Entites;
using ModernRecrut.Emplois.API.DTO;

namespace ModernRecrut.Emplois.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffresEmploisController : ControllerBase
    {
        private readonly IOffresEmploisService _offresEmploisService;

        public OffresEmploisController(IOffresEmploisService offresEmploisService)
        {
            _offresEmploisService = offresEmploisService;
        }

        /// <summary>
        /// Retourne la liste des offres d'emplois
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">La liste de tous les offres d'emplois est retournée avec succès</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // GET: api/<OffresEmploisController>
        [HttpGet]
        public async Task<IEnumerable<OffreEmploi>> Get()
        {
            return await _offresEmploisService.ObtenirTout();
        }

        /// <summary>
        /// Retourner une offre d'emploi spécifique à partir de l'id
        /// </summary>
        /// <remarks>
        ///     Cette méthode pourra être utile pour chercher une offre d'emploi spécifique afin de vérifier 
        ///     si ce dernier existe ou non.
        /// </remarks>
        /// <param name="id">Id de l'offre d'emploi à retourner</param>
        /// <response code="200">Offre emploi trouvé et retourné</response>
        /// <response code="404">L'offre d'emploi est introuvable pour l'id spécifié</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // GET api/<OffresEmploisController>/{GUID}
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            OffreEmploi? offreEmploiARecherche = await _offresEmploisService.ObtenirSelonId(id);
            if (offreEmploiARecherche == null)
            {
                return NotFound("Offre emploi n'existe pas pour l'identifiant saisi.");
            }
            return Ok(offreEmploiARecherche);
        }

        /// <summary>
        /// Ajouter une offre d'emploi
        /// </summary>
        /// <remarks></remarks>
        /// <param name="requeteOffreEmploi">L'offre d'emploi qui va être ajoutée</param>
        /// <response code="201">L'ajout de l'offre d'emploi est effectué avec succès</response>
        /// <response code="400">L'ajout de l'offre d'emploi est non valide</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // POST api/<OffresEmploisController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] RequeteOffreEmploi requeteOffreEmploi)
        {
            if(requeteOffreEmploi != null)
            {
                await _offresEmploisService.Ajouter(requeteOffreEmploi);
                return CreatedAtAction("Post", requeteOffreEmploi);
            }
            return BadRequest();
        }

        /// <summary>
        /// Modifier une offre d'emploi
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="offreEmploi">L'offre d'emploi en cours de modification</param>
        /// <response code="204">Mise à jour effectuée avec succès</response>
        /// <response code="404">Offre d'emploi non trouvé</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        ///
        // PUT api/<OffresEmploisController>/{GUID}
        [HttpPut("{offreEmploi.Id}")]
        public async Task<ActionResult> Put([FromBody] OffreEmploi offreEmploi)
        {
            if (offreEmploi == null)
            {
                return NotFound("L'offre d'emploi n'existe pas pour l'identifiant saisi");
            }

            await _offresEmploisService.Modifier(offreEmploi);
            return NoContent(); // HTTP 204
        }

        /// <summary>
        /// Supprimer une offre d'emploi
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="id">L'id de l'offre d'emploi qui va être supprimé</param>
        /// <response code="204">Suppression de l'offre d'emploi effectué avec succès</response>
        /// <response code="404">L'offre d'emploi est introuvable</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        ///
        // DELETE api/<OffresEmploisController>/{GUID}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            bool offreEstSupprime = await _offresEmploisService.Supprimer(id);
            return offreEstSupprime ? NoContent() : NotFound("Offre emploi non trouvé");
        }
    }
}

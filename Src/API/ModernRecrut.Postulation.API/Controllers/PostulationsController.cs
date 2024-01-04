using Microsoft.AspNetCore.Mvc;
using ModernRecrut.Postulations.API.DTO;
using ModernRecrut.Postulations.API.Interfaces;
using ModernRecrut.Postulations.API.Models;

namespace ModernRecrut.Postulations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostulationsController : ControllerBase
    {
        private readonly IPostulationsService _postulationsService;

        public PostulationsController(IPostulationsService postulationsService)
        {
            _postulationsService = postulationsService;
        }

        /// <summary>Retourne la liste des postulations</summary>
        /// <response code="200">La liste de tous les postulations est retournée avec succès</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // GET: api/Postulations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Postulation>>> Get()
        {
            var postulations = await _postulationsService.ObtenirTout();

            return Ok(postulations);
        }

        /// <summary>Retourner une postulation spécifique à partir de l'id</summary>
        /// <param name="id">Id de la postulation à retourner</param>
        /// <response code="200">Postulation trouvée et retournée</response>
        /// <response code="404">La postulation est introuvable pour l'id spécifié</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // GET: api/Postulations/{GUID}
        [HttpGet("{id}")]
        public async Task<ActionResult<Postulation>> Get(Guid id)
        {
            var postulationAObtenir = await _postulationsService.ObtenirSelonId(id);
            if (postulationAObtenir == null)
            {
                return NotFound("Veuillez svp saisir un id existant pour voir la postulation");
            }
            return Ok(postulationAObtenir);
        }

        /// <summary>Ajouter une postulation</summary>
        /// <param name="requetePostulation">La postulation (DTO) qui va être ajoutée</param>
        /// <response code="201">L'ajout de la postulation est effectué avec succès</response>
        /// <response code="400">L'ajout de la postulation est non valide</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // POST: api/Postulations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Postulation>> Post([FromBody] RequetePostulation requetePostulation)
        {
            if (ModelState.IsValid)
            {
                Postulation? postulation = await _postulationsService.Postuler(requetePostulation);

                if (postulation == null)
                {
                    return BadRequest();
                }
                return CreatedAtAction("Post", new { id = postulation?.Id }, postulation);
            }
            return BadRequest();
        }

        /// <summary>Modifier une postulation</summary>
        /// <param name="postulation">La postulation à modifier</param>
        /// <response code="204">Mise à jour effectuée avec succès</response>
        /// <response code="404">Postulation non trouvé</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        ///
        // PUT: api/Postulations/{GUID}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Postulation postulation)
        {
            bool postulationModifiee = await _postulationsService.Modifier(postulation);
            return postulationModifiee ? NoContent() : BadRequest("Vérifier que la postulation existe" +
                "et que la date de disponibilité est supérieur et inférieure de 5 jours à la date du jour");
        }

        /// <summary>Supprimer une offre d'emploi</summary>
        /// <param name="id">L'id de la postulation qui va être supprimée</param>
        /// <response code="204">Suppression de la postulation effectuée avec succès</response>
        /// <response code="404">La postulation est introuvable</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        ///
        // DELETE: api/Postulations/{GUID}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(Guid id)
        {
            var postulationEstSupprimee = await _postulationsService.Supprimer(id);
            return postulationEstSupprimee ? NoContent() : NotFound("Postulation non trouvée");
        }
    }
}

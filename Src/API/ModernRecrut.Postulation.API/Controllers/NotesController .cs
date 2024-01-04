using Microsoft.AspNetCore.Mvc;
using ModernRecrut.Postulations.API.Interfaces;
using ModernRecrut.Postulations.API.Models;
using ModernRecrut.Postulations.API.DTO;

namespace ModernRecrut.Postulations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesService _notesService;

        public NotesController(INotesService notesSerivce)
        {
            _notesService = notesSerivce;
        }

        /// <summary>Ajouter une note</summary>
        /// <param name="requeteNote">La note qui va être ajoutée</param>
        /// <response code="201">L'ajout de la note est effectué avec succès</response>
        /// <response code="400">L'ajout de la note est non valide</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // POST: api/Notes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] RequeteNote requeteNote)
        {
            if (ModelState.IsValid)
            {
                var note = await _notesService.Ajouter(requeteNote);

                if (note != null)
                {
                    return CreatedAtAction("Post", new { id = note?.Id }, note);
                }
                return BadRequest("Veuillez svp vérifier que la postulation existe");
            }
            return BadRequest("Veuillez svp vérifier que la postulation existe");
        }

        /// <summary>Retourner une note spécifique à partir de l'id</summary>
        /// <param name="id">Id de la note</param>
        /// <response code="200">note trouvée et retournée</response>
        /// <response code="404">La note est introuvable pour l'id spécifié</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        ///
        // GET: api/Notes/{GUID}
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> Get(Guid id)
        {
            var noteAObtenir = await _notesService.ObtenirSelonId(id);
            if (noteAObtenir == null)
            {
                return NotFound("Veuillez svp saisir un id existant pour voir la note");
            }
            return Ok(noteAObtenir);
        }

        /// <summary>Retourne la liste des notes</summary>
        /// <response code="200">La liste de tous les note est retournée avec succès</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        /// 
        // GET: api/Notes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> Get()
        {
            var notes = await _notesService.ObtenirTout();

            return Ok(notes);
        }

        /// <summary>Modifier une note</summary>
        /// <param name="note">La note à modifier</param>
        /// <response code="204">Mise à jour effectuée avec succès</response>
        /// <response code="404">Note non trouvée</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        ///
        // PUT: api/Notes/{GUID}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Note note)
        {
            if(id != note.Id)
            {
                return BadRequest("Veuillez svp vérifier que l'id de la note est correcte");
            }
            bool noteModifiee = await _notesService.Modifier(note);
            return noteModifiee ? NoContent() : BadRequest("Veuillez svp vérifier que la note existe " +
                "pour la postulation correspondante");
        }

        /// <summary>Supprimer une note</summary>
        /// <param name="id">L'id de la note qui va être supprimée</param>
        /// <response code="204">Suppression de la note effectuée avec succès</response>
        /// <response code="404">La note est introuvable</response>
        /// <response code="500">Oops! le service est indisponible pour le moment</response>
        ///
        // DELETE: api/Notes/{GUID}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var noteASupprimee = await _notesService.Supprimer(id);
            return noteASupprimee ? NoContent() : NotFound("Note non trouvée");
        }
    }
}

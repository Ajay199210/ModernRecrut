using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models.Postulations;

namespace ModernRecrut.MVC.Controllers
{
    public class NotesController : Controller
    {
        private readonly INotesService _notesService;

        public NotesController(INotesService notesService)
        {
            _notesService = notesService;
        }

        [Authorize(Roles = "Admin, RH")]
        // GET: Notes/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin, RH")]
        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RequeteNote requeteNote)
        {
            if (ModelState.IsValid)
            {
                requeteNote.IdPostulation = Guid.Parse(Request.RouteValues["id"].ToString());
                requeteNote.NomEmetteur = User.Identity.Name;
                await _notesService.Ajouter(requeteNote);
                return RedirectToAction("Details", "Postulations", new { id = requeteNote.IdPostulation });
            }
            return View();
        }

        [Authorize(Roles = "Admin, RH")]
        // GET: Notes/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var noteAModifier = await _notesService.ObtenirSelonId(id);
            return View(noteAModifier);
        }

        [Authorize(Roles = "Admin, RH")]
        // POST: Notes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Note note)
        {
            var noteAModifier = await _notesService.ObtenirSelonId(note.Id);
            await _notesService.Modifier(note);
            return RedirectToAction("Details", "Postulations", new { Id = noteAModifier.IdPostulation });
        }

        [Authorize(Roles = "Admin, RH")]
        // GET: Notes/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            var noteASupprimer = await _notesService.ObtenirSelonId(id);
            return View(noteASupprimer);
        }

        [Authorize(Roles = "Admin, RH")]
        // POST: Notes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Note  note)
        {
            var noteASupprimer = await _notesService.ObtenirSelonId(note.Id);
            try
            {
                await _notesService.Supprimer(note.Id);
                return RedirectToAction("Details", "Postulations", new { Id = noteASupprimer.IdPostulation });
            }
            catch
            {
                return View(noteASupprimer);
            }
        }
    }
}

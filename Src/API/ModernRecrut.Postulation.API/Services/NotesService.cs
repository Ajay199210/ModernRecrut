using Microsoft.AspNetCore.Mvc;
using ModernRecrut.Postulations.API.Data;
using ModernRecrut.Postulations.API.DTO;
using ModernRecrut.Postulations.API.Interfaces;
using ModernRecrut.Postulations.API.Models;

namespace ModernRecrut.Postulations.API.Services
{
    public class NotesService : INotesService
    {
        private readonly IPostulationsService _postulationsService;
        private readonly PostulationContext _context;

        public NotesService(IPostulationsService postulationsService, PostulationContext context)
        {
            _postulationsService = postulationsService;
            _context = context;
        }

        public async Task<Note> Ajouter(RequeteNote requeteNote)
        {
            var postulations = await _postulationsService.ObtenirTout();
            var postulationAModifier = postulations.FirstOrDefault(p => p.Id == requeteNote.IdPostulation);
            if (postulationAModifier != null)
            {
                Note note = new()
                {
                    IdPostulation = requeteNote.IdPostulation,
                    Contenu = requeteNote.Contenu,
                    NomEmetteur = requeteNote.NomEmetteur
                };
                postulationAModifier.Notes.Add(note);

                _context.Update(postulationAModifier);
                await _context.SaveChangesAsync();
                return note;
            }
            return null;
        }

        public async Task<Note?> ObtenirSelonId(Guid id)
        {
            var notes = await ObtenirTout();
            return notes.FirstOrDefault(n => n.Id == id);
        }

        public async Task<IEnumerable<Note>> ObtenirTout()
        {
            List<Note> tousLesNotes = new List<Note>();
            var postulations = await _postulationsService.ObtenirTout();
            postulations.ToList().ForEach(p => p.Notes.ToList().ForEach(n => tousLesNotes.Add(n)));
            return tousLesNotes;
        }

        public async Task<bool> Modifier([FromBody] Note note)
        {
            var noteEstModifiee = false;

            var noteAModifier = await ObtenirSelonId(note.Id);
            var postulations = await _postulationsService.ObtenirTout();
            
            Postulation postulationAModifier;

            foreach (var postulation in postulations)
            {
                foreach (var item in postulation.Notes)
                {
                    if(item.Id == note.Id)
                    {
                        postulationAModifier = postulation;
                        if (noteAModifier != null)
                        {
                            noteAModifier.Contenu = note.Contenu;
                            noteAModifier.Postulation = postulationAModifier;
                            _context.Update(postulationAModifier);
                            await _context.SaveChangesAsync();
                            noteEstModifiee = true;
                        }
                    }
                }
            }
            return noteEstModifiee;
        }

        public async Task<bool> Supprimer(Guid id)
        {
            var postulations = await _postulationsService.ObtenirTout();
            bool noteSupprimee = false;

            foreach (var postulation in postulations)
            {
                foreach (var note in postulation.Notes)
                {
                    if(note.Id == id)
                    {
                        postulation.Notes.Remove(note);
                        _context.Update(postulation);
                        await _context.SaveChangesAsync();
                        noteSupprimee = true;
                    }
                }
            }
            return noteSupprimee;
        }
    }
}

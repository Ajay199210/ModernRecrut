using ModernRecrut.Postulations.API.DTO;
using ModernRecrut.Postulations.API.Models;

namespace ModernRecrut.Postulations.API.Interfaces
{
    public interface INotesService
    {
        public Task<Note> Ajouter(RequeteNote requeteNote);
        public Task<Note?> ObtenirSelonId(Guid id);
        public Task<IEnumerable<Note>> ObtenirTout();
        public Task<bool> Modifier(Note note);
        public Task<bool> Supprimer(Guid id);
    }
}

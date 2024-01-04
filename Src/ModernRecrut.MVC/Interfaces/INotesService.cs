using ModernRecrut.MVC.Models.Postulations;
using ModernRecrut.MVC.DTO;

namespace ModernRecrut.MVC.Interfaces
{
    public interface INotesService
    {
        public Task<bool> Ajouter(RequeteNote requeteNote);
        public Task<Note?> ObtenirSelonId(Guid id);
        public Task<IEnumerable<Note>> ObtenirTout();
        public Task<bool> Modifier(Note note);
        public Task<bool> Supprimer(Guid id);
    }
}

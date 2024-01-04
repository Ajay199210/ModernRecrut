using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Interfaces
{
    public interface IOffresEmploisService
    {
        public Task Ajouter(RequeteOffreEmploi requeteOffreEmploi);
        public Task<OffreEmploi> ObtenirSelonId(Guid id);
        public Task<IEnumerable<OffreEmploi>> ObtenirTout();
        public Task Modifier(OffreEmploi item);
        public Task Supprimer(Guid id);
    }
}

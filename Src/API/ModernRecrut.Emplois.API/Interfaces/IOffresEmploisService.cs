using ModernRecrut.Emplois.API.Entites;
using ModernRecrut.Emplois.API.DTO;

namespace ModernRecrut.Emplois.API.Interfaces
{
    public interface IOffresEmploisService
    {
        public Task Ajouter(RequeteOffreEmploi requeteOffreEmploi);
        public Task<OffreEmploi?> ObtenirSelonId(Guid id);
        public Task<IEnumerable<OffreEmploi>> ObtenirTout();
        public Task Modifier(OffreEmploi OffreEmploi);
        public Task<bool> Supprimer(Guid id);
    }
}

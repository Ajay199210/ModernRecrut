using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Interfaces
{
    public interface IFavorisService
    {
        public Task Ajouter(RequeteFavori requeteFavori);
        public Task<OffreEmploi> ObtenirSelonId(Guid id);
        public Task<IEnumerable<OffreEmploi>> ObtenirTout(string adresseIP);
        public Task Supprimer(string addresseIP, Guid id);
    }
}

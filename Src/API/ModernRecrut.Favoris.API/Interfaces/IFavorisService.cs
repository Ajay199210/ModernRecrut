using ModernRecrut.Favoris.API.Entites;
using ModernRecrut.Favoris.API.DTO;

namespace ModernRecrut.Favoris.API.Interfaces
{
    public interface IFavorisService
    {
        public bool Ajouter(RequeteFavori requeteFavori);
        public IEnumerable<OffreEmploi> ObtenirTout(string adresseIP);
        public bool Supprimer(string addresseIP, Guid id);
    }
}

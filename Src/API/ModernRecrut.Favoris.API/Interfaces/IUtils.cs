using ModernRecrut.Favoris.API.Entites;

namespace ModernRecrut.Favoris.API.Interfaces
{
    public interface IUtils
    {
        public bool VerifierAdresseIP(string Ip);
        public int CalculerTailleCache(List<OffreEmploi> offres);
    }
}

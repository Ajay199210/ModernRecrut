using ModernRecrut.Favoris.API.Entites;

namespace ModernRecrut.Favoris.API.DTO
{
    public class RequeteFavori
    {
        //[Required(ErrorMessage = "L'addresse IP est obligatoire")]
        public string? AdresseIPClient { get; set; }
        public OffreEmploi? OffreEmploi { get; set; }
    }
}

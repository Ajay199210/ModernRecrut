namespace ModernRecrut.Emplois.API.DTO
{
    public class RequeteOffreEmploi
    {
        public string? Poste { get; set; }
        public string? Nom { get; set; }
        public DateTime DateAffichage { get; set; }
        public DateTime DateFin { get; set; }
        public string? Description { get; set; }
    }
}

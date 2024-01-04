using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.Emplois.API.Entites
{
    public class OffreEmploi
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Le champ 'Poste' est obligatoire")]
        public string? Poste { get; set; }
        
        [Required(ErrorMessage = "Le champ 'Nom' est obligatoire")]
        public string? Nom { get; set; }
        
        [Required(ErrorMessage = "Le champ 'Date d\'affichage' est obligatoire")]
        [DisplayName("Date d'affichage")]
        public DateTime DateAffichage { get; set; }
        
        [Required(ErrorMessage = "Le champ 'Date de fin' est obligatoire")]
        [DisplayName("Date de fin")]
        public DateTime DateFin { get; set; }
        
        [Required(ErrorMessage = "Le champ 'Description' est obligatoire")]
        public string? Description { get; set; }
    }
}

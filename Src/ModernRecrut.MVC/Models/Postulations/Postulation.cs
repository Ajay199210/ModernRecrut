using ModernRecrut.MVC.Models.User;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.Models.Postulations
{
    public class Postulation
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "L'Id du candidat est requise"), Display(Name = "Id candidat")]
        //[ForeignKey("Candidat"), Column(Order = 0)]
        public Guid IdCandidat { get; set; }

        [Required(ErrorMessage = "L'Id de l'offre d'emploi est requis"), Display(Name = "Id offre emploi")]
        //[ForeignKey("OffreEmploi"), Column(Order = 1)]
        public Guid IdOffreEmploi { get; set; }

        [Required(ErrorMessage = "Veuillez svp renseigner vos prétentions salariales"), DisplayName("Prétention salariales")]
        public decimal? PretentionSalariale { get; set; }

        [
            Required(ErrorMessage = "Veuillez svp renseigner la date de diponibilité pour l'entrevue"),
            DisplayName("Date de disponibilité"),
            DataType(DataType.Date),
            DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)
        ]
        public DateTime DateDisponibilite { get; set; }


        // Propriétés de navigation
        public virtual ICollection<Note>? Notes { get; set; }
        public virtual OffreEmploi? OffreEmploi { get; set; }
        public virtual Utilisateur? Candidat { get; set; }
    }
}

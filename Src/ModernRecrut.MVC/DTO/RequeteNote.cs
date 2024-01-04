using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.DTO
{
    public class RequeteNote
    {
        [Display(Name = "Id postulation")]
        [Required]
        public Guid IdPostulation { get; set; }
        [Required]
        public string? Contenu { get; set; }

        [Display(Name = "Nom de l'émetteur")]
        public string? NomEmetteur { get; set; }
    }
}
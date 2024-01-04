using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ModernRecrut.MVC.Models.Postulations
{
    public class Note
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Id postulation")]
        [Required]
        public Guid IdPostulation { get; set; }
        [Required(ErrorMessage = "Veuillez svp devez saisir la note")]
        public string? Contenu { get; set; }

        [Display(Name = "Nom de l'émetteur")]
        public string? NomEmetteur { get; set; }

        // Propriété de navigation
        [JsonIgnore]
        public virtual Postulation? Postulation { get; set; }
    }
}

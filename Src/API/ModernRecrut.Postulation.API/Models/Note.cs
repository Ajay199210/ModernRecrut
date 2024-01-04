using ModernRecrut.Postulations.API.Entites;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ModernRecrut.Postulations.API.Models
{
    public class Note : BaseEntity
    {
        [Display(Name = "Id postulation")]
        [Required]
        public Guid IdPostulation { get; set; }
        public string? Contenu { get; set; }

        [Display(Name = "Nom de l'émetteur")]
        public string? NomEmetteur { get; set; }

        // Propriété de navigation
        [JsonIgnore]
        public virtual Postulation? Postulation { get; set; }
    }
}

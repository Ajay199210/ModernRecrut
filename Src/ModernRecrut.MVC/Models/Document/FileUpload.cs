using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.Models.Document
{
    public class FileUpload
    {
        [Required]
        public Guid IdUtilisateur { get; set; }
        [Required(ErrorMessage = "Veuillez svp devez choisir le type du document")]
        [DisplayName("Type du document")]
        public DocumentType DocType { get; set; }
        [Required(ErrorMessage = "Veuillez svp choisir un fichier pdf ou doc(x)")]
        [DisplayName("Fichier")]
        public IFormFile? FileData { get; set; }
    }
}

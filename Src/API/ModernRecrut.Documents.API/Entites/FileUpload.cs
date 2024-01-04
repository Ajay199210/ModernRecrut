using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.Documents.API.Entites
{
    public class FileUpload
    {
        [Required]
        [DisplayName("Id Utilisateur")]
        public Guid IdUtilisateur { get; set; }
        [Required]
        [DisplayName("Type du document")]
        public DocumentType DocType { get; set; }
        [Required]
        [DisplayName("Fichier")]
        public IFormFile? FileData { get; set; }
    }
}

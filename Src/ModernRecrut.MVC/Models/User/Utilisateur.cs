using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.Models.User
{
    public class Utilisateur : IdentityUser
    {
        [Required(ErrorMessage = "Le nom est obligatoire")]
        public string Nom { get; set; }
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        public string Prenom { get; set; }
        [Required(ErrorMessage = "Vous devez spécifier si vous êtes un employé ou un candidat")]
        public TypeUtilisateur Type { get; set; }
    }
}

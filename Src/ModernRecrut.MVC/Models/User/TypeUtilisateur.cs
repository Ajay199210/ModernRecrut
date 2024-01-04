using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.Models.User
{
    public enum TypeUtilisateur
    {
        [Display(Name = "Employé")]
        Employe = 1,
        [Display(Name = "Candidat")]
        Candidat = 2
    }
}

using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.Models.Document
{
    public enum DocumentType
    {
        CV = 1,
        [Display(Name = "Lettre de motivation")]
        LettreDeMotivation = 2,
        [Display(Name = "Diplôme")]
        Diplome = 3
    }
}

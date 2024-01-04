using System.ComponentModel;

namespace ModernRecrut.MVC.Models.User
{
    public class UserRoleViewModel
    {
        [DisplayName("Utilisateur")]
        public string UserId { get; set; }
        [DisplayName("Rôle")]
        public string RoleName { get; set; }
    }
}

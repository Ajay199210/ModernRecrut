using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Models.Postulations;

namespace ModernRecrut.MVC.Interfaces
{
    public interface IPostulationsService
    {
        public Task Postuler(RequetePostulation requetePostulation);
        public Task<Postulation?> ObtenirSelonId(Guid id);
        public Task<IEnumerable<Postulation>> ObtenirTout();
        public Task<bool> Modifier(Postulation postulation);
        public Task Supprimer(Postulation postulation);
    }
}

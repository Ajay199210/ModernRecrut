using ModernRecrut.Postulations.API.Models;
using ModernRecrut.Postulations.API.DTO;

namespace ModernRecrut.Postulations.API.Interfaces
{
    public interface IPostulationsService
    {
        public Task<Postulation?> Postuler(RequetePostulation requetePostulation);
        public Task<Postulation?> ObtenirSelonId(Guid id); // consulter
        public Task<IEnumerable<Postulation>> ObtenirTout();
        public Task<bool> Modifier(Postulation postulation);
        //public Note GenererEvaluation(decimal pretentionSalariale);
        public Task<bool> Supprimer(Guid id);
        //public Task Supprimer(Postulation item);
    }
}

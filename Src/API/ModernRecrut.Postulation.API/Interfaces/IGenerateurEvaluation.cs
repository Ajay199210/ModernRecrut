using ModernRecrut.Postulations.API.Models;

namespace ModernRecrut.Postulations.API.Interfaces
{
    public interface IGenerateurEvaluation
    {
        public Note? GenererEvalutaion(decimal pretentionSalariale);
    }
}

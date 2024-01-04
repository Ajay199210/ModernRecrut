using ModernRecrut.Postulations.API.DTO;
using ModernRecrut.Postulations.API.Interfaces;
using ModernRecrut.Postulations.API.Models;
using ModernRecrut.Postulations.API.Data;
using Microsoft.EntityFrameworkCore;

namespace ModernRecrut.Postulations.API.Services
{
    public class PostulationsService : IPostulationsService
    {
        private readonly PostulationContext _context;
        private readonly IGenerateurEvaluation _generateurEvaluation;

        public PostulationsService(PostulationContext context, IGenerateurEvaluation generateurEvaluation)
        {
            _context = context;
            _generateurEvaluation = generateurEvaluation;
        }

        public async Task<Postulation?> Postuler(RequetePostulation requetePostulation)
        {
            Postulation postulation = new()
            {
                Id = Guid.NewGuid(),
                IdCandidat = requetePostulation.IdCandidat,
                IdOffreEmploi = requetePostulation.IdOffreEmploi,
                PretentionSalariale = requetePostulation.PretentionSalariale,
                DateDisponibilite = requetePostulation.DateDisponibilite,
                Notes = new List<Note>()
            };

            // Générer la note après la postulation
            var noteGeneree = _generateurEvaluation.GenererEvalutaion(requetePostulation.PretentionSalariale);
            
            if(noteGeneree != null)
            {
                noteGeneree.Id = Guid.NewGuid();
                //noteGeneree.NomEmetteur = "ApplicationPostulation"; // généré automatiquement
                noteGeneree.IdPostulation = postulation.Id;
                noteGeneree.Postulation = postulation;
                postulation.Notes.Add(noteGeneree);
            }

            if (postulation.DateDisponibilite < DateTime.Now ||
                postulation.DateDisponibilite > DateTime.Now.AddDays(45))
            {
                return null;
            }

            await _context.AddAsync(postulation);
            await _context.SaveChangesAsync();

            return postulation;
        }
        
        public async Task<bool> Modifier(Postulation postulation)
        {
            var postulationAModifier = await ObtenirSelonId(postulation.Id);
            
            if(postulationAModifier != null)    
            {
                // Modifier le contenu de la note selon les nouvelles prétentions salariales
                if(postulation.PretentionSalariale != postulationAModifier.PretentionSalariale)
                {
                    postulationAModifier.Notes.First().Contenu = _generateurEvaluation.GenererEvalutaion(
                        postulation.PretentionSalariale).Contenu;
                }
                
                if(postulationAModifier.DateDisponibilite < DateTime.Now.AddDays(-5)
                    || postulationAModifier.DateDisponibilite > DateTime.Now.AddDays(5))
                {
                    return false;
                }

                postulationAModifier.PretentionSalariale = postulation.PretentionSalariale;
                postulationAModifier.DateDisponibilite = postulation.DateDisponibilite;

                _context.Update(postulationAModifier);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Postulation?> ObtenirSelonId(Guid id)
        {
            return await _context.Postulations
                .Include(p => p.Notes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Postulation>> ObtenirTout()
        {
            return await _context.Postulations.ToListAsync();
        }

        public async Task<bool> Supprimer(Guid id)
        {
            var postulationASupprimer = await ObtenirSelonId(id);

            if (postulationASupprimer != null)
            {
                if (postulationASupprimer.DateDisponibilite < DateTime.Now.AddDays(-5)
                    || postulationASupprimer.DateDisponibilite > DateTime.Now.AddDays(5))
                {
                    return false;
                }

                _context.Postulations.Remove(postulationASupprimer);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ModernRecrut.Emplois.API.Data;
using ModernRecrut.Emplois.API.DTO;
using ModernRecrut.Emplois.API.Entites;
using ModernRecrut.Emplois.API.Interfaces;

namespace ModernRecrut.Emplois.API.Services
{
    public class OffresEmploisService : IOffresEmploisService
    {
        private readonly OffreEmploiContext _context;

        public OffresEmploisService(OffreEmploiContext context)
        {
            _context = context;
        }

        public async Task Ajouter(RequeteOffreEmploi requeteOffreEmploi)
        {
            OffreEmploi offreEmploi = new OffreEmploi
            {
                Poste = requeteOffreEmploi.Poste,
                Nom = requeteOffreEmploi.Nom,
                DateAffichage = requeteOffreEmploi.DateAffichage,
                DateFin = requeteOffreEmploi.DateFin,
                Description = requeteOffreEmploi.Description,
            };

            if(offreEmploi.DateAffichage < offreEmploi.DateFin)
            {
                await _context.AddAsync(offreEmploi);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Modifier(OffreEmploi offreEmploi)
        {
            var offreAModifier = await ObtenirSelonId(offreEmploi.Id);

            offreAModifier.Poste = offreEmploi.Poste;
            offreAModifier.Nom = offreEmploi.Nom;
            offreAModifier.DateAffichage = offreEmploi.DateAffichage;
            offreAModifier.DateFin = offreEmploi.DateFin;
            offreAModifier.Description = offreEmploi.Description;
            
            _context.Update(offreAModifier);
            await _context.SaveChangesAsync();
        }

        public async Task<OffreEmploi?> ObtenirSelonId(Guid id)
        {
            var offresEmplois = await ObtenirTout();
            return offresEmplois.FirstOrDefault(oe => oe.Id == id);
        }

        public async Task<IEnumerable<OffreEmploi>> ObtenirTout()
        {
            return await _context.OffresEmplois.ToListAsync();
        }

        public async Task<bool> Supprimer(Guid id)
        {
            var offreASupprimer = await ObtenirSelonId(id);
            if(offreASupprimer != null)
            {
                _context.OffresEmplois.Remove(offreASupprimer);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}

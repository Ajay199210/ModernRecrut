using ModernRecrut.Favoris.API.Entites;
using ModernRecrut.Favoris.API.Interfaces;
using System.Text.RegularExpressions;

namespace ModernRecrut.Favoris.API.Services
{
    public class Utils : IUtils
    {
        // Vérifier le format d'un addresse IPv4
        public bool VerifierAdresseIP(string Ip)
        {
            string pattern = @"^(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)
                            \.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)
                            \.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)
                            \.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)$";
            
            Regex regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace); // Ignore pattern white space
            Match match = regex.Match(Ip);

            return match.Success;
        }

        // Calculer la taille totale de la cache (tous les offres existantes)
        public int CalculerTailleCache(List<OffreEmploi> offres)
        {
            int taille = 0;
            foreach (OffreEmploi offre in offres)
            {
                taille += offre.Poste.Length + offre.Nom.Length + offre.Description.Length
                                     + offre.DateAffichage.ToString().Length +
                                     offre.DateAffichage.ToString().Length;
            }
            return taille;
        }
    }
}

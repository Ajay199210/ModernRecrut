using ModernRecrut.Postulations.API.Interfaces;
using ModernRecrut.Postulations.API.Models;

namespace ModernRecrut.Postulations.API.Services
{
    public class GenerateurEvaluation : IGenerateurEvaluation
    {
        public Note? GenererEvalutaion(decimal pretentionSalariale)
        {
            Note note = new();

            note.NomEmetteur = "ApplicationPostulation";

            // `Pattern matching` entre pretentionSalariales et salaire
            // pour tester chaque cas incluant les intervales <= ou >=
            switch (pretentionSalariale)
            {
                case var salaire when salaire < 20000:
                    note.Contenu = "Salaire inférieur à la norme";
                    break;

                case var salaire when salaire <= 39999:
                    note.Contenu = "Salaire proche mais inférieur à la norme";
                    break;

                case var salaire when salaire <= 79999:
                    note.Contenu = "Salaire dans la norme";
                    break;

                case var salaire when salaire <= 99999:
                    note.Contenu = "Salaire proche mais supérieur à la norme";
                    break;

                case var salaire when salaire >= 100000:
                    note.Contenu = "Salaire supérieur à la norme";
                    break;
            }

            return note;
        }
    }
}

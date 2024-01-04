using FluentAssertions;
using ModernRecrut.Postulations.API.Services;
using Xunit;

namespace ModernRecrut.Postulations.API.UnitTests
{
    public class GenerateurEvaluationTestsUnits
    {
        [Theory]
        [InlineData(15000, "Salaire inférieur à la norme")]
        [InlineData(30000, "Salaire proche mais inférieur à la norme")]
        [InlineData(39999, "Salaire proche mais inférieur à la norme")]
        [InlineData(65000, "Salaire dans la norme")]
        [InlineData(79999, "Salaire dans la norme")]
        [InlineData(85000, "Salaire proche mais supérieur à la norme")]
        [InlineData(99999, "Salaire proche mais supérieur à la norme")]
        [InlineData(100000, "Salaire supérieur à la norme")]
        [InlineData(250000, "Salaire supérieur à la norme")]
        public void GenererEvaluation_PretentionsSalariales_Retourne_Note(
            decimal pretentionSalariale, string? contenuNote)
        {
            //Etant donné
            GenerateurEvaluation generateurEvaluation = new GenerateurEvaluation();

            //Lorsque
            var note = generateurEvaluation.GenererEvalutaion(pretentionSalariale);

            //Alors
            note.Should().NotBeNull();

            note.NomEmetteur.Should().NotBeEmpty().And.HaveLength(22)
                .And.Be("ApplicationPostulation");

            note.Contenu.Should().NotBeEmpty().And.StartWith("Salaire")
                .And.EndWith("norme").And.Be(contenuNote);
        }
    }
}

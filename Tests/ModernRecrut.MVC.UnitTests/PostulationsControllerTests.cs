using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModernRecrut.MVC.Controllers;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Interfaces.Documents;
using ModernRecrut.MVC.Models;
using ModernRecrut.MVC.Models.Postulations;
using ModernRecrut.MVC.Models.User;
using Moq;
using System.Security.Claims;
using Xunit;

namespace ModernRecrut.MVC.UnitTests
{
    public class PostulationsControllerTests
    {
        [Fact]
        public async Task Postuler_CVManquant_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var requetePostulation = fixture.Create<RequetePostulation>();
            requetePostulation.IdCandidat = It.IsAny<Guid>();
            requetePostulation.IdOffreEmploi = It.IsAny<Guid>();
            requetePostulation.DateDisponibilite = DateTime.Now.AddDays(3);
            requetePostulation.PretentionSalariale = 70000;

            var utilisateur = fixture.Create<Utilisateur>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();

            List<string> documents = new List<string> // ne contient pas un CV
            { 
                $"{Guid.NewGuid()}_Test_100",
                $"{Guid.NewGuid()}_LettreDeMotivation_101" 
            };

            var postulation = fixture.Create<Postulation>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null, 
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(Guid.NewGuid().ToString());
            
            mockDocumentService.Setup(d => d.GetDocumentsByUserId(It.IsAny<Guid>()))
                .Returns(Task.FromResult<IEnumerable<string>>(documents));
            
            mockPostulationService.Setup(ps => ps.Postuler(It.IsAny<RequetePostulation>()));

            // Initialisation des controlleurs
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            var postulationResult = actionResult.Model as Postulation;
            postulationResult.Should().NotBe(postulation);
            mockPostulationService.Verify(x => x.Postuler(
                It.IsAny<RequetePostulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("");
            modelState[""].Errors.First().ErrorMessage.Should()
                .Be($"Un CV est obligatoire pour postuler. Veuillez déposer au préalable un CV dans votre espace Documents");
        }

        [Fact]
        public async Task Postuler_LettreDeMotivationManquant_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var requetePostulation = fixture.Create<RequetePostulation>();
            requetePostulation.IdCandidat = It.IsAny<Guid>();
            requetePostulation.IdOffreEmploi = It.IsAny<Guid>();
            requetePostulation.DateDisponibilite = DateTime.Now.AddDays(3);
            requetePostulation.PretentionSalariale = 70000;

            var utilisateur = fixture.Create<Utilisateur>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();
            
            List<string> documents = new List<string> // ne contient pas une lettre de motivation
            { 
                $"{Guid.NewGuid()}_CV_100",
                $"{Guid.NewGuid()}_Test_101" 
            };

            var postulation = fixture.Create<Postulation>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(It.IsAny<Guid>().ToString());
            mockDocumentService.Setup(d => d.GetDocumentsByUserId(It.IsAny<Guid>()))
                .Returns(Task.FromResult<IEnumerable<string>>(documents));
            mockPostulationService.Setup(ps => ps.Postuler(It.IsAny<RequetePostulation>()));

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            var postulationResult = actionResult.Model as Postulation;
            postulationResult.Should().NotBe(postulation);
            mockPostulationService.Verify(x => x.Postuler(
                It.IsAny<RequetePostulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("");
            modelState[""].Errors.First().ErrorMessage
                .Should().Be($"Une lettre de motivation est obligatoire pour postuler. " +
                    "Veuillez déposer au préalable une lettre de motivation dans votre espace Documents");
        }

        [Fact]
        public async Task Postuler_DateDispoDansLePasse_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var requetePostulation = fixture.Create<RequetePostulation>();
            requetePostulation.IdCandidat = It.IsAny<Guid>();
            requetePostulation.IdOffreEmploi = It.IsAny<Guid>();
            requetePostulation.DateDisponibilite = DateTime.Now.AddDays(-25); // date de disponiblité dans le passé
            requetePostulation.PretentionSalariale = 70000;

            var utilisateur = fixture.Create<Utilisateur>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();
            
            List<string> documents = new List<string> 
            {
                $"{Guid.NewGuid()}_CV_100",
                $"{Guid.NewGuid()}_LettreDeMotivation_101" 
            };
            
            var postulation = fixture.Create<Postulation>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(It.IsAny<Guid>().ToString());
            mockDocumentService.Setup(d => d.GetDocumentsByUserId(It.IsAny<Guid>()))
                .Returns(Task.FromResult<IEnumerable<string>>(documents));
            mockPostulationService.Setup(ps => ps.Postuler(It.IsAny<RequetePostulation>()));

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object )
                { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            var postulationResult = actionResult.Model as Postulation;
            postulationResult.Should().NotBe(postulation);
            mockPostulationService.Verify(x => x.Postuler(
                It.IsAny<RequetePostulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.First().ErrorMessage
                .Should().Be($"La date de disponibilité doit être supérieure " +
                    $"à la date du jour et inférieure au {DateTime.Now.AddDays(45)}" );
        }

        [Fact]
        public async Task Postuler_DateDispoPlusQue45Jours_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var requetePostulation = fixture.Create<RequetePostulation>();
            requetePostulation.IdCandidat = It.IsAny<Guid>();
            requetePostulation.IdOffreEmploi = It.IsAny<Guid>();
            requetePostulation.DateDisponibilite = DateTime.Now.AddDays(60); // date de disponiblité plus que 45 jours
            requetePostulation.PretentionSalariale = 70000;

            var utilisateur = fixture.Create<Utilisateur>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();
            List<string> documents = new List<string> 
            {
                $"{Guid.NewGuid()}_CV_100",
                $"{Guid.NewGuid()}_LettreDeMotivation_101" 
            };

            var postulation = fixture.Create<Postulation>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(Guid.NewGuid().ToString());
            var mockPostulationService = new Mock<IPostulationsService>();
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            mockDocumentService.Setup(d => d.GetDocumentsByUserId(It.IsAny<Guid>()))
                .Returns(Task.FromResult<IEnumerable<string>>(documents));
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();

            // Configurer les substitutions
            mockPostulationService.Setup(ps => ps.Postuler(It.IsAny<RequetePostulation>()));

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            var postulationResult = actionResult.Model as Postulation;
            postulationResult.Should().NotBe(postulation);
            mockPostulationService.Verify(x => x.Postuler(
                It.IsAny<RequetePostulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.First().ErrorMessage
                .Should().Be($"La date de disponibilité doit être supérieure " +
                    $"à la date du jour et inférieure au {DateTime.Now.AddDays(45)}");
        }

        [Fact]
        public async Task Postuler_PretentionSalarialeSupAuxLimites_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var requetePostulation = fixture.Create<RequetePostulation>();
            requetePostulation.IdCandidat = It.IsAny<Guid>();
            requetePostulation.IdOffreEmploi = It.IsAny<Guid>();
            requetePostulation.PretentionSalariale = 999999; // prétention salariale au-delà des limites
            requetePostulation.DateDisponibilite = DateTime.Now.AddDays(5);

            var utilisateur = fixture.Create<Utilisateur>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();
            List<string> documents = new List<string> 
            {
                $"{Guid.NewGuid()}_CV_100",
                $"{Guid.NewGuid()}_LettreDeMotivation_101" 
            };

            var postulation = fixture.Create<Postulation>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null );
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(Guid.NewGuid().ToString());
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            mockDocumentService.Setup(d => d.GetDocumentsByUserId(It.IsAny<Guid>()))
                .Returns(Task.FromResult<IEnumerable<string>>(documents));
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockPostulationService.Setup(ps => ps.Postuler(It.IsAny<RequetePostulation>()));

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            var postulationResult = actionResult.Model as Postulation;
            postulationResult.Should().NotBe(postulation);
            mockPostulationService.Verify(x => x.Postuler(
                It.IsAny<RequetePostulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("PretentionSalariale");
            modelState["PretentionSalariale"].Errors.First().ErrorMessage
                .Should().Be($"Votre prétention salariale est au-delà de nos limites");
        }

        [Fact]
        public async Task Postuler_PostulationExistante_Retourne_ViewResult() // candidat a déjà postulé pour une offre
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var requetePostulation = fixture.Create<RequetePostulation>();
            requetePostulation.IdCandidat = It.IsAny<Guid>();
            requetePostulation.IdOffreEmploi = It.IsAny<Guid>();
            requetePostulation.PretentionSalariale = 75000;
            requetePostulation.DateDisponibilite = DateTime.Now.AddDays(15);

            var utilisateur = fixture.Create<Utilisateur>();
            utilisateur.Id = Guid.NewGuid().ToString(); 
            
            var httpContext = new DefaultHttpContext();
            var idOffreEmploi = It.IsAny<Guid>();
            httpContext.Request.RouteValues["id"] = idOffreEmploi;
            
            List<string> documents = new List<string> 
            { 
                $"{Guid.NewGuid()}_CV_100",
                $"{Guid.NewGuid()}_LettreDeMotivation_101" 
            };

            List<OffreEmploi> offresEmplois = fixture.CreateMany<OffreEmploi>().ToList();

            var postulations = fixture.CreateMany<Postulation>().ToList();

            // Créer une postulation déjà existante pour le test
            var postulation = fixture.Create<Postulation>();
            postulation.IdCandidat = Guid.Parse(utilisateur.Id);
            postulation.IdOffreEmploi = idOffreEmploi;
            postulations.Add(postulation);

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(utilisateur.Id);
            mockOffreEmploisService.Setup(e => e.ObtenirTout())
                .Returns(Task.FromResult<IEnumerable<OffreEmploi>>(offresEmplois));
            mockDocumentService.Setup(d => d.GetDocumentsByUserId(It.IsAny<Guid>()))
                .Returns(Task.FromResult<IEnumerable<string>>(documents));
            mockPostulationService.Setup(e => e.ObtenirTout()).Returns(Task.FromResult<IEnumerable<Postulation>>(postulations));

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            //actionResult.ViewName.Should().BeNullOrEmpty();
            mockPostulationService.Verify(x => x.Postuler(
                It.IsAny<RequetePostulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("");
            modelState[""].Errors.First().ErrorMessage
                .Should().Be($"Vous avez déjà postuler pour cette offre d'emploi." +
                $" Veuillez postuler pour une autre offre.");
        }

        [Fact]
        public async Task Postuler_ModelStateInvalide_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var requetePostulation = fixture.Create<RequetePostulation>();
            requetePostulation.IdCandidat = It.IsAny<Guid>();
            requetePostulation.IdOffreEmploi = It.IsAny<Guid>();
            requetePostulation.PretentionSalariale = null;
            requetePostulation.DateDisponibilite = null;

            var utilisateur = fixture.Create<Utilisateur>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();
            
            List<string> documents = new List<string> 
            { 
                $"{Guid.NewGuid()}_CV_100",
                $"{Guid.NewGuid()}_LettreDeMotivation_101" 
            };

            var postulation = fixture.Create<Postulation>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockPostulationService.Setup(ps => ps.Postuler(It.IsAny<RequetePostulation>()));
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(Guid.NewGuid().ToString());
            mockDocumentService.Setup(d => d.GetDocumentsByUserId(It.IsAny<Guid>()))
                .Returns(Task.FromResult<IEnumerable<string>>(documents));

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            postulationsController.ModelState.AddModelError("PretentionSalariale", 
                "Votre prétention salariale est au-delà de nos limites");

            //// Lorsque ////
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            actionResult.Model.Should().Be(requetePostulation);
            mockPostulationService.Verify(x => x.Postuler(
                It.IsAny<RequetePostulation>()),
                Times.Never);
        }

        [Fact]
        public async Task Postuler_PostulationValide_Retourne_RedirectToAction()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var requetePostulation = fixture.Create<RequetePostulation>();

            requetePostulation.IdCandidat = It.IsAny<Guid>();
            requetePostulation.IdOffreEmploi = It.IsAny<Guid>();
            requetePostulation.PretentionSalariale = 75000;
            requetePostulation.DateDisponibilite = DateTime.Now.AddDays(15);

            var utilisateur = fixture.Create<Utilisateur>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();
            
            List<string> documents = new List<string> 
            { 
                $"{Guid.NewGuid()}_CV_100",
                $"{Guid.NewGuid()}_LettreDeMotivation_101" 
            };

            var postulation = fixture.Create<Postulation>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(Guid.NewGuid().ToString());
            mockDocumentService.Setup(d => d.GetDocumentsByUserId(It.IsAny<Guid>()))
                .Returns(Task.FromResult<IEnumerable<string>>(documents));
            mockPostulationService.Setup(ps => ps.Postuler(It.IsAny<RequetePostulation>()));

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Postuler(requetePostulation) as RedirectToActionResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Index");
            mockPostulationService.Verify(x => x.Postuler(
                It.IsAny<RequetePostulation>()),
                Times.Once);
        }

        [Fact]
        public async Task Edit_GET_PostulationIdInexistant_Retourne_NotFound()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null);
            var mockPostulationService = new Mock<IPostulationsService>();
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            //mockDocumentService.Setup(d => d.GetDocumentsByUserId(It.IsAny<Guid>()))
            //    .Returns(Task.FromResult<IEnumerable<string>>(documents));
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(Guid.NewGuid().ToString());
            mockPostulationService.Setup(e => e.ObtenirSelonId(It.IsAny<Guid>())).Returns(Task.FromResult<Postulation>(null));

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Edit(Guid.NewGuid());

            /// Alors ////
            actionResult.Should().BeOfType(typeof(NotFoundResult));
        }

        [Fact]
        public async Task Edit_POST_DateDispoInf5JoursAujourdhui_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Créer une postulation
            var postulation = fixture.Create<Postulation>();
            var postulationAModifier = fixture.Create<Postulation>();
            // La date de disponibitlité de la postulation à modifier est inférieure de 5 jours à la date du jour
            postulationAModifier.DateDisponibilite = DateTime.Now.AddDays(-13);
            postulation.DateDisponibilite = DateTime.Now.AddDays(5);
            postulation.PretentionSalariale = 75500;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(Guid.NewGuid().ToString());

            mockPostulationService.Setup(e => e.ObtenirSelonId(It.IsAny<Guid>()))
                .ReturnsAsync(postulationAModifier);

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            mockPostulationService.Verify(x => x.Modifier(
                It.IsAny<Postulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.First().ErrorMessage
                .Should().Be($"Il n’est pas possible de modifier " +
                    $"une postulation dont la date de disponibilité ({postulationAModifier.DateDisponibilite}) est " +
                    $"inférieure ou supérieure de 5 jours à la date du jour ({DateTime.Today})");
        }

        [Fact]
        public async Task Edit_POST_DateDispoSup5JoursAujourdhui_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Créer une postulation
            var postulation = fixture.Create<Postulation>();
            var postulationAModifier = fixture.Create<Postulation>();
            // La date de disponibitlité de la postulation à modifier est inférieure de 5 jours à la date du jour
            postulationAModifier.DateDisponibilite = DateTime.Now.AddDays(10);
            postulation.DateDisponibilite = DateTime.Now.AddDays(5);
            postulation.PretentionSalariale = 75500;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(Guid.NewGuid().ToString());

            mockPostulationService.Setup(e => e.ObtenirSelonId(It.IsAny<Guid>()))
                .ReturnsAsync(postulationAModifier);

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Edit(postulationAModifier) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            mockPostulationService.Verify(x => x.Modifier(
                It.IsAny<Postulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.First().ErrorMessage
                .Should().Be($"Il n’est pas possible de modifier " +
                    $"une postulation dont la date de disponibilité ({postulationAModifier.DateDisponibilite}) est " +
                    $"inférieure ou supérieure de 5 jours à la date du jour ({DateTime.Today})");
        }

        [Fact]
        public async Task Edit_GET_PostulationIdExistant_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Créer une postulation
            var postulation = fixture.Create<Postulation>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null, 
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(Guid.NewGuid().ToString());
            mockPostulationService.Setup(e => e.ObtenirSelonId(It.IsAny<Guid>()))
                .ReturnsAsync(postulation);

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Edit(Guid.NewGuid()) as ViewResult;

            /// Alors ////
            actionResult.Should().NotBeNull();
            var postulationResult = actionResult.Model as Postulation;
            postulationResult.Should().Be(postulation);
        }

        [Fact]
        public async Task Edit_POST_DateDispoInfAuj_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Créer une postulation
            var postulation = fixture.Create<Postulation>();
            var postulationAModifier = fixture.Create<Postulation>();
            postulationAModifier.DateDisponibilite = DateTime.Now.AddDays(3);
            postulation.DateDisponibilite = DateTime.Now.AddDays(-25);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(Guid.NewGuid().ToString());
            mockPostulationService.Setup(e => e.ObtenirSelonId(It.IsAny<Guid>())).ReturnsAsync(postulationAModifier);

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            mockPostulationService.Verify(x => x.Modifier(
                It.IsAny<Postulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.First().ErrorMessage
                .Should().Be($"La date de disponibilité doit être supérieure " +
                    $"à la date du jour et inférieure au {DateTime.Now.AddDays(45)}");
        }

        [Fact]
        public async Task Edit_POST_DateDispoAuj_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Créer une postulation dont la date de disponibilité est aujourd'hui
            var postulation = fixture.Create<Postulation>();
            var aujourdhui = DateTime.Now.Date;
            postulation.DateDisponibilite = aujourdhui;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(Guid.NewGuid().ToString());

            mockPostulationService.Setup(e => e.ObtenirSelonId(It.IsAny<Guid>()))
                .ReturnsAsync(postulation);

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            mockPostulationService.Verify(x => x.Modifier(
                It.IsAny<Postulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.First().ErrorMessage
                .Should().Be($"La date de disponibilité doit être supérieure " +
                    $"à la date du jour et inférieure au {DateTime.Now.AddDays(45)}");
        }

        [Fact]
        public async Task Edit_POST_DateSup45JoursAuj_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Créer une postulation
            var postulation = fixture.Create<Postulation>();
            var postulationAModifier = fixture.Create<Postulation>();
            postulationAModifier.DateDisponibilite = DateTime.Now.AddDays(1);
            postulation.DateDisponibilite = DateTime.Now.AddDays(75);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(Guid.NewGuid().ToString());

            mockPostulationService.Setup(e => e.ObtenirSelonId(It.IsAny<Guid>()))
                .ReturnsAsync(postulationAModifier);

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            mockPostulationService.Verify(x => x.Modifier(
                It.IsAny<Postulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.First().ErrorMessage
                .Should().Be($"La date de disponibilité doit être supérieure " +
                    $"à la date du jour et inférieure au {DateTime.Now.AddDays(45)}");
        }

        [Fact]
        public async Task Edit_POST_PretentionSalarialeSupAuxLimites_Retourne_ViewResult()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Créer une postulation
            var postulation = fixture.Create<Postulation>();
            var postulationAModifier = fixture.Create<Postulation>();
            postulationAModifier.DateDisponibilite = DateTime.Today;
            postulation.DateDisponibilite = DateTime.Now.AddDays(10);
            postulation.PretentionSalariale = 9999999; // prétention salariale au-delà des limites

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(Guid.NewGuid().ToString());

            mockPostulationService.Setup(e => e.ObtenirSelonId(It.IsAny<Guid>()))
                .ReturnsAsync(postulationAModifier);

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            mockPostulationService.Verify(x => x.Modifier(
                It.IsAny<Postulation>()),
                Times.Never);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("PretentionSalariale");
            modelState["PretentionSalariale"].Errors.First().ErrorMessage
                .Should().Be($"Votre prétention salariale est au-delà de nos limites");
        }

        [Fact]
        public async Task Edit_POST_ModelStateValide_Retourne_RedirectToAction()
        {
            //// Etant donné ////

            // Initialisation des données
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Créer une postulation
            var postulation = fixture.Create<Postulation>();
            var postulationAModifier = fixture.Create<Postulation>();
            postulationAModifier.DateDisponibilite = DateTime.Now.AddDays(5);
            postulation.DateDisponibilite = DateTime.Now.AddDays(11);
            postulation.PretentionSalariale = 75000;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues["id"] = It.IsAny<Guid>();

            // Initialisation de l'instance des Mocks pour le controller des postulations
            var mockLoggerService = new Mock<ILogger<PostulationsController>>();
            var mockUserStore = new Mock<IUserStore<Utilisateur>>();
            var mockUserManager = new Mock<UserManager<Utilisateur>>(mockUserStore.Object, null, null, null,
                null, null, null, null, null);
            var mockOffreEmploisService = new Mock<IOffresEmploisService>();
            var mockDocumentService = new Mock<IDocumentService>();
            var mockPostulationService = new Mock<IPostulationsService>();

            // Configurer les substitutions
            mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(Guid.NewGuid().ToString());

            mockPostulationService.Setup(e => e.ObtenirSelonId(It.IsAny<Guid>()))
                .ReturnsAsync(postulationAModifier);

            // Initialisation du controlleur
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var postulationsController = new PostulationsController(
                mockUserManager.Object,
                mockPostulationService.Object,
                mockOffreEmploisService.Object, mockDocumentService.Object,
                mockLoggerService.Object)
            { ControllerContext = controllerContext };

            //// Lorsque ////
            var actionResult = await postulationsController.Edit(postulation) as RedirectToActionResult;

            //// Alors ////
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Index");
            mockPostulationService.Verify(x => x.Modifier(
                It.IsAny<Postulation>()),
                Times.Once);

            // Validation des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeTrue();
            modelState.ErrorCount.Should().Be(0);
        }
    }
}

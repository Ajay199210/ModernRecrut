# ModernRecrut

La solution ModernRecrut est constituée de l'interface web implémentée avec ASP.NET Core MVC et quelques applications qui utilisent les services développés avec ASP.NET Core Web API pour répondre aux besoins des utilisateurs.

Elle permet d'identifier les composantes suivantes :

| Microservice | Fonctionnalités |
|:---:|:---:|
| Gestion des offres d'emploi | Création, modification, affichage et suppression<br>des offres d’emploi |
| Gestion des favoris | Ajout, affichage et suppression des offres dans les favoris |
| Gestion des utilisateurs | Création, modification et suppression des comptes<br>Création, modification et suppression des rôles<br>Association des comptes aux rôles<br>Connexion<br>Gestion des autorisations |
| Gestion des postulations | Postulation des candidats<br>Consultations des postulations<br>Mise à jour des postulations |
| Gestion des documents | Dépôts des documents (CV, lettre de motivation, etc.)<br>Affichage des documents |
| Application web | Offre l'interface pour interagir avec les microservices |

<br>

*Des tests unitaires ont été mis en place pour tester la gestion des postulations en utilisant xUnit, Moq, AutoFixture et FluentAssertion.*

*Le fichier **LOGINS** contient les comptes nécessaires pour tester l'application.*

using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models.Postulations;
using Newtonsoft.Json;
using System.Text;

namespace ModernRecrut.MVC.Services
{
    public class PostulationsServiceProxy : IPostulationsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _postulationsApiUrl = "api/postulations/";
        private readonly ILogger<OffresEmploisServiceProxy> _logger;

        public PostulationsServiceProxy(HttpClient httpClient, ILogger<OffresEmploisServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> Modifier(Postulation item)
        {
            // Constuire la requête avec les données au format JSON
            StringContent content = new StringContent(JsonConvert.SerializeObject(item),
                Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(_postulationsApiUrl + item.Id, content);

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode <= 499)
                {
                    _logger.LogError($"[-] {DateTime.Now} - Une erreur s'est produite lors de la " +
                        $"modification de la postulation #{item.Id}");
                    return false;
                }

                if ((int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
                {
                    _logger.LogCritical($"[-] {DateTime.Now} - Une erreur s'est produite lors de la " +
                        $"modification de la postulation #{item.Id}");
                    return false;
                }
            }

            _logger.LogInformation($"[+] {DateTime.Now} - Postulation #{item.Id} a été modifiée");
            return true;
        }

        public async Task<Postulation?> ObtenirSelonId(Guid id)
        {
            _logger.LogInformation($"[*] {DateTime.Now} - Obtention de la postulation #{id}...");
            var rep = await _httpClient.GetFromJsonAsync<Postulation>(_postulationsApiUrl + id);
            _logger.LogInformation($"[+] {DateTime.Now} - Postulation #{id} obtenue");
            return rep;
        }

        public async Task<IEnumerable<Postulation>> ObtenirTout()
        {
            _logger.LogInformation($"[*] {DateTime.Now} - Obtention des postulations...");
            var response = await _httpClient.GetAsync(_postulationsApiUrl);
            return await response.Content.ReadFromJsonAsync<IEnumerable<Postulation>>();
        }

        public async Task Postuler(RequetePostulation requetePostulation)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_postulationsApiUrl, requetePostulation);

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode <= 499)
                {
                    _logger.LogError($"[-] {DateTime.Now} - Une erreur s'est produite lors d'une postulation " +
                        $"par le candidat #{requetePostulation.IdCandidat}");
                    return;
                }

                if ((int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
                {
                    _logger.LogCritical($"[-] {DateTime.Now} - Une erreur critique s'est produite lors d'une postulation " +
                        $"par le candidat #{requetePostulation.IdCandidat}");
                    return;
                }
            }

            _logger.LogInformation($"[+] {DateTime.Now} - Candidat #{requetePostulation.IdCandidat} a postulé pour " +
                $"l'offre d'emploi {requetePostulation.IdOffreEmploi}");
        }

        public async Task Supprimer(Postulation postulation)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(_postulationsApiUrl + postulation.Id);

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode <= 499)
                {
                    _logger.LogError($"[-] {DateTime.Now} - Une erreur s'est produite lors de la suppression de la postulation " +
                        $"#{postulation.Id} par le candidat #{postulation.IdCandidat}");
                    return;
                }

                if ((int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
                {
                    _logger.LogCritical($"[-] {DateTime.Now} - Une erreur critique s'est produitelors de la suppression de la postulation " +
                        $"#{postulation.Id} par le candidat #{postulation.IdCandidat}");
                    return;
                }
            }
            _logger.LogInformation($"[+] {DateTime.Now} - La postulation #{postulation.Id} a été supprimé");
        }
    }
}

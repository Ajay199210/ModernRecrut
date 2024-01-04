using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;
using Newtonsoft.Json;
using System.Text;

namespace ModernRecrut.MVC.Services
{
    public class OffresEmploisServiceProxy : IOffresEmploisService
    {
        private readonly HttpClient _httpClient;
        private readonly string _offresEmploisApiUrl = "api/offresEmplois/";
        private readonly ILogger<OffresEmploisServiceProxy> _logger;

        public OffresEmploisServiceProxy(HttpClient httpClient, ILogger<OffresEmploisServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task Ajouter(RequeteOffreEmploi requeteOffreEmploi)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_offresEmploisApiUrl, requeteOffreEmploi);

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode <= 499)
                {
                    _logger.LogError("[-] Une erreur s'est produite");
                    return;
                }

                if((int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
                {
                    _logger.LogCritical("[-] Une erreur critique s'est produite");
                    return;
                }
            }
            _logger.LogInformation($"[+] Ajout de l'offre d'emploi {requeteOffreEmploi.Nom}");
        }

        public async Task Modifier(OffreEmploi item)
        {
            // Constuire la requete avec les données au format JSON
            StringContent content = new StringContent(JsonConvert.SerializeObject(item), 
                Encoding.UTF8, "application/json");

            var response =  await _httpClient.PutAsync(_offresEmploisApiUrl + item.Id, content);
            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode <= 499)
                {
                    _logger.LogError("une erreur s'est produite");
                    return;
                }

                if ((int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
                {
                    _logger.LogCritical("une erreur critique s'est produite");
                    return;
                }
            }
            _logger.LogInformation("Modification d'un offre d'emploi");
        }

        public async Task<OffreEmploi> ObtenirSelonId(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<OffreEmploi>(_offresEmploisApiUrl + id);
        }

        public async Task<IEnumerable<OffreEmploi>> ObtenirTout()
        {
            var response = await _httpClient.GetAsync(_offresEmploisApiUrl);
            return await response.Content.ReadFromJsonAsync<IEnumerable<OffreEmploi>>();
        }

        public async Task Supprimer(Guid id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(_offresEmploisApiUrl + id);

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode <= 499)
                {
                    _logger.LogError("[-] Une erreur s'est produite");
                    return;
                }

                if ((int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
                {
                    _logger.LogCritical("[-] Une erreur critique s'est produite");
                    return;
                }
            }
            _logger.LogInformation($"[+] Modification de l'offre d'emploi #{id}");
        }
    }
}

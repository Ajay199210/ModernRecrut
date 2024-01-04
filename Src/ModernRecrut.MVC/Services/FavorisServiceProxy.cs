using ModernRecrut.MVC.Models;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.DTO;

namespace ModernRecrut.MVC.Services
{
    public class FavorisServiceProxy : IFavorisService
    {
        private readonly HttpClient _httpClient;
        private readonly string _favorisApiUrl = "api/favoris/";
        private readonly ILogger<FavorisServiceProxy> _logger;

        public FavorisServiceProxy(HttpClient httpClient, ILogger<FavorisServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task Ajouter(RequeteFavori requeteFavori)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_favorisApiUrl, requeteFavori);

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
            _logger.LogInformation("Ajout d'une offre d'emploi dans la liste des favoris");
        }

        public async Task<OffreEmploi> ObtenirSelonId(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<OffreEmploi>(_favorisApiUrl + id);
        }

        public async Task<IEnumerable<OffreEmploi>> ObtenirTout(string addresseIPClient)
        {
            var response = await _httpClient.GetAsync(_favorisApiUrl + addresseIPClient);
            return await response.Content.ReadFromJsonAsync<IEnumerable<OffreEmploi>>();
        }

        public async Task Supprimer(string addresseIP, Guid id)
        {
            var response = await _httpClient.DeleteAsync(_favorisApiUrl + addresseIP + "/" + id);

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
            _logger.LogInformation("Suppression d'un offre d'emploi des favoris");
        }
    }
}

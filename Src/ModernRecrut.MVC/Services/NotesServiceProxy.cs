using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models.Postulations;
using Newtonsoft.Json;
using System.Text;

namespace ModernRecrut.MVC.Services
{
    public class NotesServiceProxy : INotesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _notesApiUrl = "api/notes/";
        private readonly ILogger<NotesServiceProxy> _logger;

        public NotesServiceProxy(HttpClient httpClient, ILogger<NotesServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> Ajouter(RequeteNote requeteNote)
        {
            var response = await _httpClient.PostAsJsonAsync(_notesApiUrl, requeteNote);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Modifier(Note note)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(note),
                Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(_notesApiUrl + note.Id, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<Note?> ObtenirSelonId(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Note>(_notesApiUrl + id);
        }

        public async Task<IEnumerable<Note>> ObtenirTout()
        {
            var response = await _httpClient.GetAsync(_notesApiUrl);
            return await response.Content.ReadFromJsonAsync<IEnumerable<Note>>();
        }

        public async Task<bool> Supprimer(Guid id)
        {
            var response = await _httpClient.DeleteAsync(_notesApiUrl + id);
            return response.IsSuccessStatusCode;
        }
    }
}

using ModernRecrut.Favoris.API.Interfaces;
using ModernRecrut.Favoris.API.DTO;
using ModernRecrut.Favoris.API.Entites;
using Microsoft.Extensions.Caching.Memory;

namespace ModernRecrut.Favoris.API.Services
{
    public class FavorisService : IFavorisService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IUtils _utils;
        private string? _cacheKey;

        public FavorisService(IMemoryCache memoryCache, IUtils utils)
        {
            _memoryCache = memoryCache;
            _utils = utils;
        }

        // Ajouter un favori en utilisant l'addresse IPv4 comme clé de la cache
        public bool Ajouter(RequeteFavori requeteFavori)
        {
            // Offre à ajouter dans la mémoire cache
            OffreEmploi? offre = requeteFavori.OffreEmploi;
            
            // Clé du cache (Ip addresse du client)
            _cacheKey = requeteFavori.AdresseIPClient;

            // Configurer la mémoire cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                // Les favoris ont une durée de 24h
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),

                // Les favoris doivent expirer après 6h en cas de non-accès
                SlidingExpiration = TimeSpan.FromHours(6),
                
                // Chaque caractère des données ajoutées au cache à une taille de 1
                Size = offre?.Poste?.Length + offre?.Nom?.Length + offre?.Description?.Length
                + offre?.DateAffichage.ToString().Length + offre?.DateAffichage.ToString().Length
            };

            if(_utils.VerifierAdresseIP(_cacheKey))
            {
                // Verifie si l'information existe dans le cache
                if (!_memoryCache.TryGetValue(_cacheKey, out Favori favoris))
                {
                    // Créer l'information à mettre en cache
                    var nouveauFavori = new Favori
                    {
                        AdressIpClient = _cacheKey,
                        OffresEmplois = new List<OffreEmploi> { offre }
                    };

                    // Ajouter l'information dans le cache
                    _memoryCache.Set(_cacheKey, nouveauFavori, cacheEntryOptions);
                    return true;
                }
                else
                {
                    // L'ajout ne sera pas fait si l'offre existe déjà dans la liste des favoris
                    var offreExiste = favoris.OffresEmplois.Any(o => o.Id == offre.Id);
                    
                    if (!offreExiste) // Cette condition gère les collisions de GUID pour une même addresse IP
                    {
                        favoris.OffresEmplois.Add(offre);

                        // Calculer la taille totale de la cache
                        int taille = _utils.CalculerTailleCache(favoris.OffresEmplois);

                        cacheEntryOptions.Size = taille;
                        _memoryCache.Set(_cacheKey, favoris, cacheEntryOptions);
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        // Obtenir tous les offres dans la liste des favoris pour une addresse IPv4
        public IEnumerable<OffreEmploi> ObtenirTout(string adresseIPClient)
        {
            if(_utils.VerifierAdresseIP(adresseIPClient))
            {
                var favoris = _memoryCache.Get<Favori>(adresseIPClient);

                if(favoris != null)
                {
                    return favoris.OffresEmplois;
                }
            }
            return new List<OffreEmploi>();
        }

        // Supprimer une offre des favoris pour une addresse IPv4
        public bool Supprimer(string addresseIP, Guid id)
        {
            if(_utils.VerifierAdresseIP(addresseIP))
            {
                var favoris = _memoryCache.Get<Favori>(addresseIP);
                if(favoris != null)
                {
                    var favoriASupprimer = favoris.OffresEmplois.FirstOrDefault(o => o.Id == id);

                    if (favoriASupprimer != null)
                    {
                        favoris.OffresEmplois.Remove(favoriASupprimer);

                        // Calculer la taille totale de la cache
                        int taille = _utils.CalculerTailleCache(favoris.OffresEmplois);

                        // Configurer les réglages de la mémoire cache
                        var cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            // Les favoris ont une durée de 24h
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),

                            // Les favoris doivent expirer après 6h en cas de non-accès
                            SlidingExpiration = TimeSpan.FromHours(6),
                         
                            // Chaque caractère des données ajoutées au cache à une taille de 1
                            Size = taille
                        };

                        _memoryCache.Set(addresseIP, favoris, cacheEntryOptions);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

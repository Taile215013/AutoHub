using System.Collections.Generic;
using System.Linq;

namespace AutoHub.Services
{
    public interface ILocationService
    {
        List<string> GetCities();
        List<string> GetDistricts(string city);
        List<string> GetWards(string district);
    }

    public class LocationService : ILocationService
    {
        private readonly Dictionary<string, Dictionary<string, List<string>>> _locationsData;

        public LocationService()
        {
            var filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "data", "locations.json");
            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                _locationsData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(json) 
                                 ?? new Dictionary<string, Dictionary<string, List<string>>>();
            }
            else
            {
                _locationsData = new Dictionary<string, Dictionary<string, List<string>>>();
            }
        }

        public List<string> GetCities()
        {
            return _locationsData.Keys.ToList();
        }

        public List<string> GetDistricts(string city)
        {
            if (string.IsNullOrEmpty(city) || !_locationsData.ContainsKey(city))
                return new List<string>();

            return _locationsData[city].Keys.ToList();
        }

        public List<string> GetWards(string district)
        {
            if (string.IsNullOrEmpty(district)) return new List<string>();

            foreach (var cityData in _locationsData.Values)
            {
                if (cityData.ContainsKey(district))
                {
                    return cityData[district];
                }
            }
            return new List<string>();
        }
    }
}

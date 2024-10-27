using System.Text.Json.Serialization;

namespace AdsApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Ad> Ads { get; set; }
    }
}

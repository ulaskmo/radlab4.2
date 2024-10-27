using System.Text.Json.Serialization;

namespace AdsApi.Models
{
    public class Seller
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public ICollection<Ad> Ads { get; set; }
    }
}

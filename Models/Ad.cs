namespace AdsApi.Models
{
    public class Ad
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public Seller Seller { get; set; }
        public Category Category { get; set; }
    }
}

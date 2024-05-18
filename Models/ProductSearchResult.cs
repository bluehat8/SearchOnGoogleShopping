namespace SearchOnGoogleShopping.Models
{
    public class ProductSearchResult
    {
        public ProductSearchResult(string url, string? productId, bool isFromLaCasa)
        {
            this.url = url;
            ProductId = productId;
            IsFromLaCasa = isFromLaCasa;
        }

        public string? url { get; set; }
        public string? ProductId { get; set; }
        public bool IsFromLaCasa { get; set; }
    }
}

namespace AgenciaDeViajes.Models
{
    public class MercadoPagoPreferenceRequest
    {
        public List<Item> items { get; set; }
        public BackUrls back_urls { get; set; }
        public string auto_return { get; set; } = "approved";

        public class Item
        {
            public string title { get; set; }
            public int quantity { get; set; }
            public string currency_id { get; set; } = "PEN";
            public decimal unit_price { get; set; }
        }

        public class BackUrls
        {
            public string success { get; set; }
            public string failure { get; set; }
            public string pending { get; set; }
        }
    }
}

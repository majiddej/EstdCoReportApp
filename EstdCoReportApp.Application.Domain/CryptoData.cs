namespace EstdCoReportApp.Application.Domain
{
    public class CryptoData
    {
        public string asset_id_base { get; set; }
        public List<CryptoRate> rates { get; set; }
    }
    public class CryptoRate
    {
        public DateTime time { get; set; }
        public string asset_id_quote { get; set; }
        public decimal rate { get; set; }
    }
}

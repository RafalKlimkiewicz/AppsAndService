namespace Northwind.Background.Hangfire
{
    public class WriteMessageJobDetail
    {
        public string? Message { get; set; }
        public int Seconds { get; set; }
    }
}

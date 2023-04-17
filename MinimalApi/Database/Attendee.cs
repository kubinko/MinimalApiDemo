namespace MinimalApi.Database
{
    public record Attendee(string Name, string Email, int BirthYear)
    {
        public long Id { get; set; }
        public string InvoiceCode { get; set; } = string.Empty;
    };
}

namespace MinimalApi.Database
{
    public record Attendee(string Name, int YearOfBirth, string Email, string Phone)
    {
        public long Id { get; set; }
    };
}

namespace MinimalApi.Services
{
    public class InMemoryIdGenerator : IIdGenerator
    {
        private static long _currentId = 0;

        public long GenerateAttendeeId()
            => ++_currentId;
    }
}

using System;
using Editor.Front.DocumentSessions;

namespace Editor.Tests
{
    public class MockDateTimeService : IDateTimeService
    {
        public DateTime UtcNow { get; set; }

        public void AddSeconds(double value)
        {
            UtcNow = UtcNow.AddSeconds(value);
        }
    }
}
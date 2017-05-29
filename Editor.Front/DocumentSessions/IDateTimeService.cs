using System;

namespace Editor.Front.DocumentSessions
{
    public interface IDateTimeService
    {
        DateTime UtcNow { get; }
    }
}
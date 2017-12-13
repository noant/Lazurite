using System;

namespace Lazurite.Shared
{
    public delegate void EventsHandler<T>(object sender, EventsArgs<T> args);

    public class EventsArgs<T> : EventArgs
    {
        public T Value { get; private set; }

        public EventsArgs(T obj)
        {
            Value = obj;
        }
    }
}

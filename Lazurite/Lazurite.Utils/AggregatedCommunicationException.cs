using System;

namespace Lazurite.Utils
{
    public class AggregatedCommunicationException: Exception
    {
        private AggregatedCommunicationException(string msg) : base(msg)
        {
            //
        }

        public static void Throw(Exception parentException)
        {
            var message = parentException.GetType().Name + ": " + parentException.Message;
            throw new AggregatedCommunicationException(message);
        }
    }
}

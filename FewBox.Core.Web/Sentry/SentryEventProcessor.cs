using Sentry;
using Sentry.Extensibility;

namespace FewBox.Core.Web.Sentry
{
    public class SentryEventProcessor : ISentryEventProcessor
    {
        public SentryEvent Process(SentryEvent @event)
        {
            return @event;
        }
    }
}
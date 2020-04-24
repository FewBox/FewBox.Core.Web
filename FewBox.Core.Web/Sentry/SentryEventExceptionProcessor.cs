using System;
using Sentry;
using Sentry.Extensibility;

namespace FewBox.Core.Web.Sentry
{
    public class SentryEventExceptionProcessor : ISentryEventExceptionProcessor
    {
        public void Process(Exception exception, SentryEvent sentryEvent)
        {
        }
    }
}
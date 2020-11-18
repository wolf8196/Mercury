using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Mercury.Utils
{
    internal sealed partial class FluentScopeLogger : IFluentScopeLogger
    {
        private readonly List<KeyValuePair<string, object>> scopes;
        private readonly ILogger logger;

        // shortcut to not create new scope for log entries is already started
        // only 'using' scenario is accounted for
        private bool isScopeStarted;

        public FluentScopeLogger(ILogger logger)
        {
            this.logger = logger;
            scopes = new List<KeyValuePair<string, object>>();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            var ownScope = logger.BeginScope(scopes);

            if (state != null)
            {
                isScopeStarted = true;
                return new FluentScope(this, ownScope, logger.BeginScope(state));
            }
            else
            {
                return ownScope;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var scope = !isScopeStarted ? logger.BeginScope(scopes) : null;
            try
            {
                logger.Log(logLevel, eventId, state, exception, formatter);
            }
            finally
            {
                scope?.Dispose();
            }
        }

        public IFluentScopeLogger WithScope(string propertyName, object value)
        {
            scopes.Add(new KeyValuePair<string, object>(propertyName, value));
            return this;
        }
    }
}
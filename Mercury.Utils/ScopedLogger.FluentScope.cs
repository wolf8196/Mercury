using System;

namespace Mercury.Utils
{
    internal partial class FluentScopeLogger
    {
        private class FluentScope : IDisposable
        {
            private readonly FluentScopeLogger originator;
            private readonly IDisposable[] disposables;

            public FluentScope(FluentScopeLogger originator, params IDisposable[] disposables)
            {
                this.originator = originator;
                this.disposables = disposables;
            }

            public void Dispose()
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                    originator.isScopeStarted = false;
                }
            }
        }
    }
}
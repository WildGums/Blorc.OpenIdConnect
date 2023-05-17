namespace Blorc.OpenIdConnect
{
    using System;

    internal class PromiseHandlerContext
    {
        // The defaults allow 100 * 50 = 5000 ms (5 seconds) of retries
        private const int DefaultMaxAwaitDurationInMilliseconds = 50;
        private const int DefaultMaxRetryCount = 100;

        public PromiseHandlerContext(string identifier)
            : this(identifier, Array.Empty<object?>())
        {
            // Keep empty, see other ctor
        }

        public PromiseHandlerContext(string identifier, object?[]? args)
        {
            Identifier = identifier;
            Args = args;
        }

        public string Identifier { get; }

        public object?[]? Args { get; }

        public int MaximumAwaitIntervalInMilliseconds { get; set; } = DefaultMaxAwaitDurationInMilliseconds;

        public int MaximumRetryCount { get; set; } = DefaultMaxRetryCount;
    }
}

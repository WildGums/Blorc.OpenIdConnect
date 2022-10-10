namespace Blorc.OpenIdConnect.Tests
{
    using Microsoft.AspNetCore.Components;

    public class NavigationEventArgs
    {
        public string Uri { get; }

        public NavigationOptions Options { get; }

        public NavigationEventArgs(string uri, bool forceLoad)
            : this(uri, new NavigationOptions() { ForceLoad = forceLoad })
        {
            Uri = uri;
        }

        public NavigationEventArgs(string uri, NavigationOptions options)
        {
            Uri = uri;
            Options = options;
        }
    }
}

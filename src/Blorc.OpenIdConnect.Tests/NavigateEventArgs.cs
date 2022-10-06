namespace Blorc.OpenIdConnect.Tests
{
    using Microsoft.AspNetCore.Components;

    public class NavigateEventArgs
    {
        public string Uri { get; }

        public NavigationOptions Options { get; }

        public NavigateEventArgs(string uri, bool forceLoad)
            : this(uri, new NavigationOptions() { ForceLoad = forceLoad })
        {
            Uri = uri;
        }

        public NavigateEventArgs(string uri, NavigationOptions options)
        {
            Uri = uri;
            Options = options;
        }
    }
}

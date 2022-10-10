namespace Blorc.OpenIdConnect.Tests
{
    using System;
    using Microsoft.AspNetCore.Components;

    public class NavigationManagerStub : NavigationManager
    {
        public event EventHandler<NavigationEventArgs> Navigate;

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            OnNavigate(new NavigationEventArgs(uri, forceLoad));
        }

        protected override void NavigateToCore(string uri, NavigationOptions options)
        {
            OnNavigate(new NavigationEventArgs(uri, options));
        }

        protected virtual void OnNavigate(NavigationEventArgs e)
        {
            Navigate?.Invoke(this, e);
        }
    }
}

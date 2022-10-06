namespace Blorc.OpenIdConnect.Tests
{
    using System;
    using Microsoft.AspNetCore.Components;

    public class NavigationManagerStub : NavigationManager
    {
        public event EventHandler<NavigateEventArgs> Navigate;

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            OnNavigate(new NavigateEventArgs(uri, forceLoad));
        }

        protected override void NavigateToCore(string uri, NavigationOptions options)
        {
            OnNavigate(new NavigateEventArgs(uri, options));
        }

        protected virtual void OnNavigate(NavigateEventArgs e)
        {
            Navigate?.Invoke(this, e);
        }
    }
}

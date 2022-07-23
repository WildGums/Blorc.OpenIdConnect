namespace Blorc.OpenIdConnect
{
    using System;

    public class UserInactivityEventArgs
    {
        public UserInactivityEventArgs(TimeSpan signoutTimeSpan)
        {
            SignoutTimeSpan = signoutTimeSpan;
        }

        public TimeSpan SignoutTimeSpan { get; }
    }
}

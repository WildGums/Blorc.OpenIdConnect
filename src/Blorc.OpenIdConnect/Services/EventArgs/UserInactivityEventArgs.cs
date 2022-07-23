namespace Blorc.OpenIdConnect
{
    using System;

    public class UserInactivityEventArgs
    {
        public TimeSpan SignoutTimeSpan { get; }

        public UserInactivityEventArgs(TimeSpan signoutTimeSpan)
        {
            SignoutTimeSpan = signoutTimeSpan;
        }
    }
}

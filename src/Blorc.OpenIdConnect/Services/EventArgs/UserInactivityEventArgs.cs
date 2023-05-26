namespace Blorc.OpenIdConnect
{
    using System;

    public class UserInactivityEventArgs
    {
        public UserInactivityEventArgs(TimeSpan signOutTimeSpan)
        {
            SignOutTimeSpan = signOutTimeSpan;
        }

        public TimeSpan SignOutTimeSpan { get; }

        public TimeSpan? InactivityNotificationTimeSpan { get; set; } = null;
    }
}

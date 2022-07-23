namespace Blorc.OpenIdConnect
{
    using System;

    public static class OidcProviderOptionsExtensions
    {
        public static TimeSpan GetTimeForUserInactivityAutomaticLogout(this OidcProviderOptions options)
        {
            return TimeSpan.FromMilliseconds(options.TimeForUserInactivityAutomaticLogout);
        }

        public static TimeSpan GetTimeForUserInactivityNotification(this OidcProviderOptions options)
        {
            var time = options.TimeForUserInactivityAutomaticLogout;
            if (options.TimeForUserInactivityNotification > 0)
            {
                time = Math.Min(time, options.TimeForUserInactivityNotification);
            }

            return TimeSpan.FromMilliseconds(time);
        }
    }
}

namespace Blorc.OpenIdConnect
{
    using System;

    public static class OidcProviderOptionsExtensions
    {
        public static TimeSpan GetTimeForUserInactivityAutomaticSignout(this OidcProviderOptions options)
        {
            return TimeSpan.FromMilliseconds(options.TimeForUserInactivityAutomaticSignout);
        }

        public static TimeSpan GetTimeForUserInactivityNotification(this OidcProviderOptions options)
        {
            var time = options.TimeForUserInactivityAutomaticSignout;
            if (options.TimeForUserInactivityNotification > 0)
            {
                time = Math.Min(time, options.TimeForUserInactivityNotification);
            }

            return TimeSpan.FromMilliseconds(time);
        }
    }
}

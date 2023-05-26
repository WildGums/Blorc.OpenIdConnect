namespace Blorc.OpenIdConnect
{
    using System;

    public static class OidcProviderOptionsExtensions
    {
        public static TimeSpan GetTimeForUserInactivityAutomaticSignOut(this OidcProviderOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            return TimeSpan.FromMilliseconds(options.TimeForUserInactivityAutomaticSignOut);
        }

        public static TimeSpan GetTimeForUserInactivityNotification(this OidcProviderOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            var time = options.TimeForUserInactivityAutomaticSignOut;
            if (options.TimeForUserInactivityNotification > 0)
            {
                time = Math.Min(time, options.TimeForUserInactivityNotification);
            }

            return TimeSpan.FromMilliseconds(time);
        }
    }
}

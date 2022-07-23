namespace Blorc.OpenIdConnect
{
    using System;

    public class UserActivityEventArgs
    {
        public UserActivityEventArgs(DateTime inactivityStartTime, DateTime inactivityEndTime)
        {
            InactivityStartTime = inactivityStartTime;
            InactivityEndTime = inactivityEndTime;
        }

        public DateTime InactivityStartTime { get; }

        public DateTime InactivityEndTime { get; }

        public bool ResetTime { get; set; } = true;
    }
}

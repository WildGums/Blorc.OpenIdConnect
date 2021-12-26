namespace Blorc.OpenIdConnect.Tests.Development
{
    using System.Collections.Generic;

    public class Roles
    {
        static Roles()
        {
            All = new List<string>();
        }

        public static IReadOnlyList<string> All { get; }
    }
}

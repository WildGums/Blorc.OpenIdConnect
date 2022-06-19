namespace Blorc.OpenIdConnect
{
    using System.Collections.Generic;

    public interface IHasRoles 
    {
        IEnumerable<string> Roles { get; }
    }
}
